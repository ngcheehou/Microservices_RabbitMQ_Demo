using PaymentService.API.Application.DTOs;
using PaymentService.API.Domain;
using PaymentService.API.ExternalServices.Interfaces;
using PaymentService.API.Infrastructure;
using Ecommerce.Messaging;
using Ecommerce.Messaging.Events;

namespace PaymentService.API.Application
{
    public class PaymentAppService : IPaymentAppService
    {
        private readonly PaymentDbContext _db;
        private readonly IOrderClient _orderClient;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IEventPublisher _eventPublisher;

        public PaymentAppService(
            PaymentDbContext db,
            IOrderClient orderClient,
            IPaymentGateway paymentGateway,
            IEventPublisher eventPublisher)
        {
            _db = db;
            _orderClient = orderClient;
            _paymentGateway = paymentGateway;
            _eventPublisher = eventPublisher;
        }

        public Payment? GetPaymentByOrderId(Guid orderId)
        {
            return _db.Payments.FirstOrDefault(p => p.OrderId == orderId);
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var existingPayment = _db.Payments.FirstOrDefault(p => p.OrderId == request.OrderId);
           
            if (existingPayment != null)
            {
                return CreatePaymentResult.Conflict($"Order {request.OrderId} already has a payment.");
            }

            var order = await _orderClient.GetOrderAsync(request.OrderId);
            if (order == null)
            {
                return CreatePaymentResult.OrderNotFound($"Order {request.OrderId} does not exist.");
            }

            if (order.Items.Count == 0)
            {
                return CreatePaymentResult.InvalidOrder($"Order {request.OrderId} has no items to pay for.");
            }

            var amount = order.Items.Sum(i => i.Price * i.Quantity);
            var charged = _paymentGateway.Charge(amount);

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = request.OrderId,
                Amount = amount,
                Status = charged ? PaymentStatus.Success : PaymentStatus.Rejected,
                CreatedDate = DateTime.UtcNow
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            var statusUpdated = await _orderClient.UpdateOrderStatusAsync(request.OrderId, MapToOrderStatus(payment.Status));
            if (!statusUpdated)
            {
                Console.WriteLine($"Payment {payment.Id} recorded, but failed to update order {request.OrderId} status.");
            }

            try
            {
                await _eventPublisher.PublishAsync(
                    EventRoutingKeys.PaymentCreated,
                    new PaymentCreatedEvent(payment.Id, payment.OrderId, payment.Amount, payment.Status.ToString(), payment.CreatedDate));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish {EventRoutingKeys.PaymentCreated} event: {ex.Message}");
            }

            return CreatePaymentResult.Created(payment);
        }

        private static int MapToOrderStatus(PaymentStatus status) => status switch
        {
            PaymentStatus.Pending => 0,  // OrderStatus.Pending
            PaymentStatus.Success => 1,  // OrderStatus.Success
            PaymentStatus.Rejected => 2, // OrderStatus.Rejected
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }
}
