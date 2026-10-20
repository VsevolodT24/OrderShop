using OrderShop.Core.Enums;

namespace OrderShop.Core.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    public int OrderId { get; set; }
    public Order? Order { get; set; }
}