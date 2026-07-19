namespace CustomerService.API.Application.DTOs
{
    public class UpdateCustomerRequest
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Phone { get; set; }
    }
}
