using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using JwtAuthenticationManager.Models.Enum;

namespace JwtAuthenticationManager.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string? FullName { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public Roles Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
