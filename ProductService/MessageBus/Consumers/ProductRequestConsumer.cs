﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using RabbitMQ.Contracts.Events.Product;

public class ProductRequestConsumer : IConsumer<GetProductsRequestEvent>
{
    private readonly ProductDbContext _dbContext;

    public ProductRequestConsumer(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<GetProductsRequestEvent> context)
    {
        var products = await _dbContext.Products
            .Where(p => context.Message.ProductIds.Contains(p.Id))
            .ToListAsync();

        var response = new ProductInfoResponseEvent
        {
            Products = products.Select(p => new ProductInfo
            {
                ProductId = p.Id,
                ProductName = p.Name,
                UnitPrice = p.Price
            }).ToList()
        };

        await context.RespondAsync(response);
    }
}
