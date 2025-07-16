namespace ProductInventoryAPI.Dtos.Product
{
    public class StockUpdateDto
    {
        public Guid CombinationId { get; set; }
        public decimal Quantity { get; set; }
    }
}
