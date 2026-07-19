using InventoryService.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.API.Infrastructure
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
                
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Stock> Stocks { get; set; }
    }
}
