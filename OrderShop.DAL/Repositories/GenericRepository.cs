using Microsoft.EntityFrameworkCore;

namespace OrderShop.DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly OrderShopDbContext Context;
    protected readonly DbSet<T> Set;

    public GenericRepository(OrderShopDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    // Синхронные
    public virtual T? GetById(int id) => Set.Find(id);
    public virtual IEnumerable<T> GetAll() => Set.ToList();
    public virtual void Add(T entity) => Set.Add(entity);
    public virtual void Update(T entity) => Set.Update(entity);
    public virtual void Delete(int id)
    {
        T? entity = Set.Find(id);
        if (entity != null) Set.Remove(entity);
    }

    // Асинхронные
    public virtual async Task<T?> GetByIdAsync(int id) => await Set.FindAsync(id);
    public virtual async Task<IEnumerable<T>> GetAllAsync() => await Set.ToListAsync();
    public virtual async Task AddAsync(T entity) => await Set.AddAsync(entity);
    public virtual Task UpdateAsync(T entity)
    {
        Set.Update(entity);
        return Task.CompletedTask;
    }
    public virtual async Task DeleteAsync(int id)
    {
        T? entity = await Set.FindAsync(id);
        if (entity != null) Set.Remove(entity);
    }
}