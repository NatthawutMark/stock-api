using Microsoft.EntityFrameworkCore;
using back_stock.Interfaces;
using back_stock.Models;

namespace back_stock.Repositories.EF;

public class EfGenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DbContexts _context;
    protected readonly DbSet<T> _dbSet;

    public EfGenericRepository(DbContexts context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Remove(T entity) => _dbSet.Remove(entity);
}