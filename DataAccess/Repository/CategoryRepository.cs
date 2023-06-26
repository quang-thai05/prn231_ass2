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

    public Category? FindById(int id)
    {
        return _context.Categories.FirstOrDefault(x => x.CategoryId == id);
    }
}