using System;
using System.Diagnostics;
using System.Security.Principal;

namespace WindowsDefenderBypass
{
    public static class SecurityHelper
    {
        public static bool IsUserAnAdmin()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        public static void RequestAdminElevation()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    FileName = Environment.GetCommandLineArgs()[0],
                    UseShellExecute = true
                };

                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    string args = string.Join(" ", Environment.GetCommandLineArgs(), 1, Environment.GetCommandLineArgs().Length - 1);
                    startInfo.Arguments = args;
                }

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Failed to request elevation: {ex.Message}");
            }
        }
    }
}
