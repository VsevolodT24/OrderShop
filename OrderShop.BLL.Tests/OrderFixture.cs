using Moq;
using OrderShop.BLL.Abstractions;
using OrderShop.BLL.Services;
using OrderShop.Core.Entities;
using OrderShop.DAL.Repositories;

namespace OrderShop.BLL.Tests;

internal sealed class OrderFixture
{
    public static readonly DateTime Now = new(2026, 10, 21, 12, 0, 0, DateTimeKind.Utc);

    public readonly Mock<IUnitOfWork> Uow = new();
    public readonly Mock<ICustomerRepository> Customers = new();
    public readonly Mock<IGenericRepository<Employee>> Employees = new();
    public readonly Mock<IProductRepository> Products = new();
    public readonly Mock<IOrderRepository> Orders = new();
    public readonly Mock<IPromoCodeRepository> Promos = new();
    public readonly Mock<IDiscountCalculator> Discounts = new();
    public readonly Mock<IClock> Clock = new();

    public readonly Product Laptop = new() { ProductId = 1, Name = "Ноутбук", Price = 1000m, StockQuantity = 10 };
    public readonly Product Mouse = new() { ProductId = 2, Name = "Мышь", Price = 500m, StockQuantity = 2 };

    public OrderFixture()
    {
        Uow.Setup(u => u.Customers).Returns(Customers.Object);
        Uow.Setup(u => u.Employees).Returns(Employees.Object);
        Uow.Setup(u => u.Products).Returns(Products.Object);
        Uow.Setup(u => u.Orders).Returns(Orders.Object);
        Uow.Setup(u => u.PromoCodes).Returns(Promos.Object);

        Customers.Setup(c => c.GetById(1)).Returns(new Customer { CustomerId = 1, FullName = "Петров Петр" });
        Employees.Setup(e => e.GetById(1)).Returns(new Employee { EmployeeId = 1, FullName = "Сидорова Анна" });
        Products.Setup(p => p.GetById(1)).Returns(Laptop);
        Products.Setup(p => p.GetById(2)).Returns(Mouse);
        Clock.Setup(c => c.UtcNow).Returns(Now);
        Discounts.Setup(d => d.Calculate(It.IsAny<decimal>(), It.IsAny<PromoCode?>(), It.IsAny<DateTime>())).Returns(0m);
    }

    public OrderService CreateService() => new(Uow.Object, Discounts.Object, Clock.Object);
}