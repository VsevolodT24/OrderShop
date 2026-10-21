using OrderShop.BLL.Abstractions;
using OrderShop.Core.Entities;

namespace OrderShop.BLL.Services;

public class DiscountCalculator : IDiscountCalculator
{
    public decimal Calculate(decimal subtotal, PromoCode? promo, DateTime now)
    {
        if (subtotal < 0m)
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Сумма заказа не может быть отрицательной.");

        if (promo is null)
            return 0m;

        if (promo.ValidUntil < now)
            throw new InvalidOperationException("Срок действия промокода истёк.");

        if (promo.DiscountPercent <= 0m || promo.DiscountPercent > 100m)
            throw new InvalidOperationException("Размер скидки промокода должен быть в диапазоне от 0 до 100 процентов.");

        return Math.Round(subtotal * promo.DiscountPercent / 100m, 2, MidpointRounding.AwayFromZero);
    }
}