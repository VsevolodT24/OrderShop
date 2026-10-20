using Microsoft.EntityFrameworkCore;
using OrderShop.Core.Entities;
using OrderShop.Core.Enums;

namespace OrderShop.DAL.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(OrderShopDbContext context) : base(context) { }

    public Order? GetWithItems(int orderId) => 
        Set.Include(o => o.Items).ThenInclude(i => i.Product)
           .FirstOrDefault(o => o.OrderId == orderId);

    public IEnumerable<Order> GetByCustomer(int customerId) => 
        Set.Where(o => o.CustomerId == customerId).ToList();

    public IEnumerable<Order> GetByStatus(OrderStatus status) => 
        Set.Where(o => o.Status == status).ToList();
}