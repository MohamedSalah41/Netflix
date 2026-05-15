using System.Linq.Expressions;

namespace Netflix_clone.Repositories;

public interface IGenericRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    IQueryable<T> GetAllQueryable();
    T? GetById(int id);
    T? GetById(string id);
    T? GetByIdWithIncludes(int id, params Expression<Func<T, object>>[] includes);
    IEnumerable<T> GetAllWithIncludes(params Expression<Func<T, object>>[] includes);
    T? Find(Expression<Func<T, bool>> predicate);
    IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Save();
    Task SaveAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
}
