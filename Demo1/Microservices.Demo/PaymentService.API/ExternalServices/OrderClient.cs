using System.Net.Http.Json;
using PaymentService.API.ExternalServices.Interfaces;
using PaymentService.API.ExternalServices.Models;

namespace PaymentService.API.ExternalServices
{
    public class OrderClient : IOrderClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<OrderSummary?> GetOrderAsync(Guid orderId)
        {
            var client = _httpClientFactory.CreateClient("OrderService");
            var response = await client.GetAsync($"api/Order/GetOrdersById/{orderId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<OrderSummary>();
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, int status)
        {
            var client = _httpClientFactory.CreateClient("OrderService");
            var response = await client.PutAsJsonAsync($"api/Order/UpdateOrder/{orderId}", new { Status = status });
            return response.IsSuccessStatusCode;
        }
    }
}
