using Dapper;
using ProductInventoryAPI.Data;
using ProductInventoryAPI.Dtos.Product;
using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Services.Product
{
    public class ProductService:IProductService
    {
        private readonly DapperContext _context;

        public ProductService(DapperContext context)
        {
            _context = context;
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
                HSNCode = "", // set accordingly
                TotalStock = 0
            };

            var sqlProduct = @"INSERT INTO Products (Id, ProductCode, ProductName, CreatedDate, UpdatedDate, CreatedUser, IsFavourite, Active, HSNCode, TotalStock)
                           VALUES (@Id, @ProductCode, @ProductName, @CreatedDate, @UpdatedDate, @CreatedUser, @IsFavourite, @Active, @HSNCode, @TotalStock)";
            await conn.ExecuteAsync(sqlProduct, product, tran);

            // Insert variants and options
            var variantIds = new List<Guid>();
            var optionDict = new Dictionary<string, List<(Guid, string)>>(); // name -> list of (id, optionName)
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

        // Helper function for cartesian product
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
        public async Task<ProductModel> GetProductByIdAsync(Guid id)
        {
            using var conn = _context.CreateConnection();
            string sql = @"SELECT * FROM Products WHERE Id = @Id";
            var product = await conn.QueryFirstOrDefaultAsync<ProductModel>(sql, new { Id = id });
            return product;
        }

        #endregion

        #region Get product
        public async Task<IEnumerable<ProductModel>> GetProductsAsync(int page, int pageSize)
        {
            using var conn = _context.CreateConnection();
            var sql = @"SELECT * FROM Products ORDER BY CreatedDate DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
            return await conn.QueryAsync<ProductModel>(sql, new { Skip = (page - 1) * pageSize, Take = pageSize });
        }
        #endregion

        #region Addstock
        public async Task<bool> AddStockAsync(StockUpdateDto dto)
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
        #endregion

        #region RemoveStock
        public async Task<bool> RemoveStockAsync(StockUpdateDto dto)
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
        #endregion
    }
}
