using OrderShop.BLL.Abstractions;
using OrderShop.Core.Enums;

namespace OrderShop.BLL.Infrastructure;

public class OfflinePaymentGateway : IPaymentGateway
{
    public static readonly decimal MaxCashAmount = 100000m;

    public PaymentResult Charge(decimal amount, PaymentMethod method)
    {
        if (amount <= 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма платежа должна быть больше нуля.");

        if (amount > MaxCashAmount && method == PaymentMethod.Cash)
            return new PaymentResult(false, "Превышен лимит оплаты наличными.");

        return new PaymentResult(true, null);
    }
}