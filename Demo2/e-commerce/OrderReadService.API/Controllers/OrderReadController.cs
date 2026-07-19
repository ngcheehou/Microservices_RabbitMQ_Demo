using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderReadService.API.Domain;
using OrderReadService.API.Infrastructure;

namespace OrderReadService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderReadController : ControllerBase
    {
        private readonly OrderReadDbContext _db;

        public OrderReadController(OrderReadDbContext db)
        {
            _db = db;
        }
        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _db.Orders.Include(o => o.Items).ToListAsync();
            await ResolveProductNamesAsync(orders.SelectMany(o => o.Items));
            return Ok(orders);
        }

        [HttpGet("GetOrderById/{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("order not found");
            }

            await ResolveProductNamesAsync(order.Items);
            return Ok(order);
        }

        // The item's ProductName is only a snapshot taken when the order.created event was
        // handled, which is null if ProductCache hadn't been populated yet at that point
        // (e.g. the product.created event arrived after, or was published before this
        // service's queue existed). Re-resolve from the current cache on every read so an
        // order self-heals once the product info does show up, without needing to replay events.
        private async Task ResolveProductNamesAsync(IEnumerable<OrderItemReadModel> items)
        {
            var missingProductIds = items
                .Where(i => i.ProductName == null)
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            if (missingProductIds.Count == 0)
            {
                return;
            }

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
    }
}
