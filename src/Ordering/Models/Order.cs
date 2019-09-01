using System;
using System.Collections.Generic;

namespace Ordering.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal SubTotal { get; set; }
        public bool IsOrderReady { get; set; } = false;


        public bool IsComplete { get; set; }
    }
}