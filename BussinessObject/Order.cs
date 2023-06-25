namespace BussinessObject;

public class Order
{
    public int OrderId { get; set; }

    public string? MemberId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public int? Freight { get; set; }
    
    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    
    public virtual User? User { get; set; }
}