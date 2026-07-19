namespace PaymentReadService.API.Domain
{
    // Local, denormalized copy of the customer info this service needs,
    // kept in sync via CustomerCreated/CustomerUpdated events from CustomerService.
    public class CustomerCache
    {
        public Guid CustomerId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime UpdatedDate { get; set; }
    }
}
