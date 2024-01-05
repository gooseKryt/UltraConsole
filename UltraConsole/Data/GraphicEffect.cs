using UltraConsole.ANSI;

namespace UltraConsole.Data;

/// <summary>
/// Represents a pair of foreground and background <c>Color</c>
/// </summary>
public struct GraphicEffect : IEquatable<GraphicEffect>
{
    public GraphicEffect(Color? foreground = null, Color? background = null, GraphicsMode? effect = null)
    {
        (this.foreground, this.background, this.effect) = (foreground, background, effect);
    }

    public Color? foreground, background;
    public GraphicsMode? effect;

    public override string ToString()
        => $"({foreground}, {background}, {effect})";

    public override bool Equals(object? obj)
        => obj is GraphicEffect pair && Equals(pair);
    public bool Equals(GraphicEffect other)
        => foreground.Equals(other.foreground)
        && background.Equals(other.background)
        && effect.Equals(other.effect);
    public override int GetHashCode()
        => HashCode.Combine(foreground, background, effect);

    public static bool operator ==(GraphicEffect left, GraphicEffect right)
        => left.Equals(right);
    public static bool operator !=(GraphicEffect left, GraphicEffect right)
        => !(left == right);

    public static explicit operator GraphicEffect(Color color)
        => new(color);
}
