using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UcakRezervasyon.DataAccess.Data;

namespace UcakRezervasyon.DataAccess.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<T?> GetAsync(int id) => await _dbSet.FindAsync(id);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public void Update(T entity) => _dbSet.Update(entity);
}
