using InventoryService.API.Application.DTOs;
using InventoryService.API.Domain;

namespace InventoryService.API.Application
{
    public interface IProductAppService
    {
        Task<List<Product>> GetProducts();

        Product? GetProductById(Guid id);

        Product CreateProduct(CreateProductRequest request);

        Product? UpdateProduct(Guid id, UpdateProductRequest request);

        StockAdjustmentResult ReduceStock(Guid productId, AdjustStockRequest request);

        StockAdjustmentResult IncreaseStock(Guid productId, AdjustStockRequest request);

        bool DeleteProduct(Guid id);
    }
}
