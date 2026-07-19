namespace PaymentReadService.API.Domain
{
    // Local, denormalized copy of the product info this service needs,
    // kept in sync via ProductCreated/ProductUpdated events from InventoryService.
    public class ProductCache
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
