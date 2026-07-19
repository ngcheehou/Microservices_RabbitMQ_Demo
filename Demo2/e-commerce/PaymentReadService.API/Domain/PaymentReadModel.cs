namespace PaymentReadService.API.Domain
{
    public class PaymentReadModel
    {
        public Guid PaymentId { get; set; }

        public Guid OrderId { get; set; }

        public Guid? CustomerId { get; set; }

        // Snapshot of "FirstName LastName" at the time the payment.created event
        // was handled; null if the customer/order mapping wasn't cached yet.
        public string? CustomerName { get; set; }
        
        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}
