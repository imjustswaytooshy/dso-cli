# Variables
$githubUser = "imjustprism"
$githubRepo = "psang"
$installDir = "C:\Program Files\psang"
$apiUrl = "https://api.github.com/repos/$githubUser/$githubRepo/releases/latest"
$tempFile = Join-Path $env:TEMP "psang.exe"
$userAgent = "psang-installer"

# Function to display colored messages
function Write-Message {
    param (
        [string]$Message,
        [ConsoleColor]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# Get latest release
try {
    $release = Invoke-RestMethod -Uri $apiUrl -Headers @{ "User-Agent" = $userAgent }
} catch {
    Write-Message "Error: Unable to get release info." -Color Red
    exit 1
}

# Find psang.exe asset
$asset = $release.assets | Where-Object { $_.name -eq "psang.exe" } | Select-Object -First 1
if (-not $asset) {
    Write-Message "Error: 'psang.exe' not found in assets." -Color Red
    exit 1
}

# Download psang.exe
try {
    Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $tempFile -Headers @{ "User-Agent" = $userAgent }
} catch {
    Write-Message "Error: Download failed." -Color Red
    exit 1
}

try {
    if (-not (Test-Path $installDir)) {
        New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    }
} catch {
    Write-Message "Error: Cannot create directory." -Color Red
    exit 1
}

# Check if psang.exe is running
if (Get-Process -Name "psang" -ErrorAction SilentlyContinue) {
    Write-Message "Please close 'psang.exe' before updating." -Color Yellow
    exit 1
}

# Copy psang.exe to install directory
try {
    Copy-Item -Path $tempFile -Destination $installDir -Force
} catch {
    Write-Message "Error: Installation failed." -Color Red
    exit 1
}

try {
    $envPath = [System.Environment]::GetEnvironmentVariable("Path", "Machine")
    if ($envPath -notlike "*$installDir*") {
        [System.Environment]::SetEnvironmentVariable("Path", "$envPath;$installDir", "Machine")
    } else {
        Write-Message "Path already includes installation directory." -Color Yellow
    }
} catch {
    Write-Message "Warning: Could not update PATH." -Color Yellow
}

# Clean up
try {
    Remove-Item -Path $tempFile -Force
} catch {
    Write-Message "Warning: Temporary file not removed." -Color Yellow
}

# Completion Message
Write-Message "Installation complete! Restart your terminal." -Color Cyan
