# Developer setup

This project uses .NET 9 for services and Node 20.4 (via Volta) for the UI. The steps below install common tooling and get you running quickly.

## 1) Install required tools with winget

Run in PowerShell:

```pwsh
winget import --import-file .\developer_setup\winget-config.json --disable-interactivity
```

What this installs (highlights):
- .NET 9 SDK, Visual Studio Code
- Visual Studio 2022 Professional (with workloads, on first launch)
- Volta (Node tool manager) to pin Node 20.4.0
- Git, Git LFS, PowerShell, Windows Terminal
- Browsers, Postman
- DBeaver (DB client)

Note: SQL Server tooling was intentionally removed. We use Oracle.

## 2) Finalize CLI tooling

```pwsh
# Pin Node.js to the version specified by Volta in ui/package.json
volta install node@LTS

# Optional but recommended: enable Git LFS in your repo
git lfs install

# EF Core tooling (global)
dotnet tool update --global dotnet-ef

# Install git hooks (pre-commit hook runs dotnet format on staged C# files)
.\scripts\Install-GitHooks.ps1
```

## 3) Restore and build

Services (.NET):
```pwsh
# Ensure .NET 9 SDK is active
dotnet --version
# Open solution in VS (recommended) or build from CLI
# d:\source\Demoulas\smart-profit-sharing\src\services
# dotnet build Demoulas.ProfitSharing.slnx
```

UI (Vite + React):
```pwsh
# d:\source\Demoulas\smart-profit-sharing\src\ui
node --version
npm i
npm run dev
```

## 4) Secrets and package sources

- Ask a teammate for the required user-secrets and follow: https://learn.microsoft.com/aspnet/core/security/app-secrets
- Ensure the "ArtifactoryCloud" NuGet source is configured if needed:
  ```pwsh
  dotnet nuget list source
  ```
  If missing, follow your internal guide to add it.

## Oracle 19 notes

- The services use Oracle.ManagedDataAccess.Core (managed ODP.NET), so a local Oracle Instant Client is not required for the app to run.
- Optional GUI/CLI:
  - DBeaver (installed via winget) for running SQL scripts.
  - Oracle SQL Developer / SQLcl can be installed manually from Oracle if preferred (licensing and winget availability vary).
- Connectivity tips:
  - Verify network/VPN access to the Oracle host and port.
  - Use EZConnect style connection strings (host:port/service_name) where possible.
  - If using TNS aliases, ensure tnsnames.ora is in the expected location and the alias matches.

## Troubleshooting

- dotnet SDK mismatch: check `src/services/global.json` (9.0.300). Install .NET 9 via winget import above.
- Node version mismatch: run `volta install node@LTS` and restart the terminal.
- EF tools not found: run `dotnet tool update --global dotnet-ef`.
- Oracle connectivity errors (ORA-12541/12514): confirm firewall/VPN, correct host/port/service name, and credentials.
- SSL/TLS issues to internal feeds: ensure corporate root certs are installed and NuGet source is configured.

## Optional

- Visual Studio workloads: install ASP.NET and web development, Azure, and .NET desktop development (VS will prompt as needed)
