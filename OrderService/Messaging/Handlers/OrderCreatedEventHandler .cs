using OrderService.Data;
using OrderService.Messaging.Events;
using OrderService.Messaging.RabbitMQ;

namespace OrderService.Messaging.Handlers
{
    public class OrderStockValidatedEventHandler : IEventHandler<OrderStockValidatedEvent>
    {
        private readonly OrderDbContext _dbContext;
        private readonly ILogger<OrderStockValidatedEventHandler> _logger;

        public OrderStockValidatedEventHandler(OrderDbContext dbContext, ILogger<OrderStockValidatedEventHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(OrderStockValidatedEvent message)
        {
            _logger.LogInformation($"Received OrderStockValidatedEvent for OrderId: {message.OrderId}");

            var order = await _dbContext.Orders.FindAsync(message.OrderId);
            if (order == null)
            {
                _logger.LogWarning($"Order {message.OrderId} not found.");
                return;
            }

            if (message.IsStockAvailable)
            {
                _logger.LogInformation($"Stock available. Order {message.OrderId} is confirmed.");
            }
            else
            {
                _logger.LogWarning($"Stock unavailable. Cancelling Order {message.OrderId}.");
                _dbContext.Orders.Remove(order);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
