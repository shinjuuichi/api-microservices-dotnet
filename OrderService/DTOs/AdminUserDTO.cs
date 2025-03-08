namespace OrderService.DTOs
{
    public class AdminOrderDTO
    {
        public int UserId { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
