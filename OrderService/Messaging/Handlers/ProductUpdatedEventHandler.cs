using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Messaging.Events;
using OrderService.Messaging.RabbitMQ;
namespace OrderService.Messaging.Handlers;


public class ProductUpdatedEventHandler : IEventHandler<ProductUpdatedEvent>
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(OrderDbContext dbContext, ILogger<ProductUpdatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(ProductUpdatedEvent message)
    {
        var orderDetails = await _dbContext.OrderDetails
            .Where(od => od.ProductId == message.Id)
            .ToListAsync();

        if (!orderDetails.Any())
        {
            _logger.LogWarning($"No order details found for ProductId: {message.Id}");
            return;
        }

        foreach (var orderDetail in orderDetails)
        {
            orderDetail.ProductName = message.Name;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Updated product details in OrderService: {message.Id} - {message.Name}");
    }
}
