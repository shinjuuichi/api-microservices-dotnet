namespace ProductService.Messaging.Events
{
    public class ProductUpdatedEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
    }
}
