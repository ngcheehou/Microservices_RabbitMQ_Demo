namespace OrderService.API.Domain
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        public DateTime CreatedDate { get; set; }
    }
}
