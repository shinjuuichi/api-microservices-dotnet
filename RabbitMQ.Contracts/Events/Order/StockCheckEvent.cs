namespace RabbitMQ.Contracts.Events.Order
{
    public class StockCheckEvent
    {
        public List<OrderItem> OrderItems { get; set; }
    }

    public class StockCheckResultEvent
    {
        public bool IsAvailable { get; set; }
        public string FailureReason { get; set; }
    }
}
