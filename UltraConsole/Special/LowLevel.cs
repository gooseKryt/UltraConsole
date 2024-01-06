using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

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

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadConsoleOutput(IntPtr hConsoleOutput, IntPtr lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpReadRegion);


    [StructLayout(LayoutKind.Sequential)]
    private struct CHAR_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] charData;
        public short attributes;
    }

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

    [StructLayout(LayoutKind.Sequential)]
    private struct SMALL_RECT
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CONSOLE_SCREEN_BUFFER_INFO
    {
        public COORD dwSize;
        public COORD dwCursorPosition;
        public short wAttributes;
        public SMALL_RECT srWindow;
        public COORD dwMaximumWindowSize;
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

#pragma warning disable
    public static IEnumerable<string> ReadFromBuffer(short x, short y, short width, short height)
    {
        IntPtr buffer = Marshal.AllocHGlobal(width * height * Marshal.SizeOf(typeof(CHAR_INFO)));
        if (buffer == null)
            throw new OutOfMemoryException();

        try
        {
            COORD coord = new();
            SMALL_RECT rc = new()
            {
                Left = x,
                Top = y,
                Right = (short)(x + width - 1),
                Bottom = (short)(y + height - 1)
            };

            COORD size = new()
            {
                X = width,
                Y = height
            };

            if (!ReadConsoleOutput(GetStdHandle(STD_OUTPUT_HANDLE), buffer, size, coord, ref rc))
            {
                // 'Not enough storage is available to process this command' may be raised for buffer size > 64K (see ReadConsoleOutput doc.)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IntPtr ptr = buffer;
            for (int h = 0; h < height; h++)
            {
                StringBuilder sb = new();
                for (int w = 0; w < width; w++)
                {
                    CHAR_INFO ci = (CHAR_INFO)Marshal.PtrToStructure(ptr, typeof(CHAR_INFO));
                    char[] chars = Console.OutputEncoding.GetChars(ci.charData);
                    sb.Append(chars[0]);
                    ptr += Marshal.SizeOf(typeof(CHAR_INFO));
                }
                yield return sb.ToString();
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
#pragma warning restore
}
