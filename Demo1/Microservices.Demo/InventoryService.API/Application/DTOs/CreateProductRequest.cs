namespace InventoryService.API.Application.DTOs
{
    public class CreateProductRequest
    {
        public required string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
