using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class ShoppingCart
    {
        public required string Id { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public ShoppingCart() { }

        public ShoppingCart(string id, List<CartItem> items)
        {
            Id = id;
            Items = items;
        }
    }
}
