using Microsoft.EntityFrameworkCore;
using Netflix_clone.Models;

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

    public T? GetById(int id) => _dbSet.Find(id);

    public void Add(T entity) => _dbSet.Add(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public void Save() => _context.SaveChanges();
}
