using JwtAuthenticationManager.Data;
using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;
using RabbitMQ.Contracts.Events;
using System.Security.Claims;

namespace OrderService.Controllers.User
{
    [Route("api/v1/user/orders")]
    [ApiController]
    public class UserOrderController(OrderDbContext _dbContext, IPublishEndpoint _publishEndpoint) : ControllerBase
    {
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
                                         .Where(o => o.UserId == userId)
                                         .ToListAsync();

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] UserOrderDTO orderDto, [FromServices] IRequestClient<StockCheckEvent> requestClient)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { Message = "Invalid or expired token" });

            // Send a request to check stock availability
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

            // Only create the order if stock is available
            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.UtcNow,
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    ProductPrice = od.ProductPrice
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
