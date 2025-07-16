namespace ProductInventoryAPI.Models
{
    public class ProductVariantCombinationOptionModel
    {
        public Guid ProductVariantCombinationId { get; set; }
        public Guid VariantId { get; set; }
        public Guid VariantOptionId { get; set; }
    }
}
