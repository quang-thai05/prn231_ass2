using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.Interfaces;

public interface IRepository<T> where T : class
{
    public DbSet<T> GetDbSet();
    
    public Task<List<T>> GetAll();

    public Task Add(T entity);

    public Task Update(T entity);

    public Task Delete(T entity);
}