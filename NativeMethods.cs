using System;
using System.Runtime.InteropServices;

namespace WindowsDefenderBypass
{
    /// <summary>
    /// Consolidated P/Invoke declarations for native Windows API calls.
    /// </summary>
    internal static class NativeMethods
    {
        public const uint PAGE_EXECUTE_READWRITE = 0x40;

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string name);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    }
}
