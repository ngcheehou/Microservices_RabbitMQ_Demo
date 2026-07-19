namespace Ecommerce.Messaging.Events
{
    public sealed record OrderCreatedItem(Guid ProductId, int Quantity, decimal Price);

    public sealed record OrderCreatedEvent(
        Guid OrderId,
        Guid CustomerId,
        DateTime CreatedDate,
        List<OrderCreatedItem> Items);
}
