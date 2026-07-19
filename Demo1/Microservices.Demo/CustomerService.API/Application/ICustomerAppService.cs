using CustomerService.API.Application.DTOs;
using CustomerService.API.Domain;

namespace CustomerService.API.Application
{
    public interface ICustomerAppService
    {
        Task<List<Customer>> GetCustomers();

        Customer? GetCustomerById(Guid id);

        Customer CreateCustomer(CreateCustomerRequest request);

        Customer? UpdateCustomer(Guid id, UpdateCustomerRequest request);

        bool DeleteCustomer(Guid id);
    }
}
