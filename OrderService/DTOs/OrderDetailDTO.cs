namespace OrderService.DTOs
{
    public class OrderDetailDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
