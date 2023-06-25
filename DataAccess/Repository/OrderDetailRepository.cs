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
}