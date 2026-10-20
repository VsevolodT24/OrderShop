using OrderShop.Core.Entities;

namespace OrderShop.DAL.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Customer? GetByEmail(string email);
    IEnumerable<Customer> SearchByName(string namePart);
}