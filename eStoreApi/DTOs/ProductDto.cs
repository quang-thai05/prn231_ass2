namespace Lab2.DTOs;

public class ProductDto
{
    public int CategoryId { get; set; }
    
    public string? ProductName { get; set; }
    
    public int Weight { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public int UnitInStock { get; set; }
}