using System;
using System.Collections.Generic;

namespace Basket.Models
{
    public class Basket
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}