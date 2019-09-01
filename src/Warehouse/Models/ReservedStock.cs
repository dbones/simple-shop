using System;
using System.Collections.Generic;

namespace Warehouse.Models
{
    public class ReservedStock
    {
        //the OrderId
        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();
    }
}