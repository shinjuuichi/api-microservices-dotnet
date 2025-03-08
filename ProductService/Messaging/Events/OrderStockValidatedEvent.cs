namespace ProductService.Messaging.Events
{
    public class OrderStockValidatedEvent
    {
        public int OrderId { get; set; }
        public bool IsStockAvailable { get; set; }
    }
}
