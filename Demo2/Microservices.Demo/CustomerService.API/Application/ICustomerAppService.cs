using CustomerService.API.Application.DTOs;
using CustomerService.API.Domain;

namespace CustomerService.API.Application
{
    public interface ICustomerAppService
    {
        Task<List<Customer>> GetCustomers();

        Customer? GetCustomerById(Guid id);

        Task<Customer> CreateCustomer(CreateCustomerRequest request);

        Task<Customer?> UpdateCustomer(Guid id, UpdateCustomerRequest request);

        bool DeleteCustomer(Guid id);
    }
}
