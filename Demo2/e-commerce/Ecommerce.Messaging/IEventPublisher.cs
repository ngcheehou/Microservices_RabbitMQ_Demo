namespace Ecommerce.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string routingKey, TEvent @event, CancellationToken cancellationToken = default);
    }
}
