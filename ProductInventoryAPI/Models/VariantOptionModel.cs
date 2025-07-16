namespace ProductInventoryAPI.Models
{
    public class VariantOptionModel
    {
        public Guid Id { get; set; }
        public Guid VariantId { get; set; }
        public string Name { get; set; }
    }
}
