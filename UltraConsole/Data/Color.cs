namespace UltraConsole.Data;

/// <summary>
/// Color in 24-bit space
/// </summary>
public struct Color : IEquatable<Color>
{
    /// <summary>
    /// Initializes new <c>Color</c> from byte channels
    /// </summary>
    public Color(byte r, byte g, byte b)
    {
        (this.r, this.g, this.b) = (r, g, b);
    }
    /// <summary>
    /// Initializes new <c>Color</c> from normalized channels (0-1)
    /// </summary>
    public Color(float r, float g, float b)
    {
        this.r = (byte)(Math.Clamp(r, 0f, 1f) * byte.MaxValue);
        this.g = (byte)(Math.Clamp(g, 0f, 1f) * byte.MaxValue);
        this.b = (byte)(Math.Clamp(b, 0f, 1f) * byte.MaxValue);
    }
    public Color() : this(0, 0, 0) { }

    public byte r, g, b;
    /// <summary>
    /// Represents color channels normalized to floats between 0-1
    /// </summary>
    public (float r, float g, float b) Normal
    {
        get => (
            r / (float)byte.MaxValue,
            g / (float)byte.MaxValue,
            b / (float)byte.MaxValue);
        set
        {
            try
            {
                r = checked((byte)(value.r * byte.MaxValue));
                g = checked((byte)(value.g * byte.MaxValue));
                b = checked((byte)(value.b * byte.MaxValue));
            }
            catch (OverflowException e)
            {
                throw new OverflowException("Normalized color channels are out of 0-1 range", e);
            }
        }
    }

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

    /// <summary>
    /// Creates gradient
    /// </summary>
    /// <param name="from">First color of gradient</param>
    /// <param name="to">Last color of gradient</param>
    /// <param name="size">Size of gradient</param>
    public static Color[] Gradient(Color from, Color to, int size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size should be at least 1");

        Color[] gradient = new Color[size];

        (float[] r, float[] g, float[] b) arrays = (
            LinearArray(from.Normal.r, to.Normal.r, size),
            LinearArray(from.Normal.g, to.Normal.g, size),
            LinearArray(from.Normal.b, to.Normal.b, size));

        for (int i = 0; i < size; i++)
        {
            gradient[i] = new(arrays.r[i], arrays.g[i], arrays.b[i]);
        }

        return gradient;
    }
    private static float[] LinearArray(float from, float to, int size)
    {
        float step = (to - from) / (size - 1f);
        return Enumerable.Range(0, size)
            .Select(i => from + i * step)
            .ToArray();
    }

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
    {
        return new(
            Add(left.r, right.r),
            Add(left.g, right.g),
            Add(left.b, right.b));
    }
    public static Color operator -(Color left, Color right)
    {
        return new(
            Sub(left.r, right.r),
            Sub(left.g, right.g),
            Sub(left.b, right.b));
    }
    public static Color operator *(Color left, Color right)
    {
        return new(
            left.Normal.r * right.Normal.r,
            left.Normal.g * right.Normal.g,
            left.Normal.b * right.Normal.b);
    }

    public static implicit operator Color(ConsoleColor consoleColor)
        => FromConsoleColor(consoleColor);

    public static explicit operator string(Color color)
        => color.ToString();
}
