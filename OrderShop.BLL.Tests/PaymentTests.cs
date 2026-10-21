using Moq;
using OrderShop.BLL.Abstractions;
using OrderShop.BLL.Infrastructure;
using OrderShop.BLL.Services;
using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using OrderShop.DAL.Repositories;
using Xunit;

namespace OrderShop.BLL.Tests;

public class PaymentServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IPaymentGateway> _gateway = new();
    private readonly Mock<IClock> _clock = new();
    private static readonly DateTime Now = new(2026, 10, 21, 12, 0, 0, DateTimeKind.Utc);

    public PaymentServiceTests()
    {
        _uow.Setup(u => u.Orders).Returns(_orders.Object);
        _clock.Setup(c => c.UtcNow).Returns(Now);
    }

    private PaymentService CreateService() => new(_uow.Object, _gateway.Object, _clock.Object);

    private Order NewOrder(OrderStatus status = OrderStatus.New)
    {
        var order = new Order { OrderId = 10, TotalAmount = 2500m, Status = status };
        _orders.Setup(o => o.GetById(10)).Returns(order);
        return order;
    }

    [Fact]
    public void Ctor_NullUow_Throws() => Assert.Throws<ArgumentNullException>(() => new PaymentService(null!, _gateway.Object, _clock.Object));

    [Fact]
    public void Ctor_NullGateway_Throws() => Assert.Throws<ArgumentNullException>(() => new PaymentService(_uow.Object, null!, _clock.Object));

    [Fact]
    public void Ctor_NullClock_Throws() => Assert.Throws<ArgumentNullException>(() => new PaymentService(_uow.Object, _gateway.Object, null!));

    [Fact]
    public void Pay_UnknownOrder_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => CreateService().Pay(404, PaymentMethod.Card));
        Assert.Contains("не найден", ex.Message);
        _gateway.Verify(g => g.Charge(It.IsAny<decimal>(), It.IsAny<PaymentMethod>()), Times.Never);
    }

    [Fact]
    public void Pay_OrderAlreadyPaid_Throws()
    {
        NewOrder(OrderStatus.Paid);
        var ex = Assert.Throws<InvalidOperationException>(() => CreateService().Pay(10, PaymentMethod.Card));
        Assert.Contains("Paid", ex.Message);
        _gateway.Verify(g => g.Charge(It.IsAny<decimal>(), It.IsAny<PaymentMethod>()), Times.Never);
    }

    [Fact]
    public void Pay_GatewayDeclinesWithMessage_ThrowsWithThatMessage()
    {
        NewOrder();
        _gateway.Setup(g => g.Charge(2500m, PaymentMethod.Card)).Returns(new PaymentResult(false, "Недостаточно средств."));
        var ex = Assert.Throws<InvalidOperationException>(() => CreateService().Pay(10, PaymentMethod.Card));
        Assert.Equal("Недостаточно средств.", ex.Message);
        _uow.Verify(u => u.Complete(), Times.Never);
    }

    [Fact]
    public void Pay_GatewayDeclinesWithoutMessage_ThrowsDefaultMessage()
    {
        NewOrder();
        _gateway.Setup(g => g.Charge(It.IsAny<decimal>(), It.IsAny<PaymentMethod>())).Returns(new PaymentResult(false, null));
        var ex = Assert.Throws<InvalidOperationException>(() => CreateService().Pay(10, PaymentMethod.Online));
        Assert.Equal("Платёж отклонён.", ex.Message);
    }

    [Fact]
    public void Pay_Success_CreatesPaymentAndMarksOrderPaid()
    {
        Order order = NewOrder();
        _gateway.Setup(g => g.Charge(2500m, PaymentMethod.Card)).Returns(new PaymentResult(true, null));
        
        Payment payment = CreateService().Pay(10, PaymentMethod.Card);

        Assert.Equal(10, payment.OrderId);
        Assert.Equal(2500m, payment.Amount);
        Assert.Equal(PaymentMethod.Card, payment.Method);
        Assert.Equal(Now, payment.PaidAt);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Contains(payment, order.Payments);
        _orders.Verify(o => o.Update(order), Times.Once);
        _uow.Verify(u => u.Complete(), Times.Once);
    }
}

public class OfflinePaymentGatewayTests
{
    private readonly OfflinePaymentGateway _gateway = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Charge_NonPositiveAmount_Throws(double amount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _gateway.Charge((decimal)amount, PaymentMethod.Card));
    }

    [Fact]
    public void Charge_CashOverLimit_IsDeclined()
    {
        PaymentResult result = _gateway.Charge(OfflinePaymentGateway.MaxCashAmount + 1m, PaymentMethod.Cash);
        Assert.False(result.Success);
        Assert.Equal("Превышен лимит оплаты наличными.", result.Error);
    }

    [Fact]
    public void Charge_CardOverCashLimit_IsAccepted()
    {
        PaymentResult result = _gateway.Charge(OfflinePaymentGateway.MaxCashAmount + 1m, PaymentMethod.Card);
        Assert.True(result.Success);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Charge_CashAtLimit_IsAccepted()
    {
        Assert.True(_gateway.Charge(OfflinePaymentGateway.MaxCashAmount, PaymentMethod.Cash).Success);
    }
}