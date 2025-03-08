using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using RabbitMQ.Contracts.Events;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ProductDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedConsumer(ProductDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Console.WriteLine($"Received OrderCreatedEvent for Order ID: {context.Message.OrderId}");

        foreach (var item in context.Message.OrderItems)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            if (product == null || product.Quantity < item.Quantity)
            {
                Console.WriteLine($"Product {item.ProductId} is out of stock!");
                return;
            }

            product.Quantity -= item.Quantity;
        }

        await _dbContext.SaveChangesAsync();
        Console.WriteLine($"Stock updated successfully for Order ID: {context.Message.OrderId}");
    }
}
