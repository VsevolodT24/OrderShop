using OrderShop.BLL.Services;
using OrderShop.BLL.Infrastructure;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OrderShop.BLL;
using OrderShop.Core.Entities;
using OrderShop.Core.Enums;
using OrderShop.DAL;
using OrderShop.DAL.Repositories;

Console.OutputEncoding = Encoding.UTF8;

var options = new DbContextOptionsBuilder<OrderShopDbContext>()
    .UseSqlite("Data Source=ordershop_demo.db")
    .Options;

// Удаляем старую БД, чтобы при каждом запуске всё было с нуля
using (var ctx = new OrderShopDbContext(options))
{
    ctx.Database.EnsureDeleted();
    ctx.Database.EnsureCreated();
}

Console.WriteLine("=== Часть 2, задание 1. Подготовка БД ===");
using (var ctx = new OrderShopDbContext(options))
{
    var category = new Category { Name = "Электроника" };
    var supplier = new Supplier { CompanyName = "ООО Технопоставка" };
    ctx.Categories.Add(category);
    ctx.Suppliers.Add(supplier);
    ctx.SaveChanges();
    Console.WriteLine($"База данных создана (SQLite): ordershop_demo.db");
    Console.WriteLine($"Категория: {category.Name} (Id={category.CategoryId}); поставщик: {supplier.CompanyName} (Id={supplier.SupplierId})");
}

Console.WriteLine("=== Задание 3. CRUD через ProductRepository ===");
using (var ctx = new OrderShopDbContext(options))
{
    IProductRepository products = new ProductRepository(ctx);

    // CREATE
    products.Add(new Product { Name = "Ноутбук Lenovo", Price = 65000m, StockQuantity = 10, CategoryId = 1, SupplierId = 1 });
    products.Add(new Product { Name = "Мышь Logitech", Price = 1500m, StockQuantity = 50, CategoryId = 1, SupplierId = 1 });
    products.Add(new Product { Name = "Монитор Samsung", Price = 18000m, StockQuantity = 0, CategoryId = 1, SupplierId = 1 });
    products.Add(new Product { Name = "Клавиатура Keychron", Price = 7500m, StockQuantity = 5, CategoryId = 1, SupplierId = 1 });
    ctx.SaveChanges();
    Console.WriteLine("CREATE – добавлено 4 товара:");
    PrintProducts(products.GetAll());

    // READ
    Product? mouse = products.GetById(2);
    Console.WriteLine($"READ – GetById(2): {mouse?.Name}, {mouse?.Price:F2}");

    // UPDATE
    mouse!.Price = 1700m;
    products.Update(mouse);
    ctx.SaveChanges();
    Console.WriteLine($"UPDATE – новая цена товара «{mouse.Name}»: {products.GetById(2)!.Price:F2}");

    // DELETE
    products.Delete(4);
    ctx.SaveChanges();
    Console.WriteLine($"DELETE – товар #4 удалён, осталось товаров: {products.GetAll().Count()}");

    // Специализированные методы
    Console.WriteLine("Цена от 1000 до 20000:");
    PrintProducts(products.GetByPriceRange(1000m, 20000m));
    
    Console.WriteLine("Поиск по названию «Lenovo»:");
    PrintProducts(products.SearchByName("Lenovo"));
    
    Console.WriteLine("Нет на складе:");
    PrintProducts(products.GetOutOfStock());
}

Console.WriteLine();
Console.WriteLine("=== Задание 4. GenericRepository, async, Unit of Work ===");
using (var uow = new UnitOfWork(new OrderShopDbContext(options)))
{
    await uow.Customers.AddAsync(new Customer
    {
        FullName = "Петров Пётр",
        Email = "petrov@example.com",
        Phone = "+7 900 000-00-01"
    });
    
    await uow.Employees.AddAsync(new Employee
    {
        FullName = "Сидорова Анна",
        Position = "Менеджер",
        Email = "sidorova@shop.ru"
    });

    int saved = await uow.CompleteAsync();
    Console.WriteLine($"Асинхронно сохранено записей: {saved} (клиент и сотрудник - одной транзакцией)");

    Customer? customer = await uow.Customers.GetByIdAsync(1);
    Console.WriteLine($"GetByIdAsync(1): {customer?.FullName}, {customer?.Email}");
    Console.WriteLine($"GetByEmail: {uow.Customers.GetByEmail("petrov@example.com")?.FullName}");

    // Бизнес-логика (BLL) работает через IUnitOfWork и не знает о БД
    var discounts = new DiscountCalculator();
    var clock = new SystemClock();
    var orderService = new OrderService(uow, discounts, clock);
    Order order = orderService.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 2 }, { 2, 3 } });
    Console.WriteLine($"Заказ №{order.OrderId} создан: позиций - {order.Items.Count}, сумма - {order.TotalAmount:F2}");

    Order? loaded = uow.Orders.GetWithItems(order.OrderId);
    foreach (OrderItem item in loaded!.Items)
    {
        Console.WriteLine($"  - {item.Product!.Name} × {item.Quantity} = {item.UnitPrice * item.Quantity:F2}");
    }

    Console.WriteLine("Остатки после заказа:");
    PrintProducts(uow.Products.GetAll());

    IEnumerable<Order> allOrders = await uow.Orders.GetAllAsync();
    Console.WriteLine($"Всего заказов в базе: {allOrders.Count()}");

    // Проверка бизнес-правила
    try
    {
        orderService.PlaceOrder(1, 1, new Dictionary<int, int> { { 1, 100 } });
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine("Готово.");

// Вспомогательная функция вывода
static void PrintProducts(IEnumerable<Product> products)
{
    foreach (Product p in products)
        Console.WriteLine($"  #{p.ProductId} {p.Name} цена: {p.Price:F2} на складе: {p.StockQuantity}");
}