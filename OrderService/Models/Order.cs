using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{

    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public decimal TotalPrice => OrderDetails.Sum(od => od.Quantity * od.ProductPrice);
    }

}
