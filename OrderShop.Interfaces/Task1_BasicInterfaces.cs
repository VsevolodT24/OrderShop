namespace OrderShop.Interfaces;

public interface IMovable
{
    void Move(int x, int y);
}

public interface IDrawable
{
    void Draw();
}

public class Point : IMovable
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Move(int x, int y)
    {
        X += x;
        Y += y;
    }

    public override string ToString() => $"({X}; {Y})";
}

public static class Drawer
{
    public static void DrawAll(List<IDrawable> shapes)
    {
        foreach (IDrawable shape in shapes)
        {
            shape.Draw();
        }
    }
}