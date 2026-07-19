namespace PaymentService.API.Infrastructure
{
    public class MockPaymentGateway : IPaymentGateway
    {
        public bool Charge(decimal amount)
        {
            // Stand-in for a real gateway (Stripe/PayPal/etc.) - simulates ~85% of charges succeeding.
            return Random.Shared.Next(1, 101) <= 85;
        }
    }
}
