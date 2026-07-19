using PaymentService.API.Domain;

namespace PaymentService.API.Application.DTOs
{
    public enum CreatePaymentOutcome
    {
        Created,
        Conflict,
        OrderNotFound,
        InvalidOrder
    }

    public class CreatePaymentResult
    {
        public CreatePaymentOutcome Outcome { get; }

        public string? Error { get; }

        public Payment? Payment { get; }

        private CreatePaymentResult(CreatePaymentOutcome outcome, Payment? payment, string? error)
        {
            Outcome = outcome;
            Payment = payment;
            Error = error;
        }

        public static CreatePaymentResult Created(Payment payment) => new(CreatePaymentOutcome.Created, payment, null);

        public static CreatePaymentResult Conflict(string error) => new(CreatePaymentOutcome.Conflict, null, error);

        public static CreatePaymentResult OrderNotFound(string error) => new(CreatePaymentOutcome.OrderNotFound, null, error);

        public static CreatePaymentResult InvalidOrder(string error) => new(CreatePaymentOutcome.InvalidOrder, null, error);
    }
}
