using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Domain;
using OrderService.API.Infrastructure;

namespace OrderService.API.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(OrderDbContext db, IConfiguration configuration)
        {
            if (db.Orders.Any())
            {
                return;
            }

            var customerIds = await FetchCustomerIdsAsync(configuration);
            var products = await FetchProductsAsync(configuration);

            if (customerIds.Count == 0 || products.Count == 0)
            {
                Console.WriteLine("Skipping order seed - CustomerService or InventoryService returned no data. Make sure both services have run at least once and are reachable.");
                return;
            }

            var random = new Random();
            var orders = new List<Order>();

            for (var i = 0; i < 10; i++)
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerIds[random.Next(customerIds.Count)],
                    Status = OrderStatus.Pending
                };

                var itemCount = random.Next(1, 4);
                for (var j = 0; j < itemCount; j++)
                {
                    var product = products[random.Next(products.Count)];

                    order.Items.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = random.Next(1, 5),
                        Price = product.Price
                    });
                }

                orders.Add(order);
            }

            db.Orders.AddRange(orders);
            db.SaveChanges();
        }

        private static async Task<List<Guid>> FetchCustomerIdsAsync(IConfiguration configuration)
        {
            var baseUrl = configuration["Services:CustomerService"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return new List<Guid>();
            }

            try
            {
                using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
                var customers = await client.GetFromJsonAsync<List<CustomerSummary>>("api/Customers/GetCustomers");
                return customers?.Select(c => c.Id).ToList() ?? new List<Guid>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Could not reach CustomerService at {baseUrl}: {ex.Message}");
                return new List<Guid>();
            }
        }

        private static async Task<List<ProductSummary>> FetchProductsAsync(IConfiguration configuration)
        {
            var baseUrl = configuration["Services:InventoryService"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return new List<ProductSummary>();
            }

            try
            {
                using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
                var products = await client.GetFromJsonAsync<List<ProductSummary>>("api/Product/GetProducts");
                return products ?? new List<ProductSummary>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Could not reach InventoryService at {baseUrl}: {ex.Message}");
                return new List<ProductSummary>();
            }
        }

        private sealed class CustomerSummary
        {
            public Guid Id { get; set; }
        }

        private sealed class ProductSummary
        {
            public Guid Id { get; set; }

            public decimal Price { get; set; }
        }
    }
}
