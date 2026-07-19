using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentService.API.Domain;

namespace PaymentService.API.Infrastructure
{
    public class PaymentDbContext : DbContext
    {


        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {

        }
        public DbSet<Payment> Payments { get; set; }

    }
}
