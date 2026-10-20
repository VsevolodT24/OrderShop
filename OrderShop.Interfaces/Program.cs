using System.Text;
using OrderShop.Interfaces;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== Задание 1. Базовые интерфейсы ===");
IMovable point = new Point(1, 2);
Console.WriteLine($"Точка до перемещения: {point}");
point.Move(3, 4);
Console.WriteLine($"Точка после Move(3, 4): {point}");

var drawables = new List<IDrawable> { new Circle(5), new Rectangle(4, 6) };
Drawer.DrawAll(drawables);
Console.WriteLine();

Console.WriteLine("=== Задание 2. Интерфейсы и наследование ===");
ShapePrinter.PrintShapeInfo(new Circle(5));
ShapePrinter.PrintShapeInfo(new Rectangle(4, 6));
ShapePrinter.PrintShapeInfo(new Cube(3));
Console.WriteLine();

Console.WriteLine("=== Задание 3. Сегрегация интерфейсов (ISP) ===");
Console.WriteLine("--- «Толстый» интерфейс IDevice (нарушение ISP) ---");
IDevice legacyPrinter = new LegacyPrinter();
legacyPrinter.Print("report.pdf");
Console.WriteLine("(LegacyPrinter.Scan() — пустой метод, ничего не вывел)");
legacyPrinter.Scan("report.pdf");

IDevice legacyScanner = new LegacyScanner();
legacyScanner.Scan("photo.jpg");

Console.WriteLine("--- Разделённые интерфейсы IPrinter / IScanner / IFax ---");
IPrinter printer = new Printer();
printer.Print("report.pdf");

IScanner scanner = new Scanner();
scanner.Scan("photo.jpg");

var mfu = new MultifunctionDevice();
mfu.Print("contract.docx");
mfu.Scan("passport.png");
mfu.Fax("order.pdf");
Console.WriteLine();

Console.WriteLine("=== Задание 4. Интерфейсы и полиморфизм ===");
PaymentProcessor.ProcessPayment(new CreditCard("1234567812345678"), 1500.50m);
PaymentProcessor.ProcessPayment(new Cash(), 300.00m);
Console.WriteLine();

var fileLogPath = "app.log";
if (File.Exists(fileLogPath)) File.Delete(fileLogPath);

Worker.DoWork(new ConsoleLogger());
Worker.DoWork(new FileLogger(fileLogPath));

Console.WriteLine($"Сообщения FileLogger записаны в файл: {fileLogPath}");
Console.WriteLine("Содержимое файла:");
foreach (var line in File.ReadAllLines(fileLogPath))
{
    Console.WriteLine(line);
}