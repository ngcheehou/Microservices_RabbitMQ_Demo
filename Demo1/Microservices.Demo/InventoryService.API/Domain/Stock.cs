namespace InventoryService.API.Domain
{
    public class Stock
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
