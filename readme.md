# How to run the project on Windows or WSL2

## TODO

-   Create a flake.nix file to setup the environment for the project.

## Installation Instructions

### Prerequisites

You can run the following commands to check the versions of the required tools:

```bash
pwsh -File "./scripts/CheckPrerequisites.ps1"
```

Make sure all tools are installed on your machine and are available in your PATH.

This project requires specific versions of .NET, Node.js, PowerShell, and the Protobuf compiler to be installed. Below are the instructions for setting up your environment on both Linux (specifically for Debian-based distributions such as Ubuntu) and Windows.

#### Trust the development certificate

I've generated a development certificate for the web server, because gRPC-Web requires HTTPS. And dotnet dev-certs --trust command is not available on Linux.
Trust the certificate on your host windows and WSL2 using the following commands:

**Linux:**

```bash
sudo cp cert/localhost.crt /usr/local/share/ca-certificates/localhost.crt
sudo update-ca-certificates
```

WSL2 is not required, just trust the certificate on your host machine. And ignore the linux instructions.

**Windows:**

```powershell
Import-Certificate -FilePath .\cert\localhost.crt -CertStoreLocation Cert:\LocalMachine\Root
```

You may need to run PowerShell as an administrator for the command to succeed. After importing the certificate, you might need to restart your browser to ensure the changes take effect.

**Note**: The development certificate is only for development purposes. It is not recommended to use it in a production environment.
Replace the certificate with a valid one if you are deploying the application to a production environment.

### Windows

I recommend you to use [Chocolatey](https://chocolatey.org/install#individual), a package manager for Windows, for the installations.

To install the required tools on Windows, you will need to start the terminal as an administrator.

**Install .NET SDK 8.0:**

-   Download and install directly from the [official .NET website](https://dotnet.microsoft.com/download/dotnet/8.0).

**Install Node.js:**

```powershell
choco install nodejs
```

**Install PowerShell:**

```powershell
choco install powershell-core

Set-ExecutionPolicy RemoteSigned
```

**Install the Protobuf compiler:**

```powershell
choco install protoc
```

**Install the Javascript plugin:**

-   The plugin can be downloaded from the [official GitHub repository](https://github.com/protocolbuffers/protobuf-javascript/releases). After downloading, rename the file to `protoc-gen-js.exe` and ensure it's accessible in your PATH.

**Install the gRPC-Web plugin:**

-   The plugin can be downloaded from the [official GitHub repository](https://github.com/grpc/grpc-web/releases). After downloading, rename the file to `protoc-gen-grpc-web.exe` and ensure it's accessible in your PATH.

### Common Steps

**Note**: The following steps are common for both Linux and Windows.

Run the following command in the `frontend` directory to **generate the gRPC-Web client code**:

You need to have yarn installed to run the following command. If you don't have yarn installed, you can install it using the following command:

```bash
npm install --global yarn

yarn generate
```

You can start the development server using the following commands:

**Backend:**

```bash
cd ./backend/SPTrialWebServiceCS

dotnet clean

dotnet run
```

**Frontend:**

```bash
cd ./frontend

yarn install

yarn start
```

## Project Structure

### Generate dotnet solution

If you don't need a solution file and just build the project, you can skip this step.

To generate the dotnet solution, run the following command in the root directory of the project:

```bash
pwsh -File "./scripts/MakeSolution.ps1" -directoryPath "./backend"

pwsh -File "./scripts/MakeSolution.ps1" -directoryPath . -filter "^(SP7).*(\.csproj)$"

dotnet sln backend.sln add ./backend/SP7LogicVB/SPTrialCommonCS.csproj
```

This script will generate a dotnet solution in the specified directory and add all projects to it.

### Versioning in the Project

When the solution is cleaned or rebuilt, a PowerShell script automatically updates all project versions. The version is defined in `./versionconfig.json`, with the following structure:

```json
{
    "version": {
        "major": 1,
        "minor": 2,
        "patch": 2
    }
}
```
