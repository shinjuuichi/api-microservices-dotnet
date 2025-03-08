using EasyNetQ;
using ProductService.Data;
using ProductService.Messaging.Events;
using ProductService.Messaging.RabbitMQ;

namespace ProductService.Messaging.Handlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IBus _bus;
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(ProductDbContext dbContext, IBus bus, ILogger<OrderCreatedEventHandler> logger)
        {
            _dbContext = dbContext;
            _bus = bus;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedEvent message)
        {
            _logger.LogInformation($"Received OrderCreatedEvent for OrderId: {message.OrderId}");

            bool isStockAvailable = true;
            foreach (var detail in message.OrderDetails)
            {
                var product = await _dbContext.Products.FindAsync(detail.ProductId);
                if (product == null || product.Quantity < detail.Quantity)
                {
                    isStockAvailable = false;
                    break;
                }
            }

            var stockEvent = new OrderStockValidatedEvent
            {
                OrderId = message.OrderId,
                IsStockAvailable = isStockAvailable
            };

            await _bus.PubSub.PublishAsync(stockEvent);

            _logger.LogInformation($"Published OrderStockValidatedEvent for OrderId: {message.OrderId}, StockAvailable: {isStockAvailable}");
        }
    }

}
