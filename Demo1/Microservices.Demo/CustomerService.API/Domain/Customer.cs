namespace CustomerService.API.Domain
{
    public class Customer
    {
        public Guid Id { get; set; } 
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public DateTime CreatedDate { get; set; } 
        public bool Status { get; set; } = true;

    }
}
