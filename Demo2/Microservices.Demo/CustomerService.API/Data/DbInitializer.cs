using CustomerService.API.Domain;
using CustomerService.API.Infrastructure;

namespace CustomerService.API.Data
{
    public static class DbInitializer
    {
        public static void Seed(CustomerDbContext db)
        {
            if (db.Customers.Any())
            {
                return;
            }

            var customers = new List<Customer>
            {
                new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "9123-45671", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Phone = "9123-45672", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "Michael", LastName = "Johnson", Email = "michael.johnson@example.com", Phone = "9123-45673", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "Emily", LastName = "Williams", Email = "emily.williams@example.com", Phone = "9123-45674", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "David", LastName = "Brown", Email = "david.brown@example.com", Phone = "9123-45675", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "Sarah", LastName = "Jones", Email = "sarah.jones@example.com", Phone = "9123-45676", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "Daniel", LastName = "Garcia", Email = "daniel.garcia@example.com", Phone = "9123-45677", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "Laura", LastName = "Martinez", Email = "laura.martinez@example.com", Phone = "9123-45678", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "James", LastName = "Davis", Email = "james.davis@example.com", Phone = "9123-45679", CreatedDate = DateTime.UtcNow, Status = true },
                new() { Id = Guid.NewGuid(), FirstName = "Olivia", LastName = "Miller", Email = "olivia.miller@example.com", Phone = "9123-45680", CreatedDate = DateTime.UtcNow, Status = false },
            };

            db.Customers.AddRange(customers);
            db.SaveChanges();
        }
    }
}
