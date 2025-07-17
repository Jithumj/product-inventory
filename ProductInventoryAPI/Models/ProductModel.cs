namespace ProductInventoryAPI.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public byte[] ProductImage { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public Guid CreatedUser { get; set; }
        public bool IsFavourite { get; set; }
        public bool Active { get; set; }
        public string HSNCode { get; set; }
        public decimal TotalStock { get; set; }
        public List<ProductVariantCombinationModel> VariantCombinations { get; set; } = new();

    }
}
