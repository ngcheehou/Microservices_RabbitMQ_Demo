using System.Text.Json;
using Ecommerce.Messaging;
using Ecommerce.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderReadService.API.Domain;
using OrderReadService.API.Infrastructure;

namespace OrderReadService.API.Messaging
{
    public class OrderEventConsumer : RabbitMqConsumerBackgroundService
    {
        public OrderEventConsumer(
            IOptions<RabbitMqOptions> options,
            IServiceScopeFactory scopeFactory,
            ILogger<OrderEventConsumer> logger)
            : base(options, scopeFactory, logger)
        {
        }

        protected override string QueueName => "order-read-service.events";

        protected override IReadOnlyCollection<string> RoutingKeys => new[]
        {
            EventRoutingKeys.OrderCreated,
            EventRoutingKeys.ProductCreated,
            EventRoutingKeys.ProductUpdated
        };

        protected override async Task HandleMessageAsync(
            string routingKey,
            string jsonBody,
            IServiceProvider scopedProvider,
            CancellationToken cancellationToken)
        {
            var db = scopedProvider.GetRequiredService<OrderReadDbContext>();

            switch (routingKey)
            {
                case EventRoutingKeys.ProductCreated:
                case EventRoutingKeys.ProductUpdated:
                    await HandleProductEventAsync(db, jsonBody, cancellationToken);
                    break;

                case EventRoutingKeys.OrderCreated:
                    await HandleOrderCreatedAsync(db, jsonBody, cancellationToken);
                    break;
            }
        }

        // ProductCreatedEvent and ProductUpdatedEvent share the same shape, so one
        // handler covers both routing keys.
        private static async Task HandleProductEventAsync(OrderReadDbContext db, string jsonBody, CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<ProductCreatedEvent>(jsonBody);
            if (evt == null)
            {
                return;
            }

            var product = await db.Products.FindAsync(new object[] { evt.ProductId }, cancellationToken);
            if (product == null)
            {
                product = new ProductCache { ProductId = evt.ProductId };
                db.Products.Add(product);
            }

            product.Name = evt.Name;
            product.Price = evt.Price;
            product.UpdatedDate = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }

        private static async Task HandleOrderCreatedAsync(OrderReadDbContext db, string jsonBody, CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(jsonBody);
            if (evt == null)
            {
                return;
            }

            var alreadyProcessed = await db.Orders.FindAsync(new object[] { evt.OrderId }, cancellationToken);
            if (alreadyProcessed != null)
            {
                return;
            }

            var order = new OrderReadModel
            {
                OrderId = evt.OrderId,
                CustomerId = evt.CustomerId,
                CreatedDate = evt.CreatedDate
            };

            foreach (var item in evt.Items)
            {
                var product = await db.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);

                order.Items.Add(new OrderItemReadModel
                {
                    Id = Guid.NewGuid(),
                    OrderId = evt.OrderId,
                    ProductId = item.ProductId,
                    ProductName = product?.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
