# Windows Defender Bypass Tool (C#)

## Description
An advanced Windows Defender bypass tool designed for educational purposes and security research. This tool demonstrates various techniques to disable Windows Defender protections through registry modifications, PowerShell commands, AMSI bypass, ETW bypass, and exclusion management.

**⚠️ Disclaimer**: This program is distributed for educational purposes only. Modifying system settings and disabling security features should only be done responsibly and with appropriate permissions. This tool is intended for use in controlled environments such as security research labs, penetration testing, and authorized security assessments.

## Features

### Core Functionality
- **Registry-based Bypass**: Modifies Windows Defender registry keys to disable various protection mechanisms
- **PowerShell-based Configuration**: Uses PowerShell cmdlets to disable Defender preferences
- **AMSI Bypass**: Attempts to bypass Anti-Malware Scan Interface (AMSI) protection
- **ETW Bypass**: Attempts to bypass Event Tracing for Windows (ETW) telemetry
- **Exclusion Management**: Automatically adds common exclusion paths, processes, and file extensions
- **Status Checking**: Verify current Windows Defender configuration state
- **Restore Functionality**: Re-enable Windows Defender settings (useful for cleanup)

### Disabled Defender Settings
- Real-Time Monitoring
- Behavior Monitoring
- Block at First Seen
- IOAV Protection
- Script Scanning
- Cloud Protection
- Network Protection
- Archive Scanning
- Email Scanning
- Removable Drive Scanning
- Sample Submission
- MAPS Reporting
- Threat Default Actions (set to Allow for all threat levels)

### Advanced Features
- **Modular Architecture**: Clean separation of concerns with dedicated classes
- **Error Handling**: Comprehensive error handling and logging
- **Command-Line Interface**: Multiple operation modes (bypass, status, restore)
- **Automatic Elevation**: Requests administrator privileges automatically

## Requirements
- Windows 10/11 or Windows Server 2016+
- .NET Framework 4.8 or later
- Administrator privileges
- PowerShell 5.1 or later

## Installation

### Option 1: Visual Studio
1. Clone the repository:
   ```bash
   git clone https://github.com/prajxwal/WindowsDefenderBypass-c-.git
   cd WindowsDefenderBypass-csharp
   ```
2. Open `WindowsDefenderBypass.sln` in Visual Studio
3. Build the solution (Ctrl+Shift+B)
4. Run with administrator privileges

### Option 2: Command Line
```bash
# Clone the repository
git clone https://github.com/prajxwal/WindowsDefenderBypass-c-.git
cd WindowsDefenderBypass-csharp

# Build using MSBuild or dotnet CLI
dotnet build WindowsDefenderBypass.csproj -c Release

# Or using MSBuild
msbuild WindowsDefenderBypass.csproj /p:Configuration=Release
```

## Usage

### Basic Usage
Run the executable with administrator privileges:
```bash
WindowsDefenderBypass.exe
```

The tool will:
1. Check for administrator privileges (request elevation if needed)
2. Attempt AMSI bypass
3. Attempt ETW bypass
4. Modify registry settings
5. Disable Defender via PowerShell
6. Add common exclusions

### Command-Line Options

#### Check Defender Status
```bash
WindowsDefenderBypass.exe --status
# or
WindowsDefenderBypass.exe -s
```
Displays current Windows Defender configuration and protection status.

#### Restore Defender Settings
```bash
WindowsDefenderBypass.exe --restore
# or
WindowsDefenderBypass.exe -r
```
Re-enables all Windows Defender protections and removes exclusions.

#### Show Help
```bash
WindowsDefenderBypass.exe --help
# or
WindowsDefenderBypass.exe -h
```

## Architecture

The project is organized into modular components:

- **Program.cs**: Main entry point and CLI interface
- **DefenderManager.cs**: PowerShell-based Defender configuration management
- **RegistryManager.cs**: Registry modification operations
- **AMSIBypass.cs**: Anti-Malware Scan Interface bypass implementation
- **ETWBypass.cs**: Event Tracing for Windows bypass implementation
- **ExclusionManager.cs**: Exclusion path, process, and extension management
- **StatusChecker.cs**: Defender status verification
- **SecurityHelper.cs**: Security and privilege management utilities
- **PowerShellHelper.cs**: PowerShell execution utilities

## Technical Details

### AMSI Bypass
The tool attempts to patch the `AmsiScanBuffer` function in `amsi.dll` to return `AMSI_RESULT_NOT_DETECTED`, effectively bypassing AMSI scanning for PowerShell scripts.

### ETW Bypass
The tool patches the `EtwEventWrite` function in `ntdll.dll` to prevent Event Tracing for Windows telemetry from being sent.

### Registry Modifications
- `HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\DisableAntiSpyware`
- `HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\*`
- `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WinDefend\Start`

### Exclusion Paths
Automatically adds exclusions for:
- User profile directories
- Application data folders
- Temporary directories
- Common system paths

## Security Considerations

⚠️ **Important Notes:**
- This tool disables critical security features. Use only in isolated test environments
- Some bypass techniques may be detected by advanced security solutions
- Registry modifications may be logged by Windows Event Viewer
- Group Policy may override registry settings in domain environments
- Windows Defender may automatically re-enable some protections after a reboot

## Troubleshooting

### "Access Denied" Errors
- Ensure you're running with administrator privileges
- Check if Group Policy is preventing registry modifications
- Verify UAC is not blocking the operation

### PowerShell Execution Errors
- Ensure PowerShell execution policy allows script execution
- Check if PowerShell is available in the system PATH
- Verify System.Management.Automation.dll is accessible

### AMSI/ETW Bypass Failures
- These bypasses may fail on newer Windows versions with enhanced protections
- Some security solutions may prevent memory patching
- The tool will continue execution even if bypasses fail

## Contributing

This is an educational project. Contributions that improve code quality, documentation, or add legitimate educational features are welcome.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Disclaimer

**FOR EDUCATIONAL AND AUTHORIZED TESTING PURPOSES ONLY**

This software is provided for educational purposes to demonstrate security concepts. The authors and contributors are not responsible for any misuse of this software. Users are responsible for ensuring they have proper authorization before using this tool on any system.

Use of this tool on systems without explicit authorization may violate laws and regulations. Always obtain proper authorization before conducting security testing.

## Acknowledgments

Developed for educational security research and authorized penetration testing scenarios.
