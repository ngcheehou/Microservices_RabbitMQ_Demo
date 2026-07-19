using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentReadService.API.Domain;
using PaymentReadService.API.Infrastructure;

namespace PaymentReadService.API.Controllers
{
    public record PaymentItemResponse(Guid ProductId, string? ProductName, int Quantity, decimal Price);

    public record PaymentDetailResponse(
        Guid PaymentId,
        Guid OrderId,
        Guid? CustomerId,
        string? CustomerName,
        decimal Amount,
        string Status,
        DateTime CreatedDate,
        List<PaymentItemResponse> Items);

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentReadController : ControllerBase
    {
        private readonly PaymentReadDbContext _db;

        public PaymentReadController(PaymentReadDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetPayments")]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _db.Payments.ToListAsync();
            var orderIds = payments.Select(p => p.OrderId).ToList();
            var itemsByOrder = await LoadItemsByOrderAsync(orderIds);

            var result = new List<PaymentDetailResponse>();
            foreach (var payment in payments)
            {
                var (customerId, customerName) = await ResolveCustomerAsync(payment.OrderId, payment.CustomerId, payment.CustomerName);
                itemsByOrder.TryGetValue(payment.OrderId, out var items);
                result.Add(ToResponse(payment, customerId, customerName, items ?? new List<OrderItemInfo>()));
            }

            return Ok(result);
        }

        [HttpGet("GetPaymentByOrderId/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(Guid orderId)
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment == null)
            {
                return NotFound("payment not found");
            }

            var (customerId, customerName) = await ResolveCustomerAsync(payment.OrderId, payment.CustomerId, payment.CustomerName);
            var itemsByOrder = await LoadItemsByOrderAsync(new List<Guid> { orderId });
            itemsByOrder.TryGetValue(orderId, out var items);

            return Ok(ToResponse(payment, customerId, customerName, items ?? new List<OrderItemInfo>()));
        }

        // CustomerName is only a snapshot taken when the payment.created event was handled,
        // which is null if the order->customer mapping or the customer cache wasn't populated
        // yet at that point. Re-resolve from the current caches on every read so a payment
        // self-heals once that data does show up, without needing to replay events.
        private async Task<(Guid? CustomerId, string? CustomerName)> ResolveCustomerAsync(
            Guid orderId, Guid? knownCustomerId, string? knownCustomerName)
        {
            if (knownCustomerId != null && knownCustomerName != null)
            {
                return (knownCustomerId, knownCustomerName);
            }

            var customerId = knownCustomerId;
            if (customerId == null)
            {
                var map = await _db.OrderCustomerMap.FindAsync(orderId);
                customerId = map?.CustomerId;
            }

            if (customerId == null)
            {
                return (null, null);
            }

            var customer = await _db.Customers.FindAsync(customerId.Value);
            var customerName = customer == null ? null : $"{customer.FirstName} {customer.LastName}";
            return (customerId, customerName);
        }

        // Same self-healing idea as CustomerName above, but for each item's product name.
        private async Task<Dictionary<Guid, List<OrderItemInfo>>> LoadItemsByOrderAsync(List<Guid> orderIds)
        {
            var items = await _db.OrderItems.Where(i => orderIds.Contains(i.OrderId)).ToListAsync();

            var missingProductIds = items.Where(i => i.ProductName == null).Select(i => i.ProductId).Distinct().ToList();
            if (missingProductIds.Count > 0)
            {
                var names = await _db.Products
                    .Where(p => missingProductIds.Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.ProductId, p => p.Name);

                foreach (var item in items)
                {
                    if (item.ProductName == null && names.TryGetValue(item.ProductId, out var name))
                    {
                        item.ProductName = name;
                    }
                }
            }

            return items.GroupBy(i => i.OrderId).ToDictionary(g => g.Key, g => g.ToList());
        }

        private static PaymentDetailResponse ToResponse(
            PaymentReadModel payment, Guid? customerId, string? customerName, List<OrderItemInfo> items)
        {
            return new PaymentDetailResponse(
                payment.PaymentId,
                payment.OrderId,
                customerId,
                customerName,
                payment.Amount,
                payment.Status,
                payment.CreatedDate,
                items.Select(i => new PaymentItemResponse(i.ProductId, i.ProductName, i.Quantity, i.Price)).ToList());
        }
    }
}
