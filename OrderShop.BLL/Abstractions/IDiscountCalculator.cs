using OrderShop.Core.Entities;

namespace OrderShop.BLL.Abstractions;

public interface IDiscountCalculator
{
    decimal Calculate(decimal subtotal, PromoCode? promo, DateTime now);
}