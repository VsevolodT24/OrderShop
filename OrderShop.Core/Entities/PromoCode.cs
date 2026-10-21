namespace OrderShop.Core.Entities;

public class PromoCode
{
    public int PromoCodeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public DateTime ValidUntil { get; set; }
}