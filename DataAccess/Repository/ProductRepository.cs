using BussinessObject;
using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;

namespace DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly EStoreDbContext _context;

    public ProductRepository(EStoreDbContext context) : base(context)
    {
        _context = context;
    }
}