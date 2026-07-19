namespace PaymentReadService.API.Domain
{
    // Built from an order.created event's Items, so a payment's detail can list
    // what was purchased (with product name) without calling OrderService.
    public class OrderItemInfo
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
