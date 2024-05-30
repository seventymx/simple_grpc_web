{
  description = "A development environment for working with dotnet 8 and grpc/grpc-web";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
    powershell_modules = {
      url = "github:seventymx/powershell_modules";
      flake = false;
    };
  };

  outputs = { self, nixpkgs, flake-utils, powershell_modules }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        unstable = import nixpkgs {
          inherit system;
        };
      in
      {
        devShell = unstable.mkShell {
          buildInputs = [
            unstable.git
            unstable.dotnet-sdk_8
            unstable.powershell
            unstable.nodejs_20
            unstable.yarn
            unstable.protobuf_21
            unstable.protoc-gen-js
            unstable.protoc-gen-grpc-web
          ];

          shellHook = ''
            export SHELL="${unstable.powershell}/bin/pwsh"

            export PSModulePath=${powershell_modules}

            pwsh -NoExit -Command "& {
              Import-Module GrpcWebGenerator

              Import-Module NugetResolver
              Import-Module ResourceEnumGenerator

              Import-Module VersionUpdater
            }"

            exit 0
          '';
        };
      }
    );
}