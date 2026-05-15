using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;
using System.Linq.Expressions;

namespace Netflix_clone.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly NetflixContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(NetflixContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IEnumerable<T> GetAll() => _dbSet.ToList();

    public IQueryable<T> GetAllQueryable() => _dbSet.AsQueryable();

    public T? GetById(int id) => _dbSet.Find(id);

    public T? GetById(string id) => _dbSet.Find(id);

    public T? GetByIdWithIncludes(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
    }

    public IEnumerable<T> GetAllWithIncludes(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query.ToList();
    }

    public T? Find(Expression<Func<T, bool>> predicate) => _dbSet.FirstOrDefault(predicate);

    public IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).ToList();

    public void Add(T entity) => _dbSet.Add(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public void Save() => _context.SaveChanges();

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
}
