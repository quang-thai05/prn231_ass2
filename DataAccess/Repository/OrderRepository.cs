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
}