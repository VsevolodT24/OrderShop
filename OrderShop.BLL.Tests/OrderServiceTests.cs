using Moq;
using OrderShop.BLL.Services;
using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using Xunit;

namespace OrderShop.BLL.Tests;

public class OrderServiceTests
{
    // --- Конструктор ---
    [Fact]
    public void Ctor_NullUow_Throws()
    {
        var f = new OrderFixture();
        Assert.Throws<ArgumentNullException>(() => new OrderService(null!, f.Discounts.Object, f.Clock.Object));
    }

    [Fact]
    public void Ctor_NullDiscounts_Throws()
    {
        var f = new OrderFixture();
        Assert.Throws<ArgumentNullException>(() => new OrderService(f.Uow.Object, null!, f.Clock.Object));
    }

    [Fact]
    public void Ctor_NullClock_Throws()
    {
        var f = new OrderFixture();
        Assert.Throws<ArgumentNullException>(() => new OrderService(f.Uow.Object, f.Discounts.Object, null!));
    }

    // --- Проверка входных данных ---
    [Fact]
    public void PlaceOrder_NullItems_Throws()
    {
        var service = new OrderFixture().CreateService();
        Assert.Throws<ArgumentException>(() => service.PlaceOrder(1, 1, null!));
    }

    [Fact]
    public void PlaceOrder_EmptyItems_Throws()
    {
        var service = new OrderFixture().CreateService();
        Assert.Throws<ArgumentException>(() => service.PlaceOrder(1, 1, new Dictionary<int, int>()));
    }

    [Fact]
    public void PlaceOrder_UnknownCustomer_Throws()
    {
        var service = new OrderFixture().CreateService();
        var ex = Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(99, 1, new Dictionary<int, int> { { 1, 1 } }));
        Assert.Contains("Клиент", ex.Message);
    }

    [Fact]
    public void PlaceOrder_UnknownEmployee_Throws()
    {
        var service = new OrderFixture().CreateService();
        var ex = Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(1, 99, new Dictionary<int, int> { { 1, 1 } }));
        Assert.Contains("Сотрудник", ex.Message);
    }

    [Fact]
    public void PlaceOrder_UnknownProduct_Throws()
    {
        var service = new OrderFixture().CreateService();
        var ex = Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(1, 1, new Dictionary<int, int> { { 99, 1 } }));
        Assert.Contains("Товар", ex.Message);
    }

    [Fact]
    public void PlaceOrder_ZeroQuantity_Throws()
    {
        var service = new OrderFixture().CreateService();
        Assert.Throws<ArgumentException>(() => service.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 0 } }));
    }

    [Fact]
    public void PlaceOrder_NotEnoughStock_Throws()
    {
        var service = new OrderFixture().CreateService();
        var ex = Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 100 } }));
        Assert.Contains("Недостаточно", ex.Message);
    }

    // --- Промокоды ---
    [Fact]
    public void PlaceOrder_WithPromo_AppliesDiscount()
    {
        var f = new OrderFixture();
        var promo = new PromoCode { PromoCodeId = 5, Code = "SALE10", DiscountPercent = 10m, ValidUntil = OrderFixture.Now.AddDays(1) };
        f.Promos.Setup(p => p.GetByCode("SALE10")).Returns(promo);
        f.Discounts.Setup(d => d.Calculate(2500m, promo, OrderFixture.Now)).Returns(250m);
        
        var service = f.CreateService();
        Order order = service.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 2 }, { 2, 1 } }, "SALE10");

        Assert.Equal(250m, order.DiscountAmount);
        Assert.Equal(2250m, order.TotalAmount);
        Assert.Equal(5, order.PromoCodeId);
        f.Promos.Verify(p => p.GetByCode("SALE10"), Times.Once);
    }

    [Fact]
    public void PlaceOrder_UnknownPromo_ThrowsAndDoesNotSave()
    {
        var f = new OrderFixture();
        var service = f.CreateService();
        var ex = Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 1 } }, "NOPE"));
        
        Assert.Contains("NOPE", ex.Message);
        f.Uow.Verify(u => u.Complete(), Times.Never);
        Assert.Equal(10, f.Laptop.StockQuantity);
    }

    [Fact]
    public void PlaceOrder_DiscountCalculatorFails_PropagatesAndDoesNotSave()
    {
        var f = new OrderFixture();
        var promo = new PromoCode { PromoCodeId = 7, Code = "OLD" };
        f.Promos.Setup(p => p.GetByCode("OLD")).Returns(promo);
        f.Discounts.Setup(d => d.Calculate(It.IsAny<decimal>(), promo, It.IsAny<DateTime>()))
                   .Throws(new InvalidOperationException("Срок действия промокода истёк."));
        var service = f.CreateService();

        Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 1 } }, "OLD"));
        f.Uow.Verify(u => u.Complete(), Times.Never);
        Assert.Equal(10, f.Laptop.StockQuantity);
    }

    [Fact]
    public void PlaceOrder_ValidOrder_SavesAndUpdatesStock()
    {
        var f = new OrderFixture();
        var service = f.CreateService();
        Order order = service.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 2 }, { 2, 1 } });

        Assert.Equal(OrderStatus.New, order.Status);
        Assert.Equal(2500m, order.TotalAmount);
        Assert.Equal(2, order.Items.Count);
        f.Uow.Verify(u => u.Complete(), Times.Once);
        f.Products.Verify(p => p.Update(It.IsAny<Product>()), Times.Exactly(2));
    }
}