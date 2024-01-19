using UltraConsole.Data;
using UltraConsole.ANSI;

using Encoding = System.Text.Encoding;
using SysConsole = System.Console;
using Seq = UltraConsole.ANSI.EscapeSequence;
using Low = UltraConsole.Special.LowLevel;

namespace UltraConsole;

/// <summary>
/// Class with basic UltraConsole features
/// </summary>
public static class UConsole
{
    public static TextWriter Out => SysConsole.Out;
    public static TextReader In => SysConsole.In;

    public static string Title
    {
        get => SysConsole.Title;
        set => SysConsole.Title = value;
    }

    public static class Graphics
    {
        public static Color DefaultFG { get; set; }
            = Color.FromConsoleColor(SysConsole.ForegroundColor);
        public static Color DefaultBG { get; set; }
            = Color.FromConsoleColor(SysConsole.BackgroundColor);

        public static Font DefaultFont { get; set; } = Font.Consolas;
        public static short DefaultFontSize { get; set; } = 16;

        private static Color foregroundColor = DefaultFG;
        public static Color ForegroundColor
        {
            get => foregroundColor;
            set
            {
                foregroundColor = value;
                Write(Seq.ForegroundColorMode(foregroundColor));
            }
        }
        private static Color backgroundColor = DefaultBG;
        public static Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                Write(Seq.BackgroundColorMode(backgroundColor));
            }
        }

        private static GraphicsMode textEffect = GraphicsMode.Reset;
        public static GraphicsMode TextEffect
        {
            get => textEffect;
            set
            {
                textEffect = value;
                Write(Seq.GraphicsMode(textEffect));
            }
        }

        public static GraphicEffect GraphicEffect
        {
            get => new(foregroundColor, backgroundColor, textEffect);
            set
            {
                if (value.foreground != null)
                    ForegroundColor = (Color)value.foreground;
                if (value.background != null)
                    BackgroundColor = (Color)value.background;
                if (value.effect != null)
                    TextEffect = (GraphicsMode)value.effect;
            }
        }

        private static Font font;
        public static Font Font
        {
            get => font;
            set
            {
                font = value;
                Low.SetFont(font, fontSize);
            }
        }
        private static short fontSize;
        public static short FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                Low.SetFont(font, fontSize);
            }
        }

        public static Coords WindowSize
        {
            get => new(SysConsole.WindowWidth, SysConsole.WindowHeight);
            set
            {
                SysConsole.SetWindowSize(value.x, value.y);
                SysConsole.SetBufferSize(value.x, value.y);
            }
        }
    }

    public static class Cursor
    {
        private static Coords pos = new(SysConsole.CursorLeft, SysConsole.CursorTop);
        public static Coords Pos
        {
            get => pos;
            set
            {
                pos = value;
                SysConsole.SetCursorPosition(pos.x, pos.y);
            }
        }

        public static bool Visible
        {
            get => SysConsole.CursorVisible;
            set => SysConsole.CursorVisible = value;
        }

        public static void Reset()
        {
            Pos = (0, 0);
        }
    }

    /// <summary>
    /// Initializes console
    /// </summary>
    /// <param name="title">Window title</param>
    /// <param name="size">Window size</param>
    /// <param name="graphicEffect">Sets the current and default foreground and background colors</param>
    /// <param name="font">Sets the current and default font</param>
    /// <param name="fontSize">Sets the current and default font size</param>
    /// <param name="inEncoding">Input encoding</param>
    /// <param name="outEncoding">Output encoding</param>
    public static void Init(string? title = null, Coords? size = null, bool cursorVisible = false, GraphicEffect? graphicEffect = null, Font? font = null, short? fontSize = null, Encoding? inEncoding = null, Encoding? outEncoding = null)
    {
        if (title != null) SysConsole.Title = title;

        if (graphicEffect != null)
        {
            GraphicEffect graphic = (GraphicEffect)graphicEffect;

            if (graphic.foreground != null)
                Graphics.DefaultFG = (Color)graphic.foreground;
            if (graphic.background != null)
                Graphics.DefaultBG = (Color)graphic.background;
        }
        if (font != null) Graphics.DefaultFont = (Font)font;
        if (fontSize != null) Graphics.DefaultFontSize = (short)fontSize;

        SetFont(Graphics.DefaultFont, Graphics.DefaultFontSize);
        if (size != null) Graphics.WindowSize = (Coords)size;
        Low.LockWindowSize();

        SysConsole.InputEncoding = inEncoding ?? Encoding.UTF8;
        SysConsole.OutputEncoding = outEncoding ?? Encoding.Unicode;

        SysConsole.CursorVisible = cursorVisible;

        Graphics.ForegroundColor = Graphics.DefaultFG;
        Graphics.BackgroundColor = Graphics.DefaultBG;
    }

    /// <summary>
    /// Sets the font, able to proccess both TrueType and Vector
    /// </summary>
    /// <param name="size">TrueType font size, defaults to <c>Graphics.DefaultFontSize</c>, ignored in Vector</param>
    public static void SetFont(Font font, short? size = null)
    {
        if (font.IsVector)
            Low.SetVectorFont((short)font.width!, (short)font.height!);
        else
            Low.SetFont(font, size ?? Graphics.DefaultFontSize);
    }

    public static void Write(string? value)
        => SysConsole.Write(value);
    public static void Write(char? value)
        => SysConsole.Write(value);
    public static void Write(object? value)
        => SysConsole.Write(value);

    public static void WriteLine(string? value)
        => SysConsole.WriteLine(value);
    public static void WriteLine(char? value)
        => SysConsole.WriteLine(value);
    public static void WriteLine(object? value)
        => SysConsole.WriteLine(value);
    public static void WriteLine()
        => SysConsole.WriteLine();

    public static void Write(FormatString value)
    {
        foreach((char c, GraphicEffect? colorPair) in value)
        {
            Write(Seq.GraphicsMode(colorPair) + c);
        }
        Write(Seq.GraphicsMode(GraphicsMode.Reset));
    }
    public static void WriteLine(FormatString value)
        => Write(value + "\n");

    /// <summary>
    /// Creates a FormatString and writes it
    /// </summary>
    public static void Write(string value, GraphicEffect graphic)
        => Write(FormatString.Create(value, graphic));
    /// <summary>
    /// Creates a FormatString and writes it
    /// </summary>
    public static void Write(string value, Color color)
        => Write(FormatString.Create(value, color));
    /// <summary>
    /// Creates a FormatString and writes it
    /// </summary>
    public static void Write(string value, GraphicsMode effect)
        => Write(FormatString.Create(value, effect));
    public static void Write(string value, ConsoleColor color)
        => Write(FormatString.Create(value, Color.FromConsoleColor(color)));
    /// <summary>
    /// Creates a FormatString and writes it
    /// </summary>
    public static void WriteLine(string value, GraphicEffect graphic)
        => Write(FormatString.Create(value + "\n", graphic));
    /// <summary>
    /// Creates a FormatString and writes it
    /// </summary>
    public static void WriteLine(string value, Color color)
        => Write(FormatString.Create(value + "\n", color));
    /// <summary>
    /// Creates a FormatString and writes it
    /// </summary>
    public static void WriteLine(string value, GraphicsMode effect)
        => Write(FormatString.Create(value + "\n", effect));
    public static void WriteLine(string value, ConsoleColor color)
        => Write(FormatString.Create(value + "\n", Color.FromConsoleColor(color)));

    /// <summary>
    /// Writes character at position
    /// </summary>
    [Obsolete($"To quickly write use {nameof(WriteBuffer)} instead")]
    public static void WriteAt(char value, Coords pos)
    {
        Cursor.Pos = pos;
        Write(value);
    }
    /// <summary>
    /// Writes text at position
    /// </summary>
    [Obsolete($"To quickly write use {nameof(WriteBuffer)} instead")]
    public static void WriteAt(string value, Coords pos)
    {
        Cursor.Pos = pos;
        Write(value);
    }
    /// <summary>
    /// Writes text at position
    /// </summary>
    /// <param name="returnCursor"><c>true</c> to return cursor to it's starting position</param>
    [Obsolete($"To quickly write use {nameof(WriteBuffer)} instead")]
    public static void WriteAt(string value, Coords pos, bool returnCursor)
    {
        if (returnCursor)
        {
            Coords revertPos = Cursor.Pos;
            WriteAt(value, pos);
            Cursor.Pos = revertPos;
        }
        else
        {
            WriteAt(value, pos);
            Cursor.Pos = pos;
        }
    }
    /// <summary>
    /// Writes text at position
    /// </summary>
    public static void WriteAt(FormatString value, Coords pos)
    {
        Cursor.Pos = pos;
        Write(value);
    }
    /// <summary>
    /// Writes text at position
    /// </summary>
    /// <param name="returnCursor"><c>true</c> to return cursor to it's starting position</param>
    public static void WriteAt(FormatString value, Coords pos, bool returnCursor)
    {
        if (returnCursor)
        {
            Coords revertPos = Cursor.Pos;
            WriteAt(value, pos);
            Cursor.Pos = revertPos;
        }
        else
        {
            WriteAt(value, pos);
            Cursor.Pos = pos;
        }
    }
    /// <summary>
    /// Writes text at position
    /// </summary>
    public static void WriteAt(string value, Coords pos, GraphicEffect graphic)
    {
        Cursor.Pos = pos;
        Write(value, graphic);
    }

    /// <summary>
    /// Writes directly to buffer
    /// </summary>
    public static void WriteBuffer(string value, Coords pos)
    {
        checked
        {
            Low.WriteToBuffer(value, (short)pos.x, (short)pos.y);
        }
    }
    /// <summary>
    /// Writes directly to buffer
    /// </summary>
    public static void WriteBuffer(char value, Coords pos)
    {
        checked
        {
            Low.WriteToBuffer(value, (short)pos.x, (short)pos.y);
        }
    }

    /// <summary>
    /// Reads a part of console buffer
    /// </summary>
    /// <param name="pos">Reading starting position</param>
    /// <param name="size">Size of part being read</param>
    public static IEnumerable<string> ReadBuffer(Coords pos, Coords size)
    {
        checked
        {
            return Low.ReadFromBuffer(
            (short)pos.x, (short)pos.y,
            (short)size.x, (short)size.y);
        }
    }
    public static string ReadBufferLine(Coords pos, int length)
        => ReadBuffer(pos, (length, 1)).First();
    public static char ReadBufferChar(Coords pos)
        => ReadBufferLine(pos, 1)[0];

    /// <summary>
    /// Overrides <c>GraphicEffect</c> at <c>pos</c> without changing text
    /// </summary>
    public static void GraphicOverride(GraphicEffect graphic, Coords pos)
    {
        WriteAt(ReadBufferChar(pos).ToString(), pos, graphic);
    }
    /// <summary>
    /// Overrides color at <c>pos</c> without changing text
    /// </summary>
    public static void ColorOverride(Color color, Coords pos)
        => GraphicOverride(new(color), pos);
    /// <summary>
    /// Overrides color at <c>pos</c> without changing text
    /// </summary>
    /// <param name="foreground"><c>true</c> for FG color, otherwise <c>false</c></param>
    public static void ColorOverride(Color color, Coords pos, bool foreground)
        => GraphicOverride(new(
            foreground ? color : null,
            !foreground ? color : null),
            pos);

    public static string? ReadLine()
    {
        SysConsole.CursorVisible = true;
        string? text = SysConsole.ReadLine();
        SysConsole.CursorVisible = false;
        return text;
    }
    public static ConsoleKeyInfo ReadKey(bool displayInput = false)
        => SysConsole.ReadKey(!displayInput);

    /// <summary>
    /// Clears the console buffer
    /// </summary>
    public static void Clear()
        => SysConsole.Clear();
    /// <summary>
    /// Resets foreground and background color to defaults
    /// </summary>
    public static void ResetColor()
    {
        Graphics.ForegroundColor = Graphics.DefaultFG;
        Graphics.BackgroundColor = Graphics.DefaultBG;
    }
    /// <summary>
    /// Resets color and text effect
    /// </summary>
    public static void ResetGraphics()
    {
        ResetColor();
        Graphics.TextEffect = GraphicsMode.Reset;
    }
    /// <summary>
    /// Fully resets graphic settings
    /// </summary>
    /// <param name="resetFont"><c>true</c> to also reset font and font size</param>
    public static void ResetGraphics(bool resetFont)
    {
        ResetGraphics();
        if (resetFont)
        {
            Graphics.Font = Graphics.DefaultFont;
            Graphics.FontSize = Graphics.DefaultFontSize;
        }
    }
}
