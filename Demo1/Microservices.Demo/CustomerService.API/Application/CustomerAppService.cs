using Microsoft.EntityFrameworkCore;
using CustomerService.API.Application.DTOs;
using CustomerService.API.Domain;
using CustomerService.API.Infrastructure;

namespace CustomerService.API.Application
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly CustomerDbContext _db;

        public CustomerAppService(CustomerDbContext db)
        {
            _db = db;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _db.Customers.ToListAsync();
        }

        public Customer? GetCustomerById(Guid id)
        {
            return _db.Customers.Find(id);
        }

        public Customer CreateCustomer(CreateCustomerRequest request)
        {
            var newCustomer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                CreatedDate = DateTime.UtcNow
            };

            _db.Customers.Add(newCustomer);
            _db.SaveChanges();

            return newCustomer;
        }

        public Customer? UpdateCustomer(Guid id, UpdateCustomerRequest request)
        {
            var existingCustomer = _db.Customers.Find(id);
            if (existingCustomer == null)
            {
                return null;
            }

            existingCustomer.FirstName = request.FirstName;
            existingCustomer.LastName = request.LastName;
            existingCustomer.Phone = request.Phone;

            _db.SaveChanges();

            return existingCustomer;
        }

        public bool DeleteCustomer(Guid id)
        {
            var customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return false;
            }

            _db.Customers.Remove(customer);
            _db.SaveChanges();

            return true;
        }
    }
}
