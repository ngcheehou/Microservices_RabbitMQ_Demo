namespace OrderService.API.Application.DTOs
{
    public class CreateOrderRequest
    {
        public required Guid CustomerId { get; set; }

        public required List<CreateItemRequest> Items { get; set; }
    }
}
