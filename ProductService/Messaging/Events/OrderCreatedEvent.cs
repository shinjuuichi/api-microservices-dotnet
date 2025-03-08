namespace ProductService.Messaging.Events
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }

    public class OrderDetailDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
