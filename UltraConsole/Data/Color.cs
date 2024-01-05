namespace UltraConsole.Data;

/// <summary>
/// Color in 24-bit space
/// </summary>
public struct Color : IEquatable<Color>
{
    public Color(byte r, byte g, byte b)
    {
        (this.r, this.g, this.b) = (r, g, b);
    }
    public Color() : this(0, 0, 0) { }

    public byte r, g, b;

    private static readonly Dictionary<ConsoleColor, Color> ConsoleColors = new()
    {
        { ConsoleColor.Black, new(12,12,12) },
        { ConsoleColor.DarkRed, new(197,15,31) },
        { ConsoleColor.DarkGreen, new(19,161,14) },
        { ConsoleColor.DarkYellow, new(193,156,0) },
        { ConsoleColor.DarkBlue, new(0,55,218) },
        { ConsoleColor.DarkMagenta, new(136,23,152) },
        { ConsoleColor.DarkCyan, new(58,150,221) },
        { ConsoleColor.Gray, new(204,204,204) },
        { ConsoleColor.DarkGray, new(118,118,118) },
        { ConsoleColor.Red, new(231,72,86) },
        { ConsoleColor.Green, new(22,198,12) },
        { ConsoleColor.Yellow, new(249,241,165) },
        { ConsoleColor.Blue, new(59,120,255) },
        { ConsoleColor.Magenta, new(180,0,158) },
        { ConsoleColor.Cyan, new(97,214,214) },
        { ConsoleColor.White, new(242,242,242) }
    };
    public static Color FromConsoleColor(ConsoleColor color)
        => ConsoleColors[color];

    public override string ToString()
        => $"({r}, {g}, {b})";

    public override bool Equals(object? obj)
        => obj is Color color && Equals(color);
    public bool Equals(Color other)
        => r == other.r
        && g == other.g
        && b == other.b;
    public override int GetHashCode()
        => HashCode.Combine(r, g, b);

    public static byte Add(byte left, byte right)
    {
        try
        {
            return checked((byte)(left + right));
        }
        catch (OverflowException)
        {
            return byte.MaxValue;
        }
    }
    public static byte Sub(byte left, byte right)
    {
        try
        {
            return checked((byte)(left - right));
        }
        catch (OverflowException)
        {
            return byte.MinValue;
        }
    }

    public static bool operator ==(Color left, Color right)
        => left.Equals(right);
    public static bool operator !=(Color left, Color right)
        => !(left == right);

    public static Color operator +(Color left, Color right)
        => new(
            Add(left.r, right.r),
            Add(left.g, right.g),
            Add(left.b, right.b));
    public static Color operator -(Color left, Color right)
        => new(
            Sub(left.r, right.r),
            Sub(left.g, right.g),
            Sub(left.b, right.b));

    public static explicit operator string(Color color)
        => color.ToString();
}
