using OrderShop.BLL.Abstractions;

namespace OrderShop.BLL.Infrastructure;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}