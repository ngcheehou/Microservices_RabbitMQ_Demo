using CustomerService.API.Application;
using CustomerService.API.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerAppService _customerAppService;

        public CustomerController(ICustomerAppService customerAppService)
        {
            _customerAppService = customerAppService;
        }

        [HttpGet("GetCustomers")]
        public async Task<IActionResult> Get()
        {
            var customers = await _customerAppService.GetCustomers();
            return Ok(customers);
        }

        [HttpGet("GetCustomerById/{id}")]
        public IActionResult GetCustomerById(Guid id)
        {
            var customer = _customerAppService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound("customer not found");
            }
            return Ok(customer);
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var newCustomer = await _customerAppService.CreateCustomer(request);
            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.Id }, newCustomer);
        }

        [HttpPut("UpdateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            var existingCustomer = await _customerAppService.UpdateCustomer(id, request);
            if (existingCustomer == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetCustomerById), new { id = existingCustomer.Id }, existingCustomer);
        }

        [HttpDelete("DeleteCustomer/{id}")]
        public IActionResult DeleteCustomer(Guid id)
        {
            var deleted = _customerAppService.DeleteCustomer(id);
            if (!deleted)
            {
                return NotFound("Customer not found");
            }
            return NoContent();
        }
    }
}
