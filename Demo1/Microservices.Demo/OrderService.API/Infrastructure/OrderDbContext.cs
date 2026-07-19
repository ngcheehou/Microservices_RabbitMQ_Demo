using Microsoft.EntityFrameworkCore;
using OrderService.API.Domain;
using System.Collections;


namespace OrderService.API.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> Items { get; set; }
    }
}
