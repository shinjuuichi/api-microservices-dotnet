using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Messaging.Events;
using OrderService.Messaging.RabbitMQ;

namespace OrderService.Messaging.Handlers;
public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<UserUpdatedEventHandler> _logger;

    public UserUpdatedEventHandler(OrderDbContext dbContext, ILogger<UserUpdatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(UserUpdatedEvent message)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.UserId == message.UserId)
            .ToListAsync();

        foreach (var order in orders)
        {
            order.UserName = message.Name;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Updated user name in OrderService: {message.UserId} - {message.Name}");
    }
}
