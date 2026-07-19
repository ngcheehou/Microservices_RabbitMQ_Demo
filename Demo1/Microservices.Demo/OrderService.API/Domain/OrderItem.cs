namespace OrderService.API.Domain
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
