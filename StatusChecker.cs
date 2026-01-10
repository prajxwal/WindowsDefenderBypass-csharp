using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace WindowsDefenderBypass
{
    public static class StatusChecker
    {
        public static void ShowStatus()
        {
            Console.WriteLine("\n=== Windows Defender Status ===\n");

            bool useRunspace = false;
            try
            {
                var testRunspace = RunspaceFactory.CreateRunspace();
                testRunspace.Dispose();
                useRunspace = true;
            }
            catch
            {
                useRunspace = false;
            }

            if (useRunspace)
            {
                try
                {
                    using (Runspace runspace = RunspaceFactory.CreateRunspace())
                    {
                        runspace.Open();
                        using (PowerShell ps = PowerShell.Create())
                        {
                            ps.Runspace = runspace;

                        ps.AddScript("Get-MpPreference | Select-Object DisableRealtimeMonitoring, DisableBehaviorMonitoring, DisableIOAVProtection, DisableScriptScanning, DisableCloudProtection, DisableNetworkProtection");
                        var result = ps.Invoke();

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

                        ps.Commands.Clear();

                        ps.AddScript("Get-MpComputerStatus | Select-Object RealTimeProtectionEnabled, AntivirusEnabled, AntispywareEnabled");
                        result = ps.Invoke();

                        if (result.Count > 0)
                        {
                            var status = result[0].Properties;
                            Console.WriteLine("\nDefender Service Status:");
                            Console.WriteLine($"  Real-Time Protection:      {(status["RealTimeProtectionEnabled"]?.Value.ToString() == "True" ? "ENABLED" : "DISABLED")}");
                            Console.WriteLine($"  Antivirus:                 {(status["AntivirusEnabled"]?.Value.ToString() == "True" ? "ENABLED" : "DISABLED")}");
                            Console.WriteLine($"  Antispyware:               {(status["AntispywareEnabled"]?.Value.ToString() == "True" ? "ENABLED" : "DISABLED")}");
                        }

                        ps.Commands.Clear();

                        ps.AddScript("(Get-MpPreference).ExclusionPath.Count");
                        result = ps.Invoke();
                        if (result.Count > 0)
                        {
                            Console.WriteLine($"\nExclusion Paths: {result[0].ToString()}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Error checking status: {ex.Message}");
                    ShowStatusFallback();
                }
            }
            else
            {
                ShowStatusFallback();
            }

            Console.WriteLine();
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
