namespace Ecommerce.Messaging.Events
{
    public sealed record CustomerCreatedEvent(Guid CustomerId, string FirstName, string LastName);

    public sealed record CustomerUpdatedEvent(Guid CustomerId, string FirstName, string LastName);
}
