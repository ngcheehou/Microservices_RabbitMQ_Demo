using OrderService.API.Domain;

namespace OrderService.API.Application.DTOs
{
    public class UpdateOrderRequest
    {
        public required OrderStatus Status { get; set; }
    }
}
