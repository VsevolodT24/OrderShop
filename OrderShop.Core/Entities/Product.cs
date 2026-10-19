namespace OrderShop.Core.Entities;

public class Product
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}