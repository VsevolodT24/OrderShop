using OrderShop.BLL.Interfaces;
using OrderShop.Core.Entities;
using OrderShop.DAL.Repositories;

namespace OrderShop.BLL.Services;

public class CustomerService : ICustomerService
{
    private readonly CustomerRepository _repository;

    public CustomerService(CustomerRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Customer> GetAllCustomers() => _repository.GetAll();
}