namespace OrderShop.Core.Entities;

public class Product
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Category? Category { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}