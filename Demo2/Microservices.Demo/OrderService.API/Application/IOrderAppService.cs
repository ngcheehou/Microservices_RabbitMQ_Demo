using OrderService.API.Application.DTOs;
using OrderService.API.Domain;

namespace OrderService.API.Application
{
    public interface IOrderAppService
    {
        Task<List<Order>> GetOrders();

        Order? GetOrderById(Guid id);

        Task<List<Order>> GetOrdersByCustomerId(Guid id);

        Task<CreateOrderResult> CreateOrder(CreateOrderRequest request);

        Order? UpdateOrder(Guid id, UpdateOrderRequest request);

        bool DeleteOrder(Guid id);
    }
}
