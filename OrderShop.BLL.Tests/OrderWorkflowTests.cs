using Moq;
using OrderShop.BLL.Services;
using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using OrderShop.DAL.Repositories;
using Xunit;

namespace OrderShop.BLL.Tests;

public class OrderWorkflowTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IOrderRepository> _orders = new();

    public OrderWorkflowTests() => _uow.Setup(u => u.Orders).Returns(_orders.Object);

    private static bool Expected(OrderStatus from, OrderStatus to) => (from, to) switch
    {
        (OrderStatus.New, OrderStatus.Paid) => true,
        (OrderStatus.New, OrderStatus.Cancelled) => true,
        (OrderStatus.Paid, OrderStatus.Shipped) => true,
        (OrderStatus.Paid, OrderStatus.Cancelled) => true,
        (OrderStatus.Shipped, OrderStatus.Delivered) => true,
        _ => false
    };

    public static IEnumerable<object[]> AllTransitions()
    {
        foreach (OrderStatus from in Enum.GetValues<OrderStatus>())
            foreach (OrderStatus to in Enum.GetValues<OrderStatus>())
                yield return new object[] { from, to };
    }

    [Theory]
    [MemberData(nameof(AllTransitions))]
    public void IsTransitionAllowed_MatchesRules(OrderStatus from, OrderStatus to)
    {
        Assert.Equal(Expected(from, to), OrderWorkflow.IsTransitionAllowed(from, to));
    }

    [Fact]
    public void Ctor_NullUow_Throws() => Assert.Throws<ArgumentNullException>(() => new OrderWorkflow(null!));

    [Fact]
    public void ChangeStatus_UnknownOrder_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new OrderWorkflow(_uow.Object).ChangeStatus(404, OrderStatus.Paid));
        Assert.Contains("не найден", ex.Message);
    }

    [Fact]
    public void ChangeStatus_ForbiddenTransition_ThrowsAndDoesNotSave()
    {
        var order = new Order { OrderId = 1, Status = OrderStatus.Delivered };
        _orders.Setup(o => o.GetById(1)).Returns(order);

        Assert.Throws<InvalidOperationException>(() => new OrderWorkflow(_uow.Object).ChangeStatus(1, OrderStatus.New));
        Assert.Equal(OrderStatus.Delivered, order.Status);
        _uow.Verify(u => u.Complete(), Times.Never);
    }

    [Fact]
    public void ChangeStatus_AllowedTransition_UpdatesAndSaves()
    {
        var order = new Order { OrderId = 1, Status = OrderStatus.Paid };
        _orders.Setup(o => o.GetById(1)).Returns(order);

        new OrderWorkflow(_uow.Object).ChangeStatus(1, OrderStatus.Shipped);
        
        Assert.Equal(OrderStatus.Shipped, order.Status);
        _orders.Verify(o => o.Update(order), Times.Once);
        _uow.Verify(u => u.Complete(), Times.Once);
    }
}