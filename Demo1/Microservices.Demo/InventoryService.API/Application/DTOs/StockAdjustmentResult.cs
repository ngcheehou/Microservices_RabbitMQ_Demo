using InventoryService.API.Domain;

namespace InventoryService.API.Application.DTOs
{
    public enum StockAdjustmentOutcome
    {
        Success,
        InvalidQuantity,
        NotFound,
        InsufficientStock
    }

    public class StockAdjustmentResult
    {
        public StockAdjustmentOutcome Outcome { get; }

        public string? Error { get; }

        public Stock? Stock { get; }

        private StockAdjustmentResult(StockAdjustmentOutcome outcome, Stock? stock, string? error)
        {
            Outcome = outcome;
            Stock = stock;
            Error = error;
        }

        public static StockAdjustmentResult Success(Stock stock) => new(StockAdjustmentOutcome.Success, stock, null);

        public static StockAdjustmentResult InvalidQuantity(string error) => new(StockAdjustmentOutcome.InvalidQuantity, null, error);

        public static StockAdjustmentResult NotFound(string error) => new(StockAdjustmentOutcome.NotFound, null, error);

        public static StockAdjustmentResult InsufficientStock(string error) => new(StockAdjustmentOutcome.InsufficientStock, null, error);
    }
}
