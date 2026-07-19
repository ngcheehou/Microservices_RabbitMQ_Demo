namespace PaymentService.API.Infrastructure
{
    public interface IPaymentGateway
    {
        bool Charge(decimal amount);
    }
}
