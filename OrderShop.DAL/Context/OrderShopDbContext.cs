using Microsoft.EntityFrameworkCore;
using OrderShop.Core.Entities;

namespace OrderShop.DAL.Context;

public class OrderShopDbContext : DbContext
{
    public OrderShopDbContext(DbContextOptions<OrderShopDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();
}