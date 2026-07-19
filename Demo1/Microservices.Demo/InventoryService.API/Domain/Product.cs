namespace InventoryService.API.Domain
{
    public class Product
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
