using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using System.Threading.Tasks;
using System.Linq;
using OrderService.DTOs;

namespace OrderService.Controllers.Admin
{
    [Route("api/v1/admin/orders")]
    [ApiController]
    public class OrderManagementController(OrderDbContext _dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _dbContext.Orders.Include(o => o.OrderDetails).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _dbContext.Orders.Include(o => o.OrderDetails)
                                               .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminOrderDTO orderDto)
        {
            var order = new Order
            {
                UserId = orderDto.UserId,
                OrderDate = orderDto.OrderDate,
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    ProductPrice = od.ProductPrice
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminOrderDTO orderDto)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            order.UserId = orderDto.UserId;
            order.OrderDate = orderDto.OrderDate;
            order.OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
            {
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                ProductPrice = od.ProductPrice
            }).ToList();

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}