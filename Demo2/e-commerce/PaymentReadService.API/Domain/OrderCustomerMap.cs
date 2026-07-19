namespace PaymentReadService.API.Domain
{
    // Built from OrderCreated events; lets a payment.created event (which only
    // carries OrderId) be traced back to the customer who placed the order.
    public class OrderCustomerMap
    {
        public Guid OrderId { get; set; }

        public Guid CustomerId { get; set; }
    }
}
