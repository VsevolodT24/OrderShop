using OrderShop.Core.Entities;

namespace OrderShop.DAL.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(OrderShopDbContext context) : base(context) { }

    public Customer? GetByEmail(string email) => Set.FirstOrDefault(c => c.Email == email);
    public IEnumerable<Customer> SearchByName(string namePart) => 
        Set.Where(c => c.FullName.Contains(namePart)).ToList();
}