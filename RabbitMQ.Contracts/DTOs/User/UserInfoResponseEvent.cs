using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Contracts.DTOs.User
{
    public class UserInfoResponseEvent
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
    }
}
