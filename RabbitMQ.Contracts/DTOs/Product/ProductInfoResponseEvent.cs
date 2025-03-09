using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Contracts.DTOs.Product
{

    public class ProductInfoResponseEvent
    {
        public List<ProductInfo> Products { get; set; }
    }

    public class ProductInfo
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
