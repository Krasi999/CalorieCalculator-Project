using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace DataLayer.Repository;

public class Repository : IRepository
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Add<T>(T entity) where T : class
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task AddRange<T>(IEnumerable<T> entities) where T : class
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }

    public void Update<T>(T entity) where T : class
    {
        _context.Set<T>().Update(entity);
    }

    public void UpdateRange<T>(IEnumerable<T> entities) where T : class
    {
        _context.Set<T>().UpdateRange(entities);
    }

    public void Delete<T>(T entity) where T : class
    {
        _context.Set<T>().Remove(entity);
    }

    public void DeleteRange<T>(IEnumerable<T> entities) where T : class
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public IQueryable<T> Set<T>() where T : class
    {
        return _context.Set<T>().AsTracking();
    }

    public IQueryable<T> SetNoTracking<T>() where T : class
    {
        return _context.Set<T>().AsNoTracking();
    }

    public IQueryable<T> Query<T>() where T : class
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<List<T>> Find<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<int> ExecuteSql(string sql)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql);
    }

    public async Task<IDbContextTransaction> BeginTransaction()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<int> SaveChanges()
    {
        return await _context.SaveChangesAsync();
    }
}
