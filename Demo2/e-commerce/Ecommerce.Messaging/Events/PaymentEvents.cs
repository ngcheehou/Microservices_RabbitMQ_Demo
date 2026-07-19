namespace Ecommerce.Messaging.Events
{
    public sealed record PaymentCreatedEvent(
        Guid PaymentId,
        Guid OrderId,
        decimal Amount,
        string Status,
        DateTime CreatedDate);
}
