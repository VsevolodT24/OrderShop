using Microsoft.EntityFrameworkCore;
using OrderShop.Core.Entities;

namespace OrderShop.DAL;

public class OrderShopDbContext : DbContext
{
    public OrderShopDbContext(DbContextOptions<OrderShopDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<Customer>().Property(c => c.FullName).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(100);

        modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Order>().HasOne(o => o.Employee).WithMany(e => e.Orders)
            .HasForeignKey(o => o.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Product>().HasOne(p => p.Supplier).WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>().HasOne(i => i.Order).WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<OrderItem>().HasOne(i => i.Product).WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>().HasOne(o => o.Delivery).WithOne(d => d.Order)
            .HasForeignKey<Delivery>(d => d.OrderId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>().HasOne(p => p.Order).WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}