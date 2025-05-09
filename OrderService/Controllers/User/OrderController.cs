﻿using JwtAuthenticationManager.Data;
using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;
using RabbitMQ.Contracts.Events.Order;
using RabbitMQ.Contracts.Events.Product;
using RabbitMQ.Contracts.Events.User;
using System.Security.Claims;

namespace OrderService.Controllers.User
{
    [Route("api/v1/user/orders")]
    [ApiController]
    public class UserOrderController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetUserRequestEvent> _userRequestClient;
        private readonly IRequestClient<GetProductsRequestEvent> _productRequestClient;

        public UserOrderController(
            OrderDbContext dbContext,
            IPublishEndpoint publishEndpoint,
            IRequestClient<GetUserRequestEvent> userRequestClient,
            IRequestClient<GetProductsRequestEvent> productRequestClient)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
            _userRequestClient = userRequestClient;
            _productRequestClient = productRequestClient;
        }

        private int? GetUserIdFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.Claims.Any())
            {
                return null;
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { Message = "Invalid or expired token" });

            var orders = await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.UserId == userId.Value)
                .ToListAsync();

            if (!orders.Any())
                return NotFound(new { Message = "No orders found for this user" });

            var productIds = orders.SelectMany(o => o.OrderDetails)
                                   .Select(d => d.ProductId)
                                   .Distinct()
                                   .ToList();

            var userResponse = await _userRequestClient.GetResponse<UserInfoResponseEvent>(
                new GetUserRequestEvent { UserId = userId.Value });

            var productResponse = await _productRequestClient.GetResponse<ProductInfoResponseEvent>(
                new GetProductsRequestEvent(productIds));

            var productDictionary = productResponse.Message.Products
                                                     .ToDictionary(p => p.ProductId, p => p.ProductName);
            var userName = userResponse.Message.FullName;

            var result = orders.Select(order => new
            {
                OrderId = order.Id,
                UserName = userName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalPrice,
                OrderDetails = order.OrderDetails.Select(detail => new
                {
                    ProductId = detail.ProductId,
                    ProductName = productDictionary.GetValueOrDefault(detail.ProductId, "Unknown Product"),
                    Quantity = detail.Quantity,
                    Price = detail.ProductPrice
                })
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] UserOrderDTO orderDto, [FromServices] IRequestClient<StockCheckEvent> requestClient)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { Message = "Invalid or expired token" });

            var productIds = orderDto.OrderDetails.Select(od => od.ProductId).ToList();
            var productResponse = await _productRequestClient.GetResponse<ProductInfoResponseEvent>(
                new GetProductsRequestEvent(productIds));

            var productDictionary = productResponse.Message.Products
                .ToDictionary(p => p.ProductId, p => new { p.ProductName, p.UnitPrice });

            var stockCheckEvent = new StockCheckEvent
            {
                OrderItems = orderDto.OrderDetails.Select(od => new OrderItem
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity
                }).ToList()
            };

            var response = await requestClient.GetResponse<StockCheckResultEvent>(stockCheckEvent);
            if (!response.Message.IsAvailable)
            {
                return BadRequest(new { Message = response.Message.FailureReason });
            }

            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.UtcNow,
                OrderDetails = orderDto.OrderDetails.Select(od =>
                {
                    var product = productDictionary.GetValueOrDefault(od.ProductId);
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {od.ProductId} not found.");
                    }

                    return new OrderDetail
                    {
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        ProductPrice = product.UnitPrice
                    };
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                OrderItems = order.OrderDetails.Select(od => new OrderItem
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity
                }).ToList()
            };

            await _publishEndpoint.Publish(orderEvent);

            return CreatedAtAction(nameof(GetUserOrders), new { userId = order.UserId }, order);
        }
    }
}
