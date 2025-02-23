using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using SharedLibrary;

namespace OrderService.Data
{
    public class OrderDbContext() : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1333;Database=OrderDb;User Id=sa;Password=Shinjuuichidesu@11;TrustServerCertificate=True");
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
