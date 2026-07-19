namespace OrderService.API.Application.DTOs
{
    public class CreateItemRequest
    {
        public required Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
