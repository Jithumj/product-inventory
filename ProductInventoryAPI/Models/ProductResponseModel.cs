using ProductInventoryAPI.Dtos.Product;

namespace ProductInventoryAPI.Models
{
    public class ProductResponseModel
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public List<VariantDto> Variants { get; set; } = new();
        public List<VariantCombinationModel> VariantCombinations { get; set; } = new();
    }
}
