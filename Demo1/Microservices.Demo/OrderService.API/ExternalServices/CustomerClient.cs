using System.Net.Http.Json;
using OrderService.API.ExternalServices.Interfaces;
using OrderService.API.ExternalServices.Models;

namespace OrderService.API.ExternalServices
{
    public class CustomerClient : ICustomerClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CustomerSummary?>  GetCustomerByIdAsync(Guid id)
        {
            var customerClient = _httpClientFactory.CreateClient("CustomerService");
            var customerResponse = await customerClient.GetAsync($"api/Customer/GetCustomerById/{id}");

            if (!customerResponse.IsSuccessStatusCode)
            {
                return null;
            }

            return await customerResponse.Content.ReadFromJsonAsync<CustomerSummary>();
        }
    }
}
