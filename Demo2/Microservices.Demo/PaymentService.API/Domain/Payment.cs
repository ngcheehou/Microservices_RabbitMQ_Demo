namespace PaymentService.API.Domain
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
