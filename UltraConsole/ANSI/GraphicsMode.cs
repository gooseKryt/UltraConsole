namespace UltraConsole.ANSI;

/// <summary>
/// ANSI graphics mode
/// </summary>
public enum GraphicsMode
{
    Reset = 0,
    Bold = 1,
    [Obsolete("Not supported in windows terminal")] Faint = 2,
    [Obsolete("Not supported in windows terminal")] Italic = 3,
    Underline = 4,
    [Obsolete("Not supported in windows terminal")] Blinking = 5,
    Inverse = 7,
    [Obsolete("Not supported in windows terminal")] Hidden = 8,
    [Obsolete("Not supported in windows terminal")] Strikethrough = 9
}
