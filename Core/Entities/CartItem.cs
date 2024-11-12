using System;

namespace Core.Entities;

public class CartItem
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public required string PictureUrl { get; set; }
    public required string Brand { get; set; }
    public required string Type { get; set; }

    // Boş constructor
    public CartItem() { }

    // Parametreli constructor
    public CartItem(int productId, string productName, decimal price, int quantity, string pictureUrl, string brand, string type)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
        PictureUrl = pictureUrl;
        Brand = brand;
        Type = type;
    }
}
