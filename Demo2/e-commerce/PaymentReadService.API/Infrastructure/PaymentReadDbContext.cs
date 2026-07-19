using Microsoft.EntityFrameworkCore;
using PaymentReadService.API.Domain;

namespace PaymentReadService.API.Infrastructure
{
    public class PaymentReadDbContext : DbContext
    {
        public PaymentReadDbContext(DbContextOptions<PaymentReadDbContext> options) : base(options)
        {
        }

        public DbSet<CustomerCache> Customers => Set<CustomerCache>();

        public DbSet<ProductCache> Products => Set<ProductCache>();

        public DbSet<OrderCustomerMap> OrderCustomerMap => Set<OrderCustomerMap>();

        public DbSet<OrderItemInfo> OrderItems => Set<OrderItemInfo>();

        public DbSet<PaymentReadModel> Payments => Set<PaymentReadModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerCache>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<ProductCache>().HasKey(p => p.ProductId);
            modelBuilder.Entity<OrderCustomerMap>().HasKey(m => m.OrderId);
            modelBuilder.Entity<OrderItemInfo>().HasKey(i => i.Id);
            modelBuilder.Entity<PaymentReadModel>().HasKey(p => p.PaymentId);
        }
    }
}
