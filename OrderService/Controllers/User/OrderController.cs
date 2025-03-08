using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Messaging.Events;
using OrderService.Models;
using System.Security.Claims;

namespace OrderService.Controllers.User
{
    [Route("api/v1/user/orders")]
    [ApiController]
    public class UserOrderController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;

        public UserOrderController(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
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
                                         .Where(o => o.UserId == userId)
                                         .ToListAsync();

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] UserOrderDTO orderDto, [FromServices] IBus bus)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { Message = "Invalid token" });

            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.UtcNow,
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    ProductId = od.ProductId,
                    ProductName = "Pending",
                    ProductPrice = od.ProductPrice,
                    Quantity = od.Quantity
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    ProductPrice = od.ProductPrice
                }).ToList()
            };

            await bus.PubSub.PublishAsync(orderEvent);

            return Accepted(new { Message = "Order placed successfully, awaiting stock validation.", OrderId = order.Id });
        }
    }
}
