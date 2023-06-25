using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly EStoreDbContext _context;
    private readonly DbSet<T> _entities;

    public Repository(EStoreDbContext context)
    {
        _context = context;
        _entities = _context.Set<T>();
    }

    public DbSet<T> GetDbSet() => _entities;

    public async Task<List<T>> GetAll()
    {
        return await _entities.ToListAsync();
    }

    public async Task Add(T entity)
    {
        _entities.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(T entity)
    {
        if (_context.Entry<T>(entity) == null) throw new NullReferenceException("Record not found");
        _entities.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        if (_context.Entry<T>(entity) == null) throw new NullReferenceException("Record not found");
        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }
}