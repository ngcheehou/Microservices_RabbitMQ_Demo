using Microsoft.EntityFrameworkCore;
using OrderReadService.API.Domain;

namespace OrderReadService.API.Infrastructure
{
    public class OrderReadDbContext : DbContext
    {
        public OrderReadDbContext(DbContextOptions<OrderReadDbContext> options) : base(options)
        {
        }

        public DbSet<ProductCache> Products => Set<ProductCache>();

        public DbSet<OrderReadModel> Orders => Set<OrderReadModel>();

        public DbSet<OrderItemReadModel> OrderItems => Set<OrderItemReadModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCache>().HasKey(p => p.ProductId);

            modelBuilder.Entity<OrderReadModel>().HasKey(o => o.OrderId);
            modelBuilder.Entity<OrderReadModel>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(i => i.OrderId);

            modelBuilder.Entity<OrderItemReadModel>().HasKey(i => i.Id);
        }
    }
}
