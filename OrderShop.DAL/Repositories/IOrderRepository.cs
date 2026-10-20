using OrderShop.Core.Entities;
using OrderShop.Core.Enums;

namespace OrderShop.DAL.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Order? GetWithItems(int orderId);
    IEnumerable<Order> GetByCustomer(int customerId);
    IEnumerable<Order> GetByStatus(OrderStatus status);
}