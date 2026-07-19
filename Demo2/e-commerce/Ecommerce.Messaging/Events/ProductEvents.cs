namespace Ecommerce.Messaging.Events
{
    public sealed record ProductCreatedEvent(Guid ProductId, string Name, decimal Price);

    public sealed record ProductUpdatedEvent(Guid ProductId, string Name, decimal Price);
}
