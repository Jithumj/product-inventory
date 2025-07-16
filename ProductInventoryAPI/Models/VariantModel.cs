namespace ProductInventoryAPI.Models
{
    public class VariantModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
    }
}
