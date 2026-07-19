using System.Text.Json;
using Ecommerce.Messaging;
using Ecommerce.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentReadService.API.Domain;
using PaymentReadService.API.Infrastructure;
using RabbitMQ.Client;

namespace PaymentReadService.API.Messaging
{
    public class PaymentEventConsumer : RabbitMqConsumerBackgroundService
    {
        public PaymentEventConsumer(
            IOptions<RabbitMqOptions> options,
            IServiceScopeFactory scopeFactory,
            ILogger<PaymentEventConsumer> logger)
            : base(options, scopeFactory, logger)
        {
        }

        protected override string QueueName => "payment-read-service.events";

        protected override IReadOnlyCollection<string> RoutingKeys => new[]
        {
            EventRoutingKeys.PaymentCreated,
            EventRoutingKeys.OrderCreated,
            EventRoutingKeys.CustomerCreated,
            EventRoutingKeys.CustomerUpdated,
            EventRoutingKeys.ProductCreated,
            EventRoutingKeys.ProductUpdated
        };

        protected override async Task HandleMessageAsync(
            string routingKey,
            string jsonBody,
            IServiceProvider scopedProvider,
            CancellationToken cancellationToken)
        {
            var db = scopedProvider.GetRequiredService<PaymentReadDbContext>();

            switch (routingKey)
            {
                case EventRoutingKeys.CustomerCreated:
                case EventRoutingKeys.CustomerUpdated:
                    await HandleCustomerEventAsync(db, jsonBody, cancellationToken);
                    break;

                case EventRoutingKeys.ProductCreated:
                case EventRoutingKeys.ProductUpdated:
                    await HandleProductEventAsync(db, jsonBody, cancellationToken);
                    break;

                case EventRoutingKeys.OrderCreated:
                    await HandleOrderCreatedAsync(db, jsonBody, cancellationToken);
                    break;

                case EventRoutingKeys.PaymentCreated:
                    await HandlePaymentCreatedAsync(db, jsonBody, cancellationToken);
                    break;
            }
        }

        // CustomerCreatedEvent and CustomerUpdatedEvent share the same shape, so one
        // handler covers both routing keys.
        private static async Task HandleCustomerEventAsync(PaymentReadDbContext db, string jsonBody, CancellationToken cancellationToken)
        {


            var evt = JsonSerializer.Deserialize<CustomerCreatedEvent>(jsonBody);
            if (evt == null)
            {
                return;
            }

            var customer = await db.Customers.FindAsync(new object[] { evt.CustomerId }, cancellationToken);
            if (customer == null)
            {
                customer = new CustomerCache { CustomerId = evt.CustomerId };
                db.Customers.Add(customer);
            }

            customer.FirstName = evt.FirstName;
            customer.LastName = evt.LastName;
            customer.UpdatedDate = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }

        // ProductCreatedEvent and ProductUpdatedEvent share the same shape, so one
        // handler covers both routing keys.
        private static async Task HandleProductEventAsync(PaymentReadDbContext db, string jsonBody, CancellationToken cancellationToken)
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

        private static async Task HandleOrderCreatedAsync(PaymentReadDbContext db, string jsonBody, CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(jsonBody);
            if (evt == null)
            {
                return;
            }

            var map = await db.OrderCustomerMap.FindAsync(new object[] { evt.OrderId }, cancellationToken);
            if (map != null)
            {
                return; // already processed (e.g. redelivery)
            }

            db.OrderCustomerMap.Add(new OrderCustomerMap { OrderId = evt.OrderId, CustomerId = evt.CustomerId });

            foreach (var item in evt.Items)
            {
                var product = await db.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);

                db.OrderItems.Add(new OrderItemInfo
                {
                    Id = Guid.NewGuid(),
                    OrderId = evt.OrderId,
                    ProductId = item.ProductId,
                    ProductName = product?.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private static async Task HandlePaymentCreatedAsync(PaymentReadDbContext db, string jsonBody, CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<PaymentCreatedEvent>(jsonBody);
            if (evt == null)
            {
                return;
            }

            var alreadyProcessed = await db.Payments.FindAsync(new object[] { evt.PaymentId }, cancellationToken);
            if (alreadyProcessed != null)
            {
                return;
            }

            var map = await db.OrderCustomerMap.FindAsync(new object[] { evt.OrderId }, cancellationToken);
            CustomerCache? customer = null;
            if (map != null)
            {
                customer = await db.Customers.FindAsync(new object[] { map.CustomerId }, cancellationToken);
            }

            var payment = new PaymentReadModel
            {
                PaymentId = evt.PaymentId,
                OrderId = evt.OrderId,
                CustomerId = map?.CustomerId,
                CustomerName = customer == null ? null : $"{customer.FirstName} {customer.LastName}",
                Amount = evt.Amount,
                Status = evt.Status,
                CreatedDate = evt.CreatedDate
            };

            db.Payments.Add(payment);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
