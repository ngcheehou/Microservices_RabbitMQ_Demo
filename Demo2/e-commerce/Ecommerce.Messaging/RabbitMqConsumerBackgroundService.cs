using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecommerce.Messaging
{
    // Declares its own durable queue against the shared topic exchange, binds the
    // routing keys the derived class cares about, and dispatches each message inside
    // a fresh DI scope (so DbContext-per-message works like it does for HTTP requests).
    public abstract class RabbitMqConsumerBackgroundService : BackgroundService
    {
        private readonly RabbitMqOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        protected RabbitMqConsumerBackgroundService(
            IOptions<RabbitMqOptions> options,
            IServiceScopeFactory scopeFactory,
            ILogger logger)
        {
            _options = options.Value;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected abstract string QueueName { get; }

        protected abstract IReadOnlyCollection<string> RoutingKeys { get; }

        protected abstract Task HandleMessageAsync(
            string routingKey,
            string jsonBody,
            IServiceProvider scopedProvider,
            CancellationToken cancellationToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            IConnection? connection = null;
            while (connection == null && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    connection = await factory.CreateConnectionAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ not reachable yet, retrying in 5s...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            if (connection == null)
            {
                return;
            }

            await using var conn = connection;
            var channel = await conn.CreateChannelAsync(cancellationToken: stoppingToken);
            await using var ch = channel;

            await ch.ExchangeDeclareAsync(
                _options.ExchangeName,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await ch.QueueDeclareAsync(
                QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            foreach (var routingKey in RoutingKeys)
            {
                await ch.QueueBindAsync(QueueName, _options.ExchangeName, routingKey, cancellationToken: stoppingToken);
            }

            var consumer = new AsyncEventingBasicConsumer(ch);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    await HandleMessageAsync(ea.RoutingKey, json, scope.ServiceProvider, stoppingToken);
                    await ch.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to handle message with routing key {RoutingKey}", ea.RoutingKey);
                    await ch.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                }
            };

            await ch.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown.
            }
        }
    }
}
