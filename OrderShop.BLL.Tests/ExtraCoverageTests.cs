using OrderShop.BLL.Infrastructure;
using OrderShop.Core.Enums;
using Xunit;

namespace OrderShop.BLL.Tests;

public class ExtraCoverageTests
{
    // 1. Покрываем SystemClock
    [Fact]
    public void SystemClock_ReturnsCurrentTime()
    {
        var clock = new SystemClock();
        Assert.True((DateTime.UtcNow - clock.UtcNow).TotalSeconds < 5);
    }

    // 2. Покрываем OfflinePaymentGateway (на случай, если прошлые тесты не зачлись)
    [Fact]
    public void OfflinePaymentGateway_CashOverLimit_ReturnsFalse()
    {
        var gateway = new OfflinePaymentGateway();
        var result = gateway.Charge(OfflinePaymentGateway.MaxCashAmount + 1m, PaymentMethod.Cash);
        Assert.False(result.Success);
    }
    [Fact]
    public void OfflinePaymentGateway_CardOverLimit_IsAccepted()
    {
        var gateway = new OfflinePaymentGateway();
        var result = gateway.Charge(OfflinePaymentGateway.MaxCashAmount + 1m, PaymentMethod.Card);
     Assert.True(result.Success);
    }
    [Fact]
    public void OfflinePaymentGateway_CardOverCashLimit_IsAccepted()
    {
        var gateway = new OfflinePaymentGateway();
        // Сумма больше лимита (100001), но оплата картой (не наличные)
        var result = gateway.Charge(OfflinePaymentGateway.MaxCashAmount + 1m, PaymentMethod.Card);
    
        Assert.True(result.Success);
        Assert.Null(result.Error);
    }
}