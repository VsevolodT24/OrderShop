namespace OrderShop.BLL.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}