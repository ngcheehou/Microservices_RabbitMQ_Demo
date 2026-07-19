namespace OrderReadService.API.Domain
{
    public class OrderItemReadModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        // Snapshot from ProductCache at the time the order.created event was handled;
        // null if the product event hadn't arrived yet.
        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
