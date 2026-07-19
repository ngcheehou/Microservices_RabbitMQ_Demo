using PaymentService.API.ExternalServices.Models;

namespace PaymentService.API.ExternalServices.Interfaces
{
    public interface IOrderClient
    {
        Task<OrderSummary?> GetOrderAsync(Guid orderId);

        Task<bool> UpdateOrderStatusAsync(Guid orderId, int status);
    }
}
