namespace InventoryService.API.Application.DTOs
{
    public class UpdateProductRequest
    {
        public required string Name { get; set; }

        public decimal Price { get; set; }
    }
}
