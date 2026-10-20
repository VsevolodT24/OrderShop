namespace OrderShop.Core.Entities;

public class Delivery
{
    public int DeliveryId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Carrier { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? DeliveredAt { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }
}