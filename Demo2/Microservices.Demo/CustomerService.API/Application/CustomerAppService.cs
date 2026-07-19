using Microsoft.EntityFrameworkCore;
using CustomerService.API.Application.DTOs;
using CustomerService.API.Domain;
using CustomerService.API.Infrastructure;
using Ecommerce.Messaging;
using Ecommerce.Messaging.Events;

namespace CustomerService.API.Application
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly CustomerDbContext _db;
        private readonly IEventPublisher _eventPublisher;

        public CustomerAppService(CustomerDbContext db, IEventPublisher eventPublisher)
        {
            _db = db;
            _eventPublisher = eventPublisher;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _db.Customers.ToListAsync();
        }

        public Customer? GetCustomerById(Guid id)
        {
            return _db.Customers.Find(id);
        }

        public async Task<Customer> CreateCustomer(CreateCustomerRequest request)
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
            _db.SaveChanges();//

            await PublishSafeAsync(EventRoutingKeys.CustomerCreated,
                new CustomerCreatedEvent(newCustomer.Id, newCustomer.FirstName, newCustomer.LastName));

            return newCustomer;
        }

        public async Task<Customer?> UpdateCustomer(Guid id, UpdateCustomerRequest request)
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

            await PublishSafeAsync(EventRoutingKeys.CustomerUpdated,
                new CustomerUpdatedEvent(existingCustomer.Id, existingCustomer.FirstName, existingCustomer.LastName));

            return existingCustomer;
        }

        private async Task PublishSafeAsync<TEvent>(string routingKey, TEvent @event)
        {
            try
            {
                await _eventPublisher.PublishAsync(routingKey, @event);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish {routingKey} event: {ex.Message}");
            }
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
