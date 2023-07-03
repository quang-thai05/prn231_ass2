using BussinessObject;

namespace Lab2.DTOs.Cart;

public class CartItem
{
    public int ProductId { get; set; }
    
    public string ProductName { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal? UnitPrice { get; set; } 
}