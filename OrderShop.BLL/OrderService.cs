using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using OrderShop.DAL.Repositories;

namespace OrderShop.BLL;

/// <summary>
/// Бизнес-логика оформления заказа. Работает только с интерфейсом IUnitOfWork
/// и ничего не знает о базе данных — слои DAL и BLL разделены.
/// </summary>
public class OrderService
{
    private readonly IUnitOfWork _uow;

    public OrderService(IUnitOfWork uow) => _uow = uow;

    /// <param name="items">Словарь «Id товара -> количество».</param>
    public Order PlaceOrder(int customerId, int employeeId, IDictionary<int, int> items)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Заказ должен содержать хотя бы одну позицию.");

        if (_uow.Customers.GetById(customerId) is null)
            throw new InvalidOperationException($"Клиент с Id={customerId} не найден.");

        if (_uow.Employees.GetById(employeeId) is null)
            throw new InvalidOperationException($"Сотрудник с Id={employeeId} не найден.");

        // 1-й проход: проверяем все позиции, ничего не изменяя
        var lines = new List<(Product Product, int Quantity)>();
        foreach (KeyValuePair<int, int> item in items)
        {
            Product product = _uow.Products.GetById(item.Key)
                ?? throw new InvalidOperationException($"Товар с Id={item.Key} не найден.");

            if (item.Value <= 0)
                throw new ArgumentException($"Количество товара '{product.Name}' должно быть больше нуля.");

            if (product.StockQuantity < item.Value)
                throw new InvalidOperationException($"Недостаточно товара '{product.Name}' на складе.");

            lines.Add((product, item.Value));
        }

        // 2-й проход: формируем заказ и списываем остатки
        var order = new Order
        {
            CustomerId = customerId,
            EmployeeId = employeeId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.New
        };

        decimal total = 0m;
        foreach (var (product, quantity) in lines)
        {
            product.StockQuantity -= quantity;
            _uow.Products.Update(product);

            order.Items.Add(new OrderItem
            {
                ProductId = product.ProductId,
                Quantity = quantity,
                UnitPrice = product.Price
            });

            total += product.Price * quantity;
        }

        order.TotalAmount = total;
        _uow.Orders.Add(order);
        _uow.Complete(); // заказ, позиции и остатки товаров сохраняются одной транзакцией

        return order;
    }
}