using OrderShop.Core.Enums;

namespace OrderShop.BLL.Abstractions;

public sealed class PaymentResult
{
    public PaymentResult(bool success, string? error)
    {
        Success = success;
        Error = error;
    }

    public bool Success { get; }
    public string? Error { get; }
}

public interface IPaymentGateway
{
    PaymentResult Charge(decimal amount, PaymentMethod method);
}