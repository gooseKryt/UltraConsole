using System.Collections;
using System.Drawing;
using UltraConsole.ANSI;

namespace UltraConsole.Data;

/// <summary>
/// Represents a formatted string with a <c>GraphicEffect</c> attached to every <c>char</c>
/// </summary>
public readonly struct FormatString : IEquatable<FormatString>, IEnumerable<(char c, GraphicEffect? colorPair)>, IEnumerable
{
    private FormatString(string value, GraphicEffect?[] graphic)
    {
        if (value.Length != graphic.Length)
            throw new ArgumentOutOfRangeException(nameof(graphic), "String and color array have different lengths");

        (this.value, this.graphic) = (value, graphic);
    }
    /// <summary>
    /// Creates a <c>ColorString</c> without color
    /// </summary>
    public FormatString(string value)
    {
        (this.value, graphic) = (value, new GraphicEffect?[value.Length]);
    }

    public readonly string value;
    /// <summary>
    /// Colors and effects of <c>ColorString</c>, where indexes of chars in string and colors in array are the same
    /// </summary>
    public readonly GraphicEffect?[] graphic;

    public int Length => value.Length;

    public (char c, GraphicEffect? colorPair) this[int index]
        => (value[index], graphic[index]);
    public override string ToString()
        => value;

    public static FormatString Create(string text, GraphicEffect? graphic)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray)
            => colorArray[0] = graphic);
    }
    public static FormatString Create(string text, Color? color)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray)
            => colorArray[0] = new(color));
    }
    /// <param name="foreground"><c>true</c> for FG color, otherwise <c>false</c></param>
    public static FormatString Create(string text, Color? color, bool foreground)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray)
            => colorArray[0] = new(
                foreground ? color : null,
                !foreground ? color : null));
    }
    public static FormatString Create(string text, GraphicsMode? effect)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray)
            => colorArray[0] = new(effect: effect));
    }

    public static FormatString Create(string text, GraphicEffect?[] graphic)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray) =>
        {
            for (int i = 0; i < graphic.Length && i < colorArray.Length; i++)
                colorArray[i] = graphic[i];
        });
    }
    public static FormatString Create(string text, Color?[] colors)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray) =>
        {
            for (int i = 0; i < colors.Length && i < colorArray.Length; i++)
                colorArray[i] = new(colors[i]);
        });
    }
    /// <param name="foreground"><c>true</c> for FG color, otherwise <c>false</c></param>
    public static FormatString Create(string text, Color?[] colors, bool foreground)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray) =>
        {
            for (int i = 0; i < colors.Length && i < colorArray.Length; i++)
                colorArray[i] = new(
                    foreground ? colors[i] : null,
                    !foreground ? colors[i] : null);
        });
    }
    public static FormatString Create(string text, GraphicsMode?[] effect)
    {
        return BaseCreate(text, (GraphicEffect?[] colorArray) =>
        {
            for (int i = 0; i < effect.Length && i < colorArray.Length; i++)
                colorArray[i] = new(effect: effect[i]);
        });
    }

    private static FormatString BaseCreate(string text, Action<GraphicEffect?[]> arrayInit)
    {
        GraphicEffect?[] colorArray = new GraphicEffect?[text.Length];
        arrayInit(colorArray);

        return new(text, colorArray);
    }

    public IEnumerator<(char c, GraphicEffect? colorPair)> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
            yield return this[i];
    }
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public override bool Equals(object? obj)
        => obj is FormatString str && Equals(str);
    public bool Equals(FormatString other)
        => value == other.value && graphic == other.graphic;
    public override int GetHashCode()
        => HashCode.Combine(value, graphic);

    public static bool operator ==(FormatString left, FormatString right)
        => left.Equals(right);
    public static bool operator !=(FormatString left, FormatString right)
        => !(left == right);

    public static FormatString operator +(FormatString left, FormatString right)
    {
        string str = left.value + right.value;
        GraphicEffect?[] color = new GraphicEffect?[left.graphic.Length + right.graphic.Length];
        left.graphic.CopyTo(color, 0);
        right.graphic.CopyTo(color, left.graphic.Length);

        return new(str, color);
    }
    public static FormatString operator +(FormatString left, string right)
    {
        string str = left.value + right;
        GraphicEffect?[] color = new GraphicEffect?[str.Length];
        left.graphic.CopyTo(color, 0);

        return new(str, color);
    }
    public static FormatString operator +(string left, FormatString right)
        => right + left;

    public static implicit operator string(FormatString colorString) => colorString.value;
    public static implicit operator FormatString(string str) => new(str);
}
