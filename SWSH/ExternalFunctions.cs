using System;
using System.Runtime.InteropServices;

namespace SWSH
{
    class ExternalFunctions
    {
        private const string Kernel32 = "kernel32.dll";
        [DllImport(Kernel32, EntryPoint = "SetConsoleMode", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport(Kernel32, EntryPoint = "GetConsoleMode", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport(Kernel32, EntryPoint = "GetStdHandle", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int handle);
    }
}
