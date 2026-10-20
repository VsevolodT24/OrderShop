namespace OrderShop.Core.Entities;

public class Supplier
{
    public int SupplierId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}