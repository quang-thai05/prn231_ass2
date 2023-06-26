using BussinessObject;
using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;

namespace DataAccess.Repository;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly EStoreDbContext _context;

    public OrderRepository(EStoreDbContext context) : base(context)
    {
        _context = context;
    }

    public List<Order> GetOrdersByUserId(string userId)
    {
        var orders = _context.Orders.Where(x => x.MemberId.Equals(userId)).ToList();
        return orders;
    }

    public Order? GetById(int id)
    {
        return _context.Orders.FirstOrDefault(x => x.OrderId == id);
    }
}