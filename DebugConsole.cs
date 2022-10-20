// Debug Console
// Copyright (c) 2022 konome
// MIT License

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Konome
{
    public static class DebugConsole
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [Conditional("DEBUG")]
        public static void Create(int x = 0, int y = 0, int w = 0, int h = 0)
            => Create(true, x, y, w, h);

        public static void Create(bool debug, int x = 0, int y = 0, int w = 0, int h = 0)
        {
            if (debug)
            {
                int flag = w == 0 && h == 0 ? 0x1 : 0x40;
                AllocConsole();
                SetWindowPos(GetConsoleWindow(), 0, x, y, w, h, flag);

                Trace.Listeners.Add(new ConsoleTraceListener());
                Debug.WriteLine("Debugging...");
            }
        }

        [Conditional("DEBUG")]
        public static void Close() => Close(true);

        public static void Close(bool debug)
        {
            if (debug)
                FreeConsole();
        }
    }
}