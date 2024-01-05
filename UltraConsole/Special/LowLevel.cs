using System.Runtime.InteropServices;

namespace UltraConsole.Special;

// Written with help of ChatGPT ;]

/// <summary>
/// Class that handles low level operations, such as P/Invokes
/// </summary>
public static class LowLevel
{
    private const int MF_BYCOMMAND = 0x00000000;
    private const int SC_MINIMIZE = 0xF020;
    private const int SC_MAXIMIZE = 0xF030;
    private const int SC_SIZE = 0xF000;

    private const int STD_OUTPUT_HANDLE = -11;
    private const int TMPF_TRUETYPE = 4;
    private const int TMPF_VECTOR = 0;

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleFontInfo(IntPtr hConsoleOutput, bool bMaximumWindow, uint dwFontCount, [Out] CONSOLE_FONT_INFO[] lpConsoleFontInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

    [StructLayout(LayoutKind.Sequential)]
    private struct COORD
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CONSOLE_FONT_INFO
    {
        public int nFont;
        public COORD dwFontSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CONSOLE_FONT_INFO_EX
    {
        public int cbSize;
        public int nFont;
        public COORD dwFontSize;
        public int FontFamily;
        public int FontWeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }


    public static (int hr_minimize, int hr_maximize, int hr_size) LockWindowSize()
    {
        int min = DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
        int max = DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
        int size = DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);

        return (min, max, size);
    }

    public static bool SetFont(string name, short size, int weight = 0)
    {
        return SetFontBase(new()
        {
            cbSize = Marshal.SizeOf(typeof(CONSOLE_FONT_INFO_EX)),

            dwFontSize = new()
            {
                X = 0,
                Y = size
            },
            FontFamily = TMPF_TRUETYPE,
            FaceName = name,
            FontWeight = weight
        });
    }
    public static bool SetVectorFont(short width, short height, int weight = 0)
    {
        return SetFontBase(new()
        {
            cbSize = Marshal.SizeOf(typeof(CONSOLE_FONT_INFO_EX)),

            dwFontSize = new()
            {
                X = width,
                Y = height
            },
            FontFamily = TMPF_VECTOR,
            FontWeight = weight
        });
    }
    private static bool SetFontBase(CONSOLE_FONT_INFO_EX newFont)
    {
        IntPtr hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);

        CONSOLE_FONT_INFO[] consoleFontInfo = new CONSOLE_FONT_INFO[1];
        GetConsoleFontInfo(hConsoleOutput, false, 1, consoleFontInfo);

        return SetCurrentConsoleFontEx(hConsoleOutput, false, newFont);
    }
}
