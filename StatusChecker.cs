using System;
using System.Management.Automation;

namespace WindowsDefenderBypass
{
    public static class StatusChecker
    {
        public static void ShowStatus()
        {
            Console.WriteLine("\n=== Windows Defender Status ===\n");

            PowerShellExecutor.ExecuteWithFallback(
                ps => ShowStatusViaRunspace(ps),
                () => ShowStatusFallback()
            );

            Console.WriteLine();
        }

        private static void ShowStatusViaRunspace(PowerShell ps)
        {
            var result = PowerShellExecutor.ExecuteCommandWithResults(ps,
                "Get-MpPreference | Select-Object DisableRealtimeMonitoring, DisableBehaviorMonitoring, DisableIOAVProtection, DisableScriptScanning, DisableCloudProtection, DisableNetworkProtection");

            if (result.Count > 0)
            {
                var prefs = result[0].Properties;
                Console.WriteLine("Defender Preferences:");
                Console.WriteLine($"  Real-Time Monitoring:      {(prefs["DisableRealtimeMonitoring"]?.Value.ToString() == "True" ? "DISABLED" : "ENABLED")}");
                Console.WriteLine($"  Behavior Monitoring:       {(prefs["DisableBehaviorMonitoring"]?.Value.ToString() == "True" ? "DISABLED" : "ENABLED")}");
                Console.WriteLine($"  IOAV Protection:           {(prefs["DisableIOAVProtection"]?.Value.ToString() == "True" ? "DISABLED" : "ENABLED")}");
                Console.WriteLine($"  Script Scanning:           {(prefs["DisableScriptScanning"]?.Value.ToString() == "True" ? "DISABLED" : "ENABLED")}");
                Console.WriteLine($"  Cloud Protection:          {(prefs["DisableCloudProtection"]?.Value.ToString() == "True" ? "DISABLED" : "ENABLED")}");
                Console.WriteLine($"  Network Protection:        {(prefs["DisableNetworkProtection"]?.Value.ToString() == "True" ? "DISABLED" : "ENABLED")}");
            }

            result = PowerShellExecutor.ExecuteCommandWithResults(ps,
                "Get-MpComputerStatus | Select-Object RealTimeProtectionEnabled, AntivirusEnabled, AntispywareEnabled");

            if (result.Count > 0)
            {
                var status = result[0].Properties;
                Console.WriteLine("\nDefender Service Status:");
                Console.WriteLine($"  Real-Time Protection:      {(status["RealTimeProtectionEnabled"]?.Value.ToString() == "True" ? "ENABLED" : "DISABLED")}");
                Console.WriteLine($"  Antivirus:                 {(status["AntivirusEnabled"]?.Value.ToString() == "True" ? "ENABLED" : "DISABLED")}");
                Console.WriteLine($"  Antispyware:               {(status["AntispywareEnabled"]?.Value.ToString() == "True" ? "ENABLED" : "DISABLED")}");
            }

            result = PowerShellExecutor.ExecuteCommandWithResults(ps, "(Get-MpPreference).ExclusionPath.Count");
            if (result.Count > 0)
            {
                Console.WriteLine($"\nExclusion Paths: {result[0]}");
            }
        }

        private static void ShowStatusFallback()
        {
            Console.WriteLine("[*] Using fallback status check method...");
            Console.WriteLine("Run the following PowerShell command for detailed status:");
            Console.WriteLine("  Get-MpPreference | Format-List");
            Console.WriteLine("  Get-MpComputerStatus | Format-List");
        }
    }
}

