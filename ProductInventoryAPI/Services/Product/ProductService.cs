using Dapper;
using ProductInventoryAPI.Data;
using ProductInventoryAPI.Dtos.Product;
using ProductInventoryAPI.Models;
using ProductInventoryAPI.Services.Logging;

namespace ProductInventoryAPI.Services.Product
{
    public class ProductService:IProductService
    {
        private readonly DapperContext _context;
        private readonly ILoggerService _loggerService;

        public ProductService(DapperContext context,ILoggerService loggerService)
        {
            _context = context;
            _loggerService = loggerService;
        }

        # region Create product
        public async Task<Guid> CreateProductAsync(ProductCreateDto dto)
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            var productId = Guid.NewGuid();

            var product = new ProductModel
            {
                Id = productId,
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                CreatedUser = dto.CreatedUser,
                Active = true,
                IsFavourite = false,
                HSNCode = "", 
                TotalStock = 0
            };

            var sqlProduct = @"INSERT INTO Products (Id, ProductCode, ProductName, CreatedDate, UpdatedDate, CreatedUser, IsFavourite, Active, HSNCode, TotalStock)
                           VALUES (@Id, @ProductCode, @ProductName, @CreatedDate, @UpdatedDate, @CreatedUser, @IsFavourite, @Active, @HSNCode, @TotalStock)";
            await conn.ExecuteAsync(sqlProduct, product, tran);

            // Insert variants and options
            var variantIds = new List<Guid>();
            var optionDict = new Dictionary<string, List<(Guid, string)>>(); // list of (id, optionName)
            foreach (var v in dto.Variants)
            {
                var variantId = Guid.NewGuid();
                variantIds.Add(variantId);
                var sqlVariant = @"INSERT INTO Variants (Id, ProductId, Name) VALUES (@Id, @ProductId, @Name)";
                await conn.ExecuteAsync(sqlVariant, new { Id = variantId, ProductId = productId, Name = v.Name }, tran);

                optionDict[v.Name] = new List<(Guid, string)>();
                foreach (var option in v.Options)
                {
                    var optionId = Guid.NewGuid();
                    optionDict[v.Name].Add((optionId, option));
                    var sqlOption = @"INSERT INTO VariantOptions (Id, VariantId, Name) VALUES (@Id, @VariantId, @Name)";
                    await conn.ExecuteAsync(sqlOption, new { Id = optionId, VariantId = variantId, Name = option }, tran);
                }
            }

            // Generate all combinations (cartesian product)
            var optionLists = optionDict.Values.Select(list => list.Select(vo => vo).ToList()).ToList();
            var allCombos = GetCartesianProduct(optionLists);

            foreach (var combo in allCombos)
            {
                string combinationCode = string.Join("-", combo.Select(x => x.Item2));
                var combinationId = Guid.NewGuid();

                var sqlCombo = @"INSERT INTO ProductVariantCombinations (Id, ProductId, CombinationCode, Stock) 
                             VALUES (@Id, @ProductId, @CombinationCode, 0)";
                await conn.ExecuteAsync(sqlCombo, new
                {
                    Id = combinationId,
                    ProductId = productId,
                    CombinationCode = combinationCode
                }, tran);

                foreach (var (optionId, _, idx) in combo.Select((tuple, idx) => (tuple.Item1, tuple.Item2, idx)))
                {
                    var variantId = variantIds[idx];
                    var sqlLink = @"INSERT INTO ProductVariantCombinationOptions (ProductVariantCombinationId, VariantId, VariantOptionId)
                                VALUES (@ProductVariantCombinationId, @VariantId, @VariantOptionId)";
                    await conn.ExecuteAsync(sqlLink, new
                    {
                        ProductVariantCombinationId = combinationId,
                        VariantId = variantId,
                        VariantOptionId = optionId
                    }, tran);
                }
            }

            tran.Commit();
            return productId;
        }

        //  function for cartesian product
        private static IEnumerable<List<(Guid, string)>> GetCartesianProduct(List<List<(Guid, string)>> sequences)
        {
            IEnumerable<List<(Guid, string)>> cartesianProduct = new List<List<(Guid, string)>> { new List<(Guid, string)>() };
            foreach (var sequence in sequences)
            {
                cartesianProduct = from cp in cartesianProduct
                                   from item in sequence
                                   select new List<(Guid, string)>(cp) { item };
            }
            return cartesianProduct;
        }
        #endregion

        #region Get product by id
        public async Task<ProductResponseModel> GetProductByIdAsync(Guid productId)
        {

            using var conn = _context.CreateConnection();

            // 1. Get the product itself
            var product = await conn.QueryFirstOrDefaultAsync<ProductModel>(
                "SELECT * FROM Products WHERE Id = @Id", new { Id = productId });
            if (product == null) return null;
            try
            {
                // 2. Get variants and their option names
                var variants = (await conn.QueryAsync<(string VariantName, string OptionName)>(
                    @"SELECT v.Name AS VariantName, o.Name AS OptionName
                       FROM Variants v
                       JOIN VariantOptions o ON o.VariantId = v.Id
                       WHERE v.ProductId = @Id", new { Id = productId })).ToList();

                var variantsGrouped = variants.GroupBy(x => x.VariantName)
                    .Select(g => new VariantDto
                    {
                        Name = g.Key,
                        Options = g.Select(xx => xx.OptionName).ToList()
                    }).ToList();

                // 3. Get all combinations with their option mappings
                var combinations = (await conn.QueryAsync<dynamic>(
                    @"SELECT pvc.Id, pvc.CombinationCode, pvc.Stock, v.Name AS VariantName, o.Name AS OptionName
                    FROM ProductVariantCombinations pvc
                    JOIN ProductVariantCombinationOptions pvco ON pvc.Id = pvco.ProductVariantCombinationId
                    JOIN Variants v ON pvco.VariantId = v.Id
                    JOIN VariantOptions o ON pvco.VariantOptionId = o.Id
                    WHERE pvc.ProductId = @Id
                    ORDER BY pvc.Id", new { Id = productId })).ToList();

                var combGroups = combinations
                    .GroupBy(x => new { x.Id, x.CombinationCode, x.Stock })
                    .Select(g => new VariantCombinationModel
                    {
                        Id = g.Key.Id,
                        CombinationCode = g.Key.CombinationCode,
                        Stock = g.Key.Stock,
                        Options = g.Select(x => new OptionModel
                        {
                            Variant = x.VariantName,
                            Option = x.OptionName
                        }).ToList()
                    })
                    .ToList();

                // 4. Compose response
                return new ProductResponseModel
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    Variants = variantsGrouped,
                    VariantCombinations = combGroups
                };
            }
            catch(Exception ex)
            {
                _loggerService.LogAsync(ex.Message);

            }
            return new ProductResponseModel();
        }



        #endregion

        #region Get product
        public async Task<IEnumerable<ProductModel>> GetProductsAsync(int page, int pageSize)
        {
            using var conn = _context.CreateConnection();

            // Get products (paginated)
            var sqlProducts = @"SELECT * FROM Products
                        ORDER BY CreatedDate DESC
                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
            var products = (await conn.QueryAsync<ProductModel>(sqlProducts,
                new { Skip = (page - 1) * pageSize, Take = pageSize })).ToList();

            if (!products.Any())
                return products;
            try
            {

                var productIds = products.Select(p => p.Id).ToList();

                // Get all combinations for these products
                var sqlCombinations = @"SELECT * FROM ProductVariantCombinations WHERE ProductId IN @Ids";
                var combinations = (await conn.QueryAsync<ProductVariantCombinationModel>(
                    sqlCombinations, new { Ids = productIds })).ToList();

                foreach (var p in products)
                    p.VariantCombinations = combinations.Where(c => c.ProductId == p.Id).ToList();

                // Get all variant/options for these products (joins all at once)
                var sqlVariants = @"
                            SELECT v.ProductId, v.Name AS VariantName, o.Name AS OptionName
                                 FROM Variants v
                                 JOIN VariantOptions o ON o.VariantId = v.Id
                            WHERE v.ProductId IN @Ids";
                var variantsFlat = (await conn.QueryAsync<(Guid ProductId, string VariantName, string OptionName)>(sqlVariants, new { Ids = productIds }))
                                    .ToList();

                // Group them by product & by variant name:
                var groupedVariants = variantsFlat
                    .GroupBy(x => x.ProductId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.GroupBy(x => x.VariantName)
                              .Select(vg => new VariantDto
                              {
                                  Name = vg.Key,
                                  Options = vg.Select(v => v.OptionName).ToList()
                              }).ToList()
                    );

                // Attach to each product:
                foreach (var p in products)
                    if (groupedVariants.ContainsKey(p.Id))
                        p.Variants = groupedVariants[p.Id];
            }
            catch (Exception ex)
            {
                _loggerService.LogAsync(ex.Message);
                return new List<ProductModel>();
            }
            return products;
        }


        #endregion

        #region Addstock
        public async Task<bool> AddStockAsync(StockUpdateDto dto)
        {
            try
            {
                using var conn = _context.CreateConnection();
                var getCombo = "SELECT * FROM ProductVariantCombinations WHERE Id = @Id";
                var combo = await conn.QueryFirstOrDefaultAsync<ProductVariantCombinationModel>(getCombo, new { Id = dto.CombinationId });
                if (combo == null) return false;

                var sql = @"UPDATE ProductVariantCombinations SET Stock = Stock + @Qty WHERE Id = @Id;
                    UPDATE Products SET TotalStock = TotalStock + @Qty WHERE Id = @Pid";
                await conn.ExecuteAsync(sql, new { Qty = dto.Quantity, Id = dto.CombinationId, Pid = combo.ProductId });
                return true;
            }

            catch (Exception ex)
            {
                 _loggerService.LogAsync(ex.Message);
                return false;
            }
        }
        #endregion

        #region RemoveStock
        public async Task<bool> RemoveStockAsync(StockUpdateDto dto)
        {
            try
            {
                using var conn = _context.CreateConnection();
                var getCombo = "SELECT * FROM ProductVariantCombinations WHERE Id = @Id";
                var combo = await conn.QueryFirstOrDefaultAsync<ProductVariantCombinationModel>(getCombo, new { Id = dto.CombinationId });
                if (combo == null || combo.Stock < dto.Quantity) return false;

                var sql = @"UPDATE ProductVariantCombinations SET Stock = Stock - @Qty WHERE Id = @Id;
                    UPDATE Products SET TotalStock = TotalStock - @Qty WHERE Id = @Pid";
                await conn.ExecuteAsync(sql, new { Qty = dto.Quantity, Id = dto.CombinationId, Pid = combo.ProductId });
                return true;
            }
            catch(Exception ex)
            {
                _loggerService.LogAsync(ex.Message);
                return false;
            }
        }
        #endregion
    }
}
