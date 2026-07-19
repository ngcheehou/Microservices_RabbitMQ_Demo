using OrderService.API.ExternalServices.Models;

namespace OrderService.API.ExternalServices.Interfaces
{
    public interface ICustomerClient
    {
        Task<CustomerSummary?> GetCustomerByIdAsync(Guid id);
    }
}
