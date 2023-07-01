namespace Lab2.DTOs.Cart;

public class ProductCartItem
{
    public int ProductId { get; set; }
    
    public string? ProductName { get; set; }

    public int? Weight { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? UnitInStock { get; set; }
}