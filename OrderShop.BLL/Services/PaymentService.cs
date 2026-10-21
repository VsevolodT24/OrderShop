using OrderShop.BLL.Abstractions;
using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using OrderShop.DAL.Repositories;

namespace OrderShop.BLL.Services;

public class PaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IPaymentGateway _gateway;
    private readonly IClock _clock;

    public PaymentService(IUnitOfWork uow, IPaymentGateway gateway, IClock clock)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public Payment Pay(int orderId, PaymentMethod method)
    {
        Order order = _uow.Orders.GetById(orderId)
            ?? throw new InvalidOperationException($"Заказ с Id={orderId} не найден.");

        if (order.Status != OrderStatus.New)
            throw new InvalidOperationException($"Оплатить можно только новый заказ, текущий статус: {order.Status}.");

        PaymentResult result = _gateway.Charge(order.TotalAmount, method);
        if (!result.Success)
            throw new InvalidOperationException(result.Error ?? "Платёж отклонён.");

        var payment = new Payment
        {
            OrderId = order.OrderId,
            Amount = order.TotalAmount,
            Method = method,
            PaidAt = _clock.UtcNow
        };

        order.Payments.Add(payment);
        order.Status = OrderStatus.Paid;
        _uow.Orders.Update(order);
        _uow.Complete();
        return payment;
    }
}