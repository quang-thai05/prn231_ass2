using BussinessObject;
using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;

namespace DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly EStoreDbContext _context;

    public OrderDetailRepository(EStoreDbContext context) : base(context)
    {
        _context = context;
    }

    public List<OrderDetail>? GetByOrderId(int orderId)
    {
        var details = _context.OrderDetails.Where(x => x.OrderId == orderId).ToList();
        return details;
    }

    public List<OrderDetail>? GetByProductId(int productId)
    {
        var details = _context.OrderDetails.Where(x => x.ProductId == productId).ToList();
        return details;
    }

    public async Task AddOrderDetails(IEnumerable<OrderDetail> orderDetails)
    {
        _context.OrderDetails.AddRange(orderDetails);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderDetails(IEnumerable<OrderDetail> orderDetails)
    {
        _context.OrderDetails.RemoveRange(orderDetails);
        await _context.SaveChangesAsync();
    }
}