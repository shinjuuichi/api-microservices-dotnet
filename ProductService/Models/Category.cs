using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
    }
}
