namespace OrderShop.Interfaces;

public interface IDevice
{
    void Print(string document);
    void Scan(string document);
    void Fax(string document);
}

public class LegacyPrinter : IDevice
{
    public void Print(string document) => Console.WriteLine($"[LegacyPrinter] Печать: {document}");
    public void Scan(string document) { /* не поддерживается */ }
    public void Fax(string document) { /* не поддерживается */ }
}

public class LegacyScanner : IDevice
{
    public void Print(string document) { /* не поддерживается */ }
    public void Scan(string document) => Console.WriteLine($"[LegacyScanner] Сканирование: {document}");
    public void Fax(string document) { /* не поддерживается */ }
}

public interface IPrinter { void Print(string document); }
public interface IScanner { void Scan(string document); }
public interface IFax { void Fax(string document); }

public class Printer : IPrinter
{
    public void Print(string document) => Console.WriteLine($"[Printer] Печать: {document}");
}

public class Scanner : IScanner
{
    public void Scan(string document) => Console.WriteLine($"[Scanner] Сканирование: {document}");
}

public class MultifunctionDevice : IPrinter, IScanner, IFax
{
    public void Print(string document) => Console.WriteLine($"[MFU] Печать: {document}");
    public void Scan(string document) => Console.WriteLine($"[MFU] Сканирование: {document}");
    public void Fax(string document) => Console.WriteLine($"[MFU] Отправка факса: {document}");
}