using OrderShop.Core.Entities;

namespace OrderShop.BLL.Interfaces;

public interface ICustomerService
{
    IEnumerable<Customer> GetAllCustomers();
}