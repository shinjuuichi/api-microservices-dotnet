using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using RabbitMQ.Contracts.Events.Order;

public class StockCheckConsumer : IConsumer<StockCheckEvent>
{
    private readonly ProductDbContext _dbContext;

    public StockCheckConsumer(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<StockCheckEvent> context)
    {
        foreach (var item in context.Message.OrderItems)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            if (product == null || product.Quantity < item.Quantity)
            {
                await context.RespondAsync(new StockCheckResultEvent
                {
                    IsAvailable = false,
                    FailureReason = product != null
                        ? $"Product '{product.Name}' is out of stock."
                        : $"Product with ID {item.ProductId} does not exist."
                });
                return;
            }
        }

        await context.RespondAsync(new StockCheckResultEvent { IsAvailable = true });
    }
}
