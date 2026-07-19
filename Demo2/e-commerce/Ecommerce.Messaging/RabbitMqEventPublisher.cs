using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Ecommerce.Messaging
{
    // Best-effort publisher: one shared connection, a fresh channel per publish
    // (IChannel isn't safe to share across concurrent calls).
    public sealed class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private IConnection? _connection;

        public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
        }

        public async Task PublishAsync<TEvent>(string routingKey, TEvent @event, CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync(cancellationToken);

            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await using (channel.ConfigureAwait(false))
            {
                await channel.ExchangeDeclareAsync(
                    _options.ExchangeName,
                    ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                var body = JsonSerializer.SerializeToUtf8Bytes(@event);
                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };

                await channel.BasicPublishAsync(
                    _options.ExchangeName,
                    routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);
            }
        }

        private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { IsOpen: true })
                {
                    return _connection;
                }

                var factory = new ConnectionFactory
                {
                    HostName = _options.Host,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }

            _connectionLock.Dispose();
        }
    }
}
