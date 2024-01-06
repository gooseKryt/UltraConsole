using UltraConsole.Data;

namespace UltraConsole.ANSI;

/// <summary>
/// ANSI escape codes and sequences
/// </summary>
public static class EscapeSequence
{
    public const char EscapeChar = '\u001b';
    public const string Escape = "\u001b[";
    public const char Separator = ';';

    public const char GraphicsModeEnd = 'm';
    public const int ColorModeForeground = 38;
    public const int ColorModeBackground = 48;
    public const int ColorMode256 = 5;
    public const int ColorModeRGB = 2;

    public const string ResetColor = "\u001b[0;39;49m";
    public const string ResetEffects = "\u001b[0;0;0m";

    public static string GraphicsMode(GraphicEffect? graphic)
    {
        return graphic == null
            ? ""
            : ColorMode(graphic.Value.foreground, graphic.Value.background)
            + GraphicsMode(graphic.Value.effect);
    }

    public static string ColorMode(Color? foreground = null, Color? background = null)
    {
        return ForegroundColorMode(foreground)
            + BackgroundColorMode(background);
    }
    public static string ForegroundColorMode(Color? color)
    {
        return color == null
            ? ""
            : Create(ColorModeForeground, ColorModeRGB,
            color.Value.r, color.Value.g, color.Value.b);
    }
    public static string BackgroundColorMode(Color? color)
    {
        return color == null
            ? ""
            : Create(ColorModeBackground, ColorModeRGB,
            color.Value.r, color.Value.g, color.Value.b);
    }

    public static string GraphicsMode(GraphicsMode? mode)
    {
        return mode == null ? "" : Create((int)mode);
    }

    /// <summary>
    /// Creates sequence with graphics mode sequence syntax
    /// </summary>
    public static string Create(params string[] args)
    {
        return Escape + string.Join(Separator, args) + GraphicsModeEnd;
    }
    /// <summary>
    /// Creates sequence with graphics mode sequence syntax
    /// </summary>
    public static string Create(params int[] args)
    {
        return Escape + string.Join(Separator, args) + GraphicsModeEnd;
    }
}
