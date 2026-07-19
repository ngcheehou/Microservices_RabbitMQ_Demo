using Microsoft.AspNetCore.Mvc;
using OrderService.API.Application;
using OrderService.API.Application.DTOs;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderAppService _orderAppService;

        public OrderController(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderAppService.GetOrders();
            return Ok(orders);
        }

        [HttpGet("GetOrdersById/{id}")]
        public IActionResult GetOrderById(Guid id)
        {
            var order = _orderAppService.GetOrderById(id);
            if (order == null)
            {
                return NotFound("order not found");
            }
            return Ok(order);
        }

        [HttpGet("GetOrdersByCustomerId/{id}")]
        public async Task<IActionResult> GetOrdersByCustomerId(Guid id)
        {
            var orders = await _orderAppService.GetOrdersByCustomerId(id);
            if (orders.Count == 0)
            {
                return NotFound("order not found for this customer");
            }
            return Ok(orders);
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var result = await _orderAppService.CreateOrder(request);

            return result.Outcome switch
            {
                CreateOrderOutcome.Created => CreatedAtAction(nameof(GetOrderById), new { id = result.Order!.Id }, result.Order),
                CreateOrderOutcome.NoItem => BadRequest(result.Error),
                CreateOrderOutcome.NoCustomer => BadRequest(result.Error),
                CreateOrderOutcome.ProductNotFound => BadRequest(result.Error),
                CreateOrderOutcome.StockReservationFailed => BadRequest(result.Error),
                _ => Problem()
            };
        }

        [HttpPut("UpdateOrder/{id}")]
        public IActionResult UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
        {
            var updatedOrder = _orderAppService.UpdateOrder(id, request);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetOrderById), new { id = updatedOrder.Id }, updatedOrder);
        }

        [HttpDelete("DeleteOrder/{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var deleted = _orderAppService.DeleteOrder(id);
            if (!deleted)
            {
                return NotFound("Order not found");
            }
            return NoContent();
        }
    }
}
