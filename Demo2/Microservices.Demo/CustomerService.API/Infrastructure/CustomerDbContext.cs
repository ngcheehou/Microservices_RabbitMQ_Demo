using CustomerService.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.API.Infrastructure
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(
            DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }


        public DbSet<Customer> Customers { get; set; }
    }
}
