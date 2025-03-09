using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Contracts.DTOs.Product
{
    public class GetProductsRequest
    {
        public List<int> ProductIds { get; set; }

        public GetProductsRequest(List<int> productIds)
        {
            ProductIds = productIds;
        }

    }
}
