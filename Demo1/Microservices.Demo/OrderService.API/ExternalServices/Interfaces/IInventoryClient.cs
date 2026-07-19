using OrderService.API.ExternalServices.Models;

namespace OrderService.API.ExternalServices.Interfaces
{
    public interface IInventoryClient
    {
        Task<ProductSummary?> GetProductAsync(Guid productId);

        Task<bool> ReduceStockAsync(Guid productId, int quantity);

        Task<bool> IncreaseStockAsync(Guid productId, int quantity);
    }
}
