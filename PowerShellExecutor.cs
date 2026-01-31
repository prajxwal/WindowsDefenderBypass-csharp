using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace WindowsDefenderBypass
{
    /// <summary>
    /// Provides a unified way to execute PowerShell commands with automatic fallback.
    /// </summary>
    public static class PowerShellExecutor
    {
        /// <summary>
        /// Attempts to execute PowerShell using a Runspace. Falls back to process-based execution if Runspace is unavailable.
        /// </summary>
        /// <param name="runspaceAction">Action to execute with PowerShell when Runspace is available.</param>
        /// <param name="fallbackAction">Action to execute when Runspace is unavailable.</param>
        /// <returns>True if Runspace was used, false if fallback was used.</returns>
        public static bool ExecuteWithFallback(Action<PowerShell> runspaceAction, Action fallbackAction)
        {
            bool useRunspace = CanUseRunspace();

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
                            runspaceAction(ps);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [!] Error in PowerShell runspace: {ex.Message}");
                    fallbackAction();
                    return false;
                }
            }
            else
            {
                fallbackAction();
                return false;
            }
        }

        /// <summary>
        /// Checks if PowerShell Runspace is available on this system.
        /// </summary>
        public static bool CanUseRunspace()
        {
            try
            {
                var testRunspace = RunspaceFactory.CreateRunspace();
                testRunspace.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executes a single PowerShell command and optionally reports errors.
        /// </summary>
        /// <param name="ps">The PowerShell instance to use.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="reportErrors">Whether to print errors to console.</param>
        public static void ExecuteCommand(PowerShell ps, string command, bool reportErrors = true)
        {
            ps.Commands.Clear();
            ps.AddScript(command);
            ps.Invoke();

            if (ps.HadErrors && reportErrors)
            {
                foreach (ErrorRecord error in ps.Streams.Error)
                {
                    Console.WriteLine($"    [!] Warning: {error.Exception.Message}");
                }
            }
        }

        /// <summary>
        /// Executes a PowerShell command and returns the results.
        /// </summary>
        /// <param name="ps">The PowerShell instance to use.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>Collection of PSObject results.</returns>
        public static System.Collections.ObjectModel.Collection<PSObject> ExecuteCommandWithResults(PowerShell ps, string command)
        {
            ps.Commands.Clear();
            ps.AddScript(command);
            return ps.Invoke();
        }
    }
}
