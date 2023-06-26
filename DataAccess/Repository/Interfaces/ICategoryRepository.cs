using BussinessObject;

namespace DataAccess.Repository.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Category? FindById(int id);
}