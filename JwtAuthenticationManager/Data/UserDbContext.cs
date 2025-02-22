using Microsoft.EntityFrameworkCore;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager.Models.Enum;
using SharedLibrary;

namespace JwtAuthenticationManager.Data
{
    public class UserDbContext() : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1333;Database=UserDb;User Id=sa;Password=Shinjuuichidesu@11;TrustServerCertificate=True");
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "John Doe",
                Role = Roles.Admin,
                Email = "admin@example.com",
                Password = CryptoUtil.EncryptPassword("Admin@123")
            },
            new User
            {
                Id = 2,
                FullName = "Jane Doe",
                Role = Roles.User,
                Email = "user@example.com",
                Password = CryptoUtil.EncryptPassword("User@123")
            }
        );
        }
    }
}
