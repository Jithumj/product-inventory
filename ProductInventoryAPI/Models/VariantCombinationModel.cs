namespace ProductInventoryAPI.Models
{
    public class VariantCombinationModel
    {
        public Guid Id { get; set; }
        public string CombinationCode { get; set; }
        public decimal Stock { get; set; }
        public List<OptionModel> Options { get; set; } = new();
    }
}
