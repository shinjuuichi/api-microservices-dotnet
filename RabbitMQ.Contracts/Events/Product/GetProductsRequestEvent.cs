using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Contracts.Events.Product
{
    public class GetProductsRequestEvent
    {
        public List<int> ProductIds { get; set; }

        public GetProductsRequestEvent(List<int> productIds)
        {
            ProductIds = productIds;
        }

    }
}
