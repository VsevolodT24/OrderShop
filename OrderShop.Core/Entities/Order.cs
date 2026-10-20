using OrderShop.Core.Enums;

namespace OrderShop.Core.Entities;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public decimal TotalAmount { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Delivery? Delivery { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}