using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace DataLayer.Repository;

public interface IRepository
{
    Task Add<T>(T entity) where T : class;

    Task AddRange<T>(IEnumerable<T> entities) where T : class;

    void Update<T>(T entity) where T : class;

    void UpdateRange<T>(IEnumerable<T> entities) where T : class;

    void Delete<T>(T entity) where T : class;

    void DeleteRange<T>(IEnumerable<T> entities) where T : class;

    IQueryable<T> Set<T>() where T : class;

    IQueryable<T> SetNoTracking<T>() where T : class;

    IQueryable<T> Query<T>() where T : class;

    Task<List<T>> Find<T>(Expression<Func<T, bool>> predicate) where T : class;

    Task<int> ExecuteSql(string sql);

    Task<IDbContextTransaction> BeginTransaction();

    Task<int> SaveChanges();
}
