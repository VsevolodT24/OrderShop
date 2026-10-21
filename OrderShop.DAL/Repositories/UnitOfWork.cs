using OrderShop.Core.Entities;

namespace OrderShop.DAL.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    IGenericRepository<Employee> Employees { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Supplier> Suppliers { get; }
    IPromoCodeRepository PromoCodes { get; }

    int Complete();
    Task<int> CompleteAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderShopDbContext _context;

    public UnitOfWork(OrderShopDbContext context)
    {
        _context = context;
        Products = new ProductRepository(context);
        Customers = new CustomerRepository(context);
        Orders = new OrderRepository(context);
        Employees = new GenericRepository<Employee>(context);
        Categories = new GenericRepository<Category>(context);
        Suppliers = new GenericRepository<Supplier>(context);
        PromoCodes = new PromoCodeRepository(context);
    }

    public IProductRepository Products { get; }
    public ICustomerRepository Customers { get; }
    public IOrderRepository Orders { get; }
    public IGenericRepository<Employee> Employees { get; }
    public IGenericRepository<Category> Categories { get; }
    public IGenericRepository<Supplier> Suppliers { get; }
    public IPromoCodeRepository PromoCodes { get; }

    public int Complete() => _context.SaveChanges();
    public Task<int> CompleteAsync() => _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}