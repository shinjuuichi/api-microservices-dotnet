namespace OrderService.DTOs
{
    public class UserOrderDTO
    {
        public DateTime OrderDate { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
