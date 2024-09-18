# .\InitWindowsEnv.ps1


# Set the PSModulePath to include a custom 'powershell_modules' directory
$env:PSModulePath = "$(Resolve-Path .)\powershell_modules"

# Import necessary modules
Import-Module GrpcWebGenerator
Import-Module NugetResolver
Import-Module ResourceEnumGenerator

# Set environment variables
$env:BACKEND_PORT = 5001
$env:FRONTEND_PORT = 5010

# Set certificate settings (JSON format as a string)
$env:CERTIFICATE_SETTINGS = '{"path": "cert/localhost", "password": "fancyspy10"}'

# Set the service base address using the backend port
$env:SERVICE_BASE_ADDRESS = "https://localhost:$($env:BACKEND_PORT)/"

# Print confirmation that the environment is set up
Write-Host "Environment initialized with the following settings:"
Write-Host "PSModulePath: $env:PSModulePath"
Write-Host "BACKEND_PORT: $env:BACKEND_PORT"
Write-Host "FRONTEND_PORT: $env:FRONTEND_PORT"
Write-Host "CERTIFICATE_SETTINGS: $env:CERTIFICATE_SETTINGS"
Write-Host "SERVICE_BASE_ADDRESS: $env:SERVICE_BASE_ADDRESS"