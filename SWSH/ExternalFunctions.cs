/*
 *  SWSH - Secure Windows Shell
 *  Copyright (C) 2017  Muhammad Muzzammil
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

namespace SWSH {
    class ExternalFunctions {
        private const string Kernel32 = "kernel32.dll";
        [DllImport(Kernel32, EntryPoint = "SetConsoleMode", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport(Kernel32, EntryPoint = "GetConsoleMode", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport(Kernel32, EntryPoint = "GetStdHandle", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int handle);
    }
}