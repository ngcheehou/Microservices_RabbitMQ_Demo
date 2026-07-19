namespace PaymentService.API.ExternalServices.Models
{
    public class OrderSummary
    {
        public List<OrderItemSummary> Items { get; set; } = new();
    }
}
