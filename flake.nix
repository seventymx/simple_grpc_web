{
  description = "A development environment to work with ASP.NET Core gRPC services and React.";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
      self,
      flake-utils,
      nixpkgs,
      ...
    }@inputs:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        unstable = import nixpkgs { inherit system; };

        buildDependencies = [
          unstable.which
          unstable.rsync
          unstable.openssl_legacy
          unstable.git
          unstable.nixfmt-rfc-style
          unstable.powershell
          unstable.dotnet-sdk_8
          unstable.nodejs_20
          unstable.yarn
          unstable.protobuf_21
          unstable.protoc-gen-js
          unstable.protoc-gen-grpc-web
        ];

        # certificateSettings is a JSON string that contains the path to the certificate (without the extension) and the password for the pfx file.
        certificateSettings = ''
          {
            "path": "cert/localhost",
            "password": "fancyspy10"
          }
        '';
      in
      {
        devShell = unstable.mkShell {
          buildInputs = buildDependencies;

          shellHook = ''
            # Set the shell to PowerShell - vscode will use this shell
            export SHELL="${unstable.powershell}/bin/pwsh"

            export PSModulePath="$(realpath "$(pwd)/powershell_modules")"

            export BACKEND_PORT=5001
            export FRONTEND_PORT=5010

            # Set the base URL for the services (used by the frontend)
            export SERVICE_BASE_ADDRESS="https://localhost:$BACKEND_PORT/"

            export CERTIFICATE_SETTINGS='${certificateSettings}'

            # Enter PowerShell and import the necessary modules
            pwsh -NoExit -Command "& {
              Import-Module GrpcWebGenerator

              Import-Module NugetResolver
              Import-Module ResourceEnumGenerator
            }"

            # Exit when PowerShell exits
            exit 0
          '';
        };
      }
    );
}
