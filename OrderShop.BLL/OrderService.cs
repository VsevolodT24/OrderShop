using OrderShop.BLL.Abstractions;
using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using OrderShop.DAL.Repositories;

namespace OrderShop.BLL.Services;

/// <summary>
/// Бизнес-логика оформления заказа.
/// После рефакторинга все зависимости (БД, расчёт скидки, время) передаются через интерфейсы.
/// </summary>
public class OrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IDiscountCalculator _discounts;
    private readonly IClock _clock;

    public OrderService(IUnitOfWork uow, IDiscountCalculator discounts, IClock clock)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _discounts = discounts ?? throw new ArgumentNullException(nameof(discounts));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <param name="items">Словарь «Id товара -> количество».</param>
    /// <param name="promoCode">Необязательный промокод.</param>
    public Order PlaceOrder(int customerId, int employeeId, IDictionary<int, int> items, string? promoCode = null)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Заказ должен содержать хотя бы одну позицию.");
        if (_uow.Customers.GetById(customerId) is null)
            throw new InvalidOperationException($"Клиент с Id={customerId} не найден.");
        if (_uow.Employees.GetById(employeeId) is null)
            throw new InvalidOperationException($"Сотрудник с Id={employeeId} не найден.");

        var pairs = new List<KeyValuePair<int, int>>(items);
        var orderItems = new List<OrderItem>();
        decimal subtotal = 0m;

        for (int i = 0; i < pairs.Count; i++)
        {
            Product product = _uow.Products.GetById(pairs[i].Key)
                ?? throw new InvalidOperationException($"Товар с Id={pairs[i].Key} не найден.");

            int quantity = pairs[i].Value;
            if (quantity <= 0)
                throw new ArgumentException($"Количество товара '{product.Name}' должно быть больше нуля.");
            if (product.StockQuantity < quantity)
                throw new InvalidOperationException($"Недостаточно товара '{product.Name}' на складе.");

            orderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                Product = product,
                Quantity = quantity,
                UnitPrice = product.Price
            });
            subtotal += product.Price * quantity;
        }

        PromoCode? promo = null;
        if (!string.IsNullOrWhiteSpace(promoCode))
        {
            promo = _uow.PromoCodes.GetByCode(promoCode.Trim())
                ?? throw new InvalidOperationException($"Промокод '{promoCode.Trim()}' не найден.");
        }

        DateTime now = _clock.UtcNow;
        decimal discount = _discounts.Calculate(subtotal, promo, now);

        var order = new Order
        {
            CustomerId = customerId,
            EmployeeId = employeeId,
            OrderDate = now,
            Status = OrderStatus.New,
            PromoCodeId = promo?.PromoCodeId,
            DiscountAmount = discount,
            TotalAmount = subtotal - discount
        };

        for (int i = 0; i < orderItems.Count; i++)
        {
            Product product = orderItems[i].Product!;
            product.StockQuantity -= orderItems[i].Quantity;
            _uow.Products.Update(product);
            order.Items.Add(orderItems[i]);
        }

        _uow.Orders.Add(order);
        _uow.Complete();
        return order;
    }
}