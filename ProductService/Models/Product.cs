using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required,]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        public Category Category { get; set; }
    }
}
