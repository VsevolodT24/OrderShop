using OrderShop.BLL.Services;
using OrderShop.Core.Entities;
using Xunit;

namespace OrderShop.BLL.Tests;

public class DiscountCalculatorTests
{
    private static readonly DateTime Now = new(2026, 10, 21, 12, 0, 0, DateTimeKind.Utc);
    private readonly DiscountCalculator _calc = new();

    private static PromoCode Promo(decimal percent, DateTime validUntil) =>
        new() { PromoCodeId = 1, Code = "TEST", DiscountPercent = percent, ValidUntil = validUntil };

    [Fact]
    public void Calculate_NegativeSubtotal_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calc.Calculate(-1m, null, Now));
    }

    [Fact]
    public void Calculate_NoPromo_ReturnsZero()
    {
        Assert.Equal(0m, _calc.Calculate(1000m, null, Now));
    }

    [Fact]
    public void Calculate_ExpiredPromo_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _calc.Calculate(1000m, Promo(10m, Now.AddSeconds(-1)), Now));
        Assert.Contains("истёк", ex.Message);
    }

    [Fact]
    public void Calculate_PromoValidUntilNow_IsStillValid()
    {
        Assert.Equal(100m, _calc.Calculate(1000m, Promo(10m, Now), Now));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(100.5)]
    [InlineData(150)]
    public void Calculate_InvalidPercent_Throws(double percent)
    {
        Assert.Throws<InvalidOperationException>(() => _calc.Calculate(1000m, Promo((decimal)percent, Now.AddDays(1)), Now));
    }

    [Theory]
    [InlineData(1000, 10, 100)]
    [InlineData(999.99, 10, 100.00)]
    [InlineData(2500, 100, 2500)]
    [InlineData(0, 50, 0)]
    public void Calculate_ValidPromo_ReturnsRoundedDiscount(double subtotal, double percent, double expected)
    {
        decimal result = _calc.Calculate((decimal)subtotal, Promo((decimal)percent, Now.AddDays(1)), Now);
        Assert.Equal((decimal)expected, result);
    }
}