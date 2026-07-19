using Microsoft.EntityFrameworkCore;
using OrderService.API.Application.DTOs;
using OrderService.API.Domain;
using OrderService.API.ExternalServices.Interfaces;
using OrderService.API.Infrastructure;
using Ecommerce.Messaging;
using Ecommerce.Messaging.Events;

namespace OrderService.API.Application
{
    public class OrderAppService : IOrderAppService
    {
        private readonly OrderDbContext _db;
        private readonly ICustomerClient _customerClient;
        private readonly IInventoryClient _inventoryClient;
        private readonly IEventPublisher _eventPublisher;

        public OrderAppService(
            OrderDbContext db,
            ICustomerClient customerClient,
            IInventoryClient inventoryClient,
            IEventPublisher eventPublisher)
        {
            _db = db;
            _customerClient = customerClient;
            _inventoryClient = inventoryClient;
            _eventPublisher = eventPublisher;
        }

        public async Task<List<Order>> GetOrders()
        {
            return await _db.Orders.Include(o => o.Items).ToListAsync();
        }

        public Order? GetOrderById(Guid id)
        {
            return _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == id);
        }

        public async Task<List<Order>> GetOrdersByCustomerId(Guid id)
        {
            return await _db.Orders.Include(o => o.Items).Where(o => o.CustomerId == id).ToListAsync();
        }

        public async Task<CreateOrderResult> CreateOrder(CreateOrderRequest request)
        {
            if (request.Items.Count == 0)
            {
                return CreateOrderResult.NoItem("An order must contain at least one item.");
            }

            var customer = await _customerClient.GetCustomerByIdAsync(request.CustomerId);//call customer service to get customer details
            if (customer == null)
            {
                return CreateOrderResult.NoCustomer($"Customer {request.CustomerId} does not exist.");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                Status = OrderStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            foreach (var item in request.Items)
            {
                var product = await _inventoryClient.GetProductAsync(item.ProductId);//call inventory service to get product details
                if (product == null)
                {
                    return CreateOrderResult.ProductNotFound($"Product {item.ProductId} does not exist.");
                }

                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price, // ensure price is from inventory, not from client
                    CreatedDate = DateTime.UtcNow
                });
            }

            var reducedItems = new List<CreateItemRequest>();

            foreach (var item in request.Items)
            {
                var reduced = await _inventoryClient.ReduceStockAsync(item.ProductId, item.Quantity);
                if (!reduced)
                {
                    // Compensate: restore stock already reduced for this order before failing it.
                    foreach (var previouslyReduced in reducedItems)
                    {
                        await _inventoryClient.IncreaseStockAsync(previouslyReduced.ProductId, previouslyReduced.Quantity);
                    }

                    return CreateOrderResult.StockReservationFailed($"Could not reserve stock for product {item.ProductId}.");
                }

                reducedItems.Add(item);
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent(
                order.Id,
                order.CustomerId,
                order.CreatedDate,
                order.Items.Select(i => new OrderCreatedItem(i.ProductId, i.Quantity, i.Price)).ToList());

            try
            {
                await _eventPublisher.PublishAsync(EventRoutingKeys.OrderCreated, orderCreatedEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish {EventRoutingKeys.OrderCreated} event: {ex.Message}");
            }

            return CreateOrderResult.Created(order);
        }

        public Order? UpdateOrder(Guid id, UpdateOrderRequest request)
        {
            var existingOrder = _db.Orders.Find(id);
            if (existingOrder == null)
            {
                return null;
            }

            existingOrder.Status = request.Status;
            _db.SaveChanges();

            return existingOrder;
        }

        public bool DeleteOrder(Guid id)
        {
            var order = _db.Orders.Find(id);
            if (order == null)
            {
                return false;
            }

            _db.Orders.Remove(order);
            _db.SaveChanges();
            return true;
        }
    }
}
