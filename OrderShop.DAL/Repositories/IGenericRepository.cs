namespace OrderShop.DAL.Repositories;

public interface IGenericRepository<T> : IRepository<T>, IAsyncRepository<T> where T : class
{
}