namespace OrderShop.Interfaces;

public interface IShape
{
    double GetArea();
    double GetPerimeter();
}

public interface I3DShape : IShape
{
    double GetVolume();
}

public class Circle : IDrawable, IShape
{
    public double Radius { get; }

    public Circle(double radius) => Radius = radius;

    public void Draw() => Console.WriteLine($"Рисую круг, радиус = {Radius:F2}");
    public double GetArea() => Math.PI * Radius * Radius;
    public double GetPerimeter() => 2 * Math.PI * Radius;
}

public class Rectangle : IDrawable, IShape
{
    public double Width { get; }
    public double Height { get; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public void Draw() => Console.WriteLine($"Рисую прямоугольник {Width:F2} x {Height:F2}");
    public double GetArea() => Width * Height;
    public double GetPerimeter() => 2 * (Width + Height);
}

public class Cube : I3DShape
{
    public double Side { get; }

    public Cube(double side) => Side = side;

    public double GetArea() => 6 * Side * Side;
    public double GetPerimeter() => 12 * Side;
    public double GetVolume() => Side * Side * Side;
}

public static class ShapePrinter
{
    public static void PrintShapeInfo(IShape shape)
    {
        Console.WriteLine($"{shape.GetType().Name}: площадь = {shape.GetArea():F2}, периметр = {shape.GetPerimeter():F2}");
        if (shape is I3DShape d3)
        {
            Console.WriteLine($"{shape.GetType().Name}: объём = {d3.GetVolume():F2}");
        }
    }
}