namespace Ecommerce.Messaging
{
    public static class EventRoutingKeys
    {
        public const string CustomerCreated = "customer.created";
        public const string CustomerUpdated = "customer.updated";
        public const string ProductCreated = "product.created";
        public const string ProductUpdated = "product.updated";
        public const string OrderCreated = "order.created";
        public const string PaymentCreated = "payment.created";
    }
}
