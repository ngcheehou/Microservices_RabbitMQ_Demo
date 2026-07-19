using Microsoft.EntityFrameworkCore;
using InventoryService.API.Application.DTOs;
using InventoryService.API.Domain;
using InventoryService.API.Infrastructure;

namespace InventoryService.API.Application
{
    public class ProductAppService : IProductAppService
    {
        private readonly InventoryDbContext _db;

        public ProductAppService(InventoryDbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _db.Products.ToListAsync();
        }

        public Product? GetProductById(Guid id)
        {
            return _db.Products.Find(id);
        }

        public Product CreateProduct(CreateProductRequest request)
        {
            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                CreatedDate = DateTime.UtcNow
            };

            var stock = new Stock
            {
                Id = Guid.NewGuid(),
                ProductId = newProduct.Id,
                Quantity = request.Quantity,
                CreatedDate = DateTime.UtcNow
            };

            _db.Products.Add(newProduct);
            _db.Stocks.Add(stock);
            _db.SaveChanges();

            return newProduct;
        }

        public Product? UpdateProduct(Guid id, UpdateProductRequest request)
        {
            var existingProduct = _db.Products.Find(id);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = request.Name;
            existingProduct.Price = request.Price;

            _db.SaveChanges();

            return existingProduct;
        }

        public StockAdjustmentResult ReduceStock(Guid productId, AdjustStockRequest request)
        {
            if (request.Quantity <= 0)
            {
                return StockAdjustmentResult.InvalidQuantity("Quantity must be greater than zero.");
            }

            var stock = _db.Stocks.FirstOrDefault(s => s.ProductId == productId);
            if (stock == null)
            {
                return StockAdjustmentResult.NotFound($"Stock for product {productId} not found.");
            }

            if (stock.Quantity < request.Quantity)
            {
                return StockAdjustmentResult.InsufficientStock(
                    $"Insufficient stock for product {productId}. Available: {stock.Quantity}, requested: {request.Quantity}.");
            }

            stock.Quantity -= request.Quantity;
            _db.SaveChanges();

            return StockAdjustmentResult.Success(stock);
        }

        public StockAdjustmentResult IncreaseStock(Guid productId, AdjustStockRequest request)
        {
            if (request.Quantity <= 0)
            {
                return StockAdjustmentResult.InvalidQuantity("Quantity must be greater than zero.");
            }

            var stock = _db.Stocks.FirstOrDefault(s => s.ProductId == productId);
            if (stock == null)
            {
                return StockAdjustmentResult.NotFound($"Stock for product {productId} not found.");
            }

            stock.Quantity += request.Quantity;
            _db.SaveChanges();

            return StockAdjustmentResult.Success(stock);
        }

        public bool DeleteProduct(Guid id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return false;
            }

            var stock = _db.Stocks.FirstOrDefault(s => s.ProductId == id);
            if (stock != null)
            {
                _db.Stocks.Remove(stock);
            }

            _db.Products.Remove(product);
            _db.SaveChanges();

            return true;
        }
    }
}
