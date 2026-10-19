namespace OrderShop.Core.Entities;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Новый";
    public decimal TotalAmount { get; set; }
}