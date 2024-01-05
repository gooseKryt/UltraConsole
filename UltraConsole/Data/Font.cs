namespace UltraConsole.Data;

/// <summary>
/// Represents either TrueType sizeless font OR Vector font with size
/// </summary>
public readonly struct Font
{
    private Font(string name)
    {
        this.name = name;
        (width, height) = (null, null);
    }
    private Font(short width, short height)
    {
        name = VectorFontName;
        (this.width, this.height) = (width, height);
    }

    public readonly string name;
    public readonly short? width, height;

    public bool IsVector
        => name == VectorFontName;

    private const string VectorFontName = "\0vector";

    public static Font B612_Mono
        => new("B612 Mono");

    public static Font Cascadia_Code
        => new("Cascadia Code");
    public static Font Cascadia_Code_ExtraLight
        => new("Cascadia Code ExtraLight");
    public static Font Cascadia_Code_Light
        => new("Cascadia Code Light");
    public static Font Cascadia_Code_SemiBold
        => new("Cascadia Code SemiBold");
    public static Font Cascadia_Code_SemiLight
        => new("Cascadia Code SemiLight");

    public static Font Cascadia_Mono
        => new("Cascadia Mono");
    public static Font Cascadia_Mono_ExtraLight
        => new("Cascadia Mono ExtraLight");
    public static Font Cascadia_Mono_Light
        => new("Cascadia Mono Light");
    public static Font Cascadia_Mono_SemiBold
        => new("Cascadia Mono SemiBold");
    public static Font Cascadia_Mono_SemiLight
        => new("Cascadia Mono SemiLight");

    public static Font Consolas
        => new("Consolas");

    public static Font Courier_New
        => new("Courier New");
    public static Font Courier_Prime_Sans
        => new("Courier Prime Sans");

    public static Font IBM_Plex_Mono
        => new("IBM Plex Mono");
    public static Font IBM_Plex_Mono_ExtraLight
        => new("IBM Plex Mono ExtraLight");
    public static Font IBM_Plex_Mono_Light
        => new("IBM Plex Mono Light");
    public static Font IBM_Plex_Mono_SemiBold
        => new("IBM Plex Mono SemiBold");
    public static Font IBM_Plex_Mono_SemiLight
        => new("IBM Plex Mono SemiLight");
    public static Font IBM_Plex_Mono_Thin
        => new("IBM Plex Mono Thin");

    public static Font Lucida_Console
        => new("Lucida Console");
    public static Font MPlus_1m_regular
        => new("M+ 1m regular");
    public static Font MS_Gothic
        => new("MS Gothic");
    public static Font MxPlus_Cordata_PPC_21
        => new("MxPlus Cordata PPC-21");
    public static Font MxPlus_Rainbow100_re_66
        => new("MxPlus Rainbow100 re.66");
    public static Font NSimSun
        => new("NSimSun");
    public static Font Perfect_DOS_VGA_437
        => new ("Perfect DOS VGA 437");
    public static Font PT_Mono
        => new("PT Mono");
    public static Font Simplified_Arabic_Fixed
        => new("Simplified Arabic Fixed");
    public static Font SimSun_ExtB
        => new("SimSun-ExtB");

    public static Font Source_Code_Pro
        => new("Source Code Pro");
    public static Font Source_Code_Pro_Black
        => new("Source Code Pro Black");
    public static Font Source_Code_Pro_ExtraLight
        => new("Source Code Pro ExtraLight");
    public static Font Source_Code_Pro_Light
        => new("Source Code Pro Light");
    public static Font Source_Code_Pro_Medium
        => new("Source Code Pro Medium");
    public static Font Source_Code_Pro_Semibold
        => new("Source Code Pro Semibold");

    public static Font Ubuntu_Mono
        => new("Ubuntu Mono");
    public static Font VT323
        => new("VT323");

    public static Font Vector(short width, short height)
        => new(width, height);

    public static implicit operator string(Font font)
        => font.name;
}
