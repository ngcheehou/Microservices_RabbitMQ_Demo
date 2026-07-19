namespace OrderReadService.API.Domain
{
    public class OrderReadModel
    {
        public Guid OrderId { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<OrderItemReadModel> Items { get; set; } = new();
    }
}
