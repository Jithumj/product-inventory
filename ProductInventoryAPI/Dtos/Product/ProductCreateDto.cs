namespace ProductInventoryAPI.Dtos.Product
{
    public class ProductCreateDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        //  public string HSNCode { get; set; }
        public Guid CreatedUser { get; set; }
        public List<VariantDto> Variants { get; set; }
    }
}
