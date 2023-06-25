namespace BussinessObject;

public class Product
{
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    public string? ProductName { get; set; }

    public int? Weight { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? UnitInStock { get; set; }

    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    public virtual Category? Category { get; set; }
}