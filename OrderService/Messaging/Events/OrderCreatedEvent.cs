using OrderService.DTOs;

namespace OrderService.Messaging.Events
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
