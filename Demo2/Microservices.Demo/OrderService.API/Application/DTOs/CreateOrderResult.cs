using OrderService.API.Domain;

namespace OrderService.API.Application.DTOs
{
    public enum CreateOrderOutcome
    {
        Created,
        NoItem,
        NoCustomer,
        ProductNotFound,
        StockReservationFailed
    }

    public class CreateOrderResult
    {
        public CreateOrderOutcome Outcome { get; }

        public string? Error { get; }

        public Order? Order { get; }

        private CreateOrderResult(CreateOrderOutcome outcome, Order? order, string? error)
        {
            Outcome = outcome;
            Order = order;
            Error = error;
        }

        public static CreateOrderResult Created(Order order) => new(CreateOrderOutcome.Created, order, null);

        public static CreateOrderResult NoItem(string error) => new(CreateOrderOutcome.NoItem, null, error);

        public static CreateOrderResult NoCustomer(string error) => new(CreateOrderOutcome.NoCustomer, null, error);

        public static CreateOrderResult ProductNotFound(string error) => new(CreateOrderOutcome.ProductNotFound, null, error);

        public static CreateOrderResult StockReservationFailed(string error) => new(CreateOrderOutcome.StockReservationFailed, null, error);
    }
}
