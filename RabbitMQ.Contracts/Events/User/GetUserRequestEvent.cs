using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Contracts.Events.User
{
    public class GetUserRequestEvent
    {
        public int UserId { get; set; }
    }
}
