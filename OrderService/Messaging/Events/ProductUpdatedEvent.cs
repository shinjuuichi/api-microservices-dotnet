using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderService.Data;
namespace OrderService.Messaging.Events
{
    public class ProductUpdatedEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
