using ProductInventoryAPI.Dtos.Product;
using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Services.Product
{
    public interface IProductService
    {
        Task<Guid> CreateProductAsync(ProductCreateDto dto);
        Task<ProductModel> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductModel>> GetProductsAsync(int page, int pageSize);
        Task<bool> AddStockAsync(StockUpdateDto dto);
        Task<bool> RemoveStockAsync(StockUpdateDto dto);
    }
}
