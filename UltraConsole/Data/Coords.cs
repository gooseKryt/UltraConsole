namespace UltraConsole.Data;

/// <summary>
/// Represents 2D int coordinates
/// </summary>
public struct Coords : IEquatable<Coords>
{
    public Coords(int x, int y)
    {
        (this.x, this.y) = (x, y);
    }
    public Coords() : this(0, 0) { }

    public int x { get; set; }
    public int y { get; set; }

    public override string ToString()
        => $"({x}, {y})";

    public override bool Equals(object? obj)
        => obj is Coords coords && Equals(coords);
    public bool Equals(Coords other)
        => x == other.x && y == other.y;
    public override int GetHashCode()
        => HashCode.Combine(x, y);

    public static bool operator ==(Coords left, Coords right)
        => left.Equals(right);
    public static bool operator !=(Coords left, Coords right)
        => !(left == right);

    public static implicit operator (int x, int y)(Coords coords)
        => (coords.x, coords.y);
    public static implicit operator Coords((int x, int y) coords)
        => new(coords.x, coords.y);

    public static explicit operator string(Coords coords)
        => coords.ToString();
}
