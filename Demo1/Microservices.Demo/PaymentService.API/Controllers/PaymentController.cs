using Microsoft.AspNetCore.Mvc;
using PaymentService.API.Application;
using PaymentService.API.Application.DTOs;

namespace PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IPaymentAppService _paymentAppService;

        public PaymentController(IPaymentAppService paymentAppService)
        {
            _paymentAppService = paymentAppService;
        }


        [HttpGet("GetPaymentByOrderId/{id}")]
        public IActionResult GetPaymentByOrderId(Guid id)
        {
            var payment = _paymentAppService.GetPaymentByOrderId(id);
            if (payment == null)
            {
                return NotFound("payment not found");
            }
            return Ok(payment);
        }


        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var result = await _paymentAppService.CreatePaymentAsync(request);

            return result.Outcome switch
            {
                CreatePaymentOutcome.Created => CreatedAtAction(nameof(GetPaymentByOrderId), new { id = result.Payment!.OrderId }, result.Payment),
                CreatePaymentOutcome.Conflict => Conflict(result.Error),
                CreatePaymentOutcome.OrderNotFound => BadRequest(result.Error),
                CreatePaymentOutcome.InvalidOrder => BadRequest(result.Error),
                _ => Problem()
            };
        }
    }
}
