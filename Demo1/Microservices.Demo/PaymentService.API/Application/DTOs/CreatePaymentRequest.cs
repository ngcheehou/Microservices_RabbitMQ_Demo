namespace PaymentService.API.Application.DTOs
{
    public class CreatePaymentRequest
    {
        public required Guid OrderId { get; set; }
    }
}
