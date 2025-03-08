using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderService.Models;
using SharedLibrary;

namespace OrderService.Data
{
    public class OrderDbContext(IConfiguration _configuration) : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("OrderDb");
            optionsBuilder.UseSqlServer(connectionString);
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
