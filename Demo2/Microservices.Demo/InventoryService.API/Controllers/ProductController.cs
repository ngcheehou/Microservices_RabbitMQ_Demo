using InventoryService.API.Application;
using InventoryService.API.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductAppService _productAppService;

        public ProductController(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }
         
        [HttpGet("GetProducts")]
        public async Task<IActionResult> Get()
        {
            var products = await _productAppService.GetProducts();
            return Ok(products);
        }

        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(Guid id)
        {
            var product = _productAppService.GetProductById(id);
            if (product == null)
            {
                return NotFound("product not found");
            }
            return Ok(product);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var newProduct = await _productAppService.CreateProduct(request);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
        {
            var existingProduct = await _productAppService.UpdateProduct(id, request);
            if (existingProduct == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetProductById), new { id = existingProduct.Id }, existingProduct);
        }

        [HttpPut("ReduceStock/{id}")]
        public IActionResult ReduceStock(Guid id, [FromBody] AdjustStockRequest request)
        {
            var result = _productAppService.ReduceStock(id, request);

            return result.Outcome switch
            {
                StockAdjustmentOutcome.Success => Ok(result.Stock),
                StockAdjustmentOutcome.InvalidQuantity => BadRequest(result.Error),
                StockAdjustmentOutcome.NotFound => NotFound(result.Error),
                StockAdjustmentOutcome.InsufficientStock => BadRequest(result.Error),
                _ => Problem()
            };
        }

        [HttpPut("IncreaseStock/{id}")]
        public IActionResult IncreaseStock(Guid id, [FromBody] AdjustStockRequest request)
        {
            var result = _productAppService.IncreaseStock(id, request);

            return result.Outcome switch
            {
                StockAdjustmentOutcome.Success => Ok(result.Stock),
                StockAdjustmentOutcome.InvalidQuantity => BadRequest(result.Error),
                StockAdjustmentOutcome.NotFound => NotFound(result.Error),
                _ => Problem()
            };
        }

        [HttpDelete("DeleteProduct/{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            var deleted = _productAppService.DeleteProduct(id);
            if (!deleted)
            {
                return NotFound("Product not found");
            }
            return NoContent();
        }
    }
}
