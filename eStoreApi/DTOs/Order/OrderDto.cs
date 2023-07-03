namespace Lab2.DTOs;

public class OrderDto
{
    public DateTime? OrderDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public int? Freight { get; set; }
    
    public List<OrderDetailDto> OrderDetails { get; set; }
}