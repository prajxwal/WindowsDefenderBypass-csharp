@echo off
echo Building Windows Defender Bypass Tool...
echo.

REM Check if MSBuild is available
where msbuild >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo MSBuild not found in PATH. Trying Visual Studio Developer Command Prompt...
    REM Try to find Visual Studio MSBuild
    for /f "usebackq tokens=*" %%i in (`where /r "C:\Program Files" msbuild.exe 2^>nul`) do (
        set MSBUILD=%%i
        goto :found
    )
    echo [!] MSBuild not found. Please install Visual Studio or .NET SDK.
    pause
    exit /b 1
)
:found

echo Using MSBuild: %MSBUILD%
echo.

REM Build Release configuration
%MSBUILD% WindowsDefenderBypass.csproj /p:Configuration=Release /p:Platform=AnyCPU /t:Rebuild

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [+] Build successful!
    echo Output: bin\Release\net48\WindowsDefenderBypass.exe
) else (
    echo.
    echo [!] Build failed!
    pause
    exit /b 1
)

pause

