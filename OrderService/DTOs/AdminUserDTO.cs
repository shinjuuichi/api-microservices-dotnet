namespace OrderService.DTOs
{
    public class AdminOrderDTO
    {
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
