using PaymentService.API.Application.DTOs;
using PaymentService.API.Domain;

namespace PaymentService.API.Application
{
    public interface IPaymentAppService
    {
        Payment? GetPaymentByOrderId(Guid orderId);

        Task<CreatePaymentResult> CreatePaymentAsync(CreatePaymentRequest request);
    }
}
