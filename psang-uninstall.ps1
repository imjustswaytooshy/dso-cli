# psang-uninstall.ps1
# Description: Uninstalls Psang from the system by removing the executable, deleting the installation directory,
#              and removing the installation path from the system PATH environment variable.

# Ensure the script is run as Administrator
function Test-Administrator {
    $currentUser = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    return $currentUser.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (-not (Test-Administrator)) {
    Write-Error "This script must be run as an Administrator. Please right-click the script and select 'Run as Administrator'."
    exit 1
}

# Define installation directory and executable path
$installDir = "C:\Program Files\psang"
$exePath = Join-Path $installDir "psang.exe"

# Function to terminate running Psang processes
function Terminate-PsangProcesses {
    $processName = "psang"
    $processes = Get-Process -Name $processName -ErrorAction SilentlyContinue

    if ($processes) {
        foreach ($proc in $processes) {
            try {
                Stop-Process -Id $proc.Id -Force
            }
            catch {
                Write-Warning "Failed to terminate process $($proc.ProcessName) (ID: $($proc.Id)): $_"
            }
        }
    }
    else {
        # No running Psang processes found
    }
}

# Function to remove installation directory and executable
function Remove-PsangFiles {
    if (Test-Path -Path $exePath) {
        try {
            Remove-Item -Path $exePath -Force
        }
        catch {
            Write-Error "Failed to delete Psang executable: $_"
        }
    }
    else {
        # Psang executable not found
    }

    if (Test-Path -Path $installDir) {
        try {
            # Check if directory is empty
            if (-not (Get-ChildItem -Path $installDir -Recurse -Force)) {
                Remove-Item -Path $installDir -Force -Recurse
            }
            else {
            }
        }
        catch {
            Write-Error "Failed to delete installation directory: $_"
        }
    }
    else {
        # Installation directory not found
    }
}

# Function to remove installation directory from system PATH
function Remove-PsangFromPath {
    $pathToRemove = $installDir
    $envTargets = @("Machine", "User")

    foreach ($target in $envTargets) {
        try {
            $currentPath = [Environment]::GetEnvironmentVariable("Path", $target)
            if (-not [string]::IsNullOrEmpty($currentPath) -and $currentPath -like "*$pathToRemove*") {
                $paths = $currentPath.Split(';') | Where-Object { $_ -ne $pathToRemove }
                $newPath = ($paths -join ';').TrimEnd(';')
                [Environment]::SetEnvironmentVariable("Path", $newPath, $target)
            }
            else {
                # Path not found
            }
        }
        catch {
            Write-Error "Failed to modify $target PATH: $_"
        }
    }
}

# Confirm uninstallation
Write-Host "Are you sure you want to uninstall Psang? (Y/N)" -ForegroundColor Yellow
$confirmation = Read-Host

if ($confirmation -notmatch '^[Yy]$') {
    Write-Host "Uninstallation aborted by user." -ForegroundColor Cyan
    exit 0
}

# Execute uninstallation steps
Terminate-PsangProcesses
Remove-PsangFiles
Remove-PsangFromPath

Write-Host "Psang has been successfully uninstalled from your system." -ForegroundColor Green
Write-Host "Please restart your computer to ensure all changes take effect." -ForegroundColor Cyan
