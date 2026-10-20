namespace OrderShop.Interfaces;

public interface IPayable
{
    void Pay(decimal amount);
}

public class CreditCard : IPayable
{
    private readonly string _cardNumber;

    public CreditCard(string cardNumber) => _cardNumber = cardNumber;

    public void Pay(decimal amount)
    {
        string masked = "**** **** **** " + _cardNumber[^4..];
        Console.WriteLine($"Оплата картой {masked}: {amount:F2} руб.");
    }
}

public class Cash : IPayable
{
    public void Pay(decimal amount) => Console.WriteLine($"Оплата наличными: {amount:F2} руб.");
}

public static class PaymentProcessor
{
    public static void ProcessPayment(IPayable method, decimal amount)
    {
        Console.WriteLine("Обработка платежа...");
        method.Pay(amount);
    }
}

public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[Console] {message}");
}

public class FileLogger : ILogger
{
    private readonly string _path;

    public FileLogger(string path) => _path = path;

    public void Log(string message)
    {
        File.AppendAllText(_path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
    }
}

public static class Worker
{
    public static void DoWork(ILogger logger)
    {
        logger.Log("Работа началась");
        logger.Log("Выполняется обработка данных");
        logger.Log("Работа завершена");
    }
}