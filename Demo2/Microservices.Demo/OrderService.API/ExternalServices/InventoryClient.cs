using System.Net.Http.Json;
using OrderService.API.ExternalServices.Interfaces;
using OrderService.API.ExternalServices.Models;

namespace OrderService.API.ExternalServices
{
    public class InventoryClient : IInventoryClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public InventoryClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ProductSummary?> GetProductAsync(Guid productId)
        {
            var client = _httpClientFactory.CreateClient("InventoryService");
            var response = await client.GetAsync($"api/Product/GetProductById/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductSummary>();
        }

        public async Task<bool> ReduceStockAsync(Guid productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient("InventoryService");
            var response = await client.PutAsJsonAsync(
                $"api/Product/ReduceStock/{productId}",
                new AdjustStockRequest { Quantity = quantity });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IncreaseStockAsync(Guid productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient("InventoryService");
            var response = await client.PutAsJsonAsync(
                $"api/Product/IncreaseStock/{productId}",
                new AdjustStockRequest { Quantity = quantity });

            return response.IsSuccessStatusCode;
        }

        private sealed class AdjustStockRequest
        {
            public int Quantity { get; set; }
        }
    }
}
