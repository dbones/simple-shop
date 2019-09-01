using System;
using System.Collections.Generic;

namespace Warehouse.Events
{
    public class StockReserved
    {
        public Dictionary<string,int> Items { get; set; } = new Dictionary<string, int>();
        public string Id { get; set; }
    }
    
    
}