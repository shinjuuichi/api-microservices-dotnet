using Microsoft.EntityFrameworkCore;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager.Models.Enum;
using SharedLibrary;

namespace JwtAuthenticationManager.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
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