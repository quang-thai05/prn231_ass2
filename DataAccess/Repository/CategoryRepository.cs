using BussinessObject;
using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;

namespace DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly EStoreDbContext _context;

    public CategoryRepository(EStoreDbContext context) : base(context)
    {
        _context = context;
    }
}