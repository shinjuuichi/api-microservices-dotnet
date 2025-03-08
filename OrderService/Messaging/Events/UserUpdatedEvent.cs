namespace OrderService.Messaging.Events
{
    public class UserUpdatedEvent
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }

}
