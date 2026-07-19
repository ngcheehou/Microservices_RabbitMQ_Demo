using InventoryService.API.Domain;
using InventoryService.API.Infrastructure;

namespace InventoryService.API.Data
{
    public static class DbInitializer
    {
        public static void Seed(InventoryDbContext db)
        {
            if (db.Products.Any())
            {
                return;
            }

            var seedData = new List<(string Name, decimal Price, int Quantity)>
            {
                ("Wireless Mouse", 19.99m, 150),
                ("Mechanical Keyboard", 59.99m, 80),
                ("27-inch Monitor", 189.99m, 40),
                ("USB-C Hub", 24.99m, 120),
                ("Laptop Stand", 34.99m, 90),
                ("Webcam 1080p", 29.99m, 75),
                ("Noise Cancelling Headphones", 129.99m, 60),
                ("Bluetooth Speaker", 39.99m, 100),
                ("External SSD 1TB", 89.99m, 55),
                ("Portable Power Bank", 24.99m, 200),
                ("Smartphone Case", 14.99m, 300),
                ("Screen Protector", 7.99m, 400),
                ("HDMI Cable 2m", 9.99m, 250),
                ("Ethernet Cable 5m", 8.99m, 180),
                ("Wireless Charger Pad", 19.99m, 130),
                ("Gaming Mouse Pad", 12.99m, 220),
                ("Office Chair", 149.99m, 25),
                ("Standing Desk", 349.99m, 15),
                ("Desk Lamp LED", 22.99m, 95),
                ("Water Bottle 1L", 11.99m, 260),
                ("Backpack 25L", 44.99m, 70),
                ("Notebook A5", 3.99m, 500),
                ("Ballpoint Pen (Pack of 10)", 5.99m, 450),
                ("Sticky Notes", 2.99m, 600),
                ("Whiteboard Marker Set", 9.99m, 300),
                ("Coffee Mug", 8.99m, 350),
                ("Desk Organizer", 15.99m, 110),
                ("Cable Management Box", 13.99m, 140),
                ("Surge Protector", 21.99m, 85),
                ("Router Wi-Fi 6", 79.99m, 50),
            };

            var products = new List<Product>();
            var stocks = new List<Stock>();

            foreach (var item in seedData)
            {
                var productId = Guid.NewGuid();

                products.Add(new Product
                {
                    Id = productId,
                    Name = item.Name,
                    Price = item.Price,
                    CreatedDate = DateTime.UtcNow
                });

                stocks.Add(new Stock
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = item.Quantity,
                    CreatedDate = DateTime.UtcNow
                });
            }

            db.Products.AddRange(products);
            db.Stocks.AddRange(stocks);
            db.SaveChanges();
        }
    }
}
