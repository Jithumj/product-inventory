namespace ProductInventoryAPI.Models
{
    public class ProductVariantCombinationModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string CombinationCode { get; set; }
        public decimal Stock { get; set; }
    }
}
