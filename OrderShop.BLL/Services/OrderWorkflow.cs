using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using OrderShop.DAL.Repositories;

namespace OrderShop.BLL.Services;

public class OrderWorkflow
{
    private readonly IUnitOfWork _uow;

    public OrderWorkflow(IUnitOfWork uow)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public static bool IsTransitionAllowed(OrderStatus from, OrderStatus to)
    {
        return (from, to) switch
        {
            (OrderStatus.New, OrderStatus.Paid) => true,
            (OrderStatus.New, OrderStatus.Cancelled) => true,
            (OrderStatus.Paid, OrderStatus.Shipped) => true,
            (OrderStatus.Paid, OrderStatus.Cancelled) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };
    }

    public void ChangeStatus(int orderId, OrderStatus newStatus)
    {
        Order order = _uow.Orders.GetById(orderId)
            ?? throw new InvalidOperationException($"Заказ с Id={orderId} не найден.");

        if (!IsTransitionAllowed(order.Status, newStatus))
            throw new InvalidOperationException($"Переход из {order.Status} в {newStatus} запрещён.");

        order.Status = newStatus;
        _uow.Orders.Update(order);
        _uow.Complete();
    }
}