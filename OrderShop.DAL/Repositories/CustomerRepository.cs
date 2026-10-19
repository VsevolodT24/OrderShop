using OrderShop.Core.Entities;
using OrderShop.DAL.Context;

namespace OrderShop.DAL.Repositories;

public class CustomerRepository
{
    private readonly OrderShopDbContext _context;

    public CustomerRepository(OrderShopDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Customer> GetAll() => _context.Customers.ToList();
}