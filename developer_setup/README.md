# Developer Setup

This project uses .NET 9 for services and Node 20.4 (via Volta) for the UI. The steps below install common tooling and get you running quickly.

## Quick Start (Automated - RECOMMENDED)

### For New Developers - ONE Command Setup

**All you need**: Windows 10/11 and PowerShell

1. **Open PowerShell as Administrator**
   ```powershell
   Start-Process pwsh -Verb RunAs
   ```

2. **Enable script execution** (one-time):
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
   ```

3. **Clone the repository** (or download the script folder):
   ```powershell
   git clone https://bitbucket.org/demoulas/smart-profit-sharing
   cd smart-profit-sharing
   ```

4. **Run the setup script**:
   ```powershell
   cd developer_setup
   .\Setup-DeveloperEnvironment.ps1
   ```

That's it! The script will automatically:
- ✓ Check prerequisites (winget installed)
- ✓ Read local `winget-config.json`
- ✓ Install all required tools via winget
- ✓ Configure Visual Studio 2022 workloads
- ✓ Verify Node.js (Volta)
- ✓ Show next steps

**Note**: The script only reads the local `winget-config.json` file in the same folder. No code is downloaded—only your development tools via winget.

---

## Manual Installation (If Automated Script Fails)

### 1) Download and Run Setup Script

If you prefer to use the script file directly:

1. Save `Setup-DeveloperEnvironment.ps1` locally
2. Open PowerShell as Administrator
3. Enable script execution:
   ```pwsh
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
   ```
4. Run the script:
   ```pwsh
   .\Setup-DeveloperEnvironment.ps1
   ```

### 2) Manual Package Installation

If the script fails, install packages individually:

```pwsh
winget install Microsoft.VisualStudioCode --accept-package-agreements
winget install Microsoft.VisualStudio.2022.Professional --accept-package-agreements
winget install Git.Git --accept-package-agreements
winget install TortoiseGit.TortoiseGit --accept-package-agreements
winget install Postman.Postman --accept-package-agreements
winget install Volta.Volta --accept-package-agreements
winget install Microsoft.WebDeploy --accept-package-agreements
```

### 3) Manual Visual Studio Configuration

1. Open **Visual Studio Installer**
2. Click **Modify** on Visual Studio 2022 Professional
3. Select these workloads:
   - ☑ ASP.NET and web development
   - ☑ Azure development
   - ☑ .NET desktop development
4. Click **Modify** to install

### 4) Manual Volta Installation

If Volta didn't install via winget:

1. Visit: https://docs.volta.sh/guide/getting-started
2. Download the installer for Windows
3. Run the installer and restart PowerShell
4. Verify: `volta --version`

### 5) Complete Setup

Once tools are installed, finish with:

```pwsh
# Verify Node.js is available
node --version
npm --version

# Install EF Core tools globally
dotnet tool update --global dotnet-ef

# Optional: Install git hooks
.\scripts\Install-GitHooks.ps1
```

### 5) Verify Installation

```pwsh
dotnet --version
git --version
volta --version
```
```

### 4) Restore and Build

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

### Setup Script Issues

**winget import fails:**
```pwsh
# Install packages individually
winget install Microsoft.VisualStudioCode --accept-package-agreements
winget install Microsoft.VisualStudio.2022.Professional --accept-package-agreements
winget install Git.Git --accept-package-agreements
winget install TortoiseGit.TortoiseGit --accept-package-agreements
winget install Postman.Postman --accept-package-agreements
winget install Volta.Volta --accept-package-agreements
winget install Microsoft.WebDeploy --accept-package-agreements
```

**Visual Studio workloads don't install:**
1. Open **Visual Studio Installer**
2. Click "Modify" on Visual Studio 2022 Professional
3. Select the required workloads manually
4. Click "Modify"

**Volta not found after installation:**
- Close and restart PowerShell (it may need to reload PATH)
- Verify: `volta --version`

### Development Issues

- **dotnet SDK mismatch**: Check `src/services/global.json` (requires 9.0.300+). Run `winget install Microsoft.DotNet.SDK.9`
- **Node version mismatch**: Run `volta install node@LTS` and restart terminal
- **EF tools not found**: Run `dotnet tool update --global dotnet-ef`
- **Oracle connectivity errors** (ORA-12541/12514): Verify firewall/VPN, correct host/port/service name, and credentials
- **SSL/TLS issues to internal feeds**: Ensure corporate root certs installed and NuGet source configured

## Next Steps

After setup completes:

1. **Clone repository** (if not done):
   ```pwsh
   git clone https://bitbucket.org/demoulas/smart-profit-sharing
   cd smart-profit-sharing
   ```

2. **Get secrets**:
   - Ask a teammate for `secrets.json`
   - Place in: `src/services/configuration/secrets.json`
   - See: https://learn.microsoft.com/aspnet/core/security/app-secrets

3. **Configure NuGet source**:
   ```pwsh
   dotnet nuget list source
   ```
   - If "ArtifactoryCloud" is missing, add it following your internal guide

4. **Build and run**:
   - Open `src/services/Demoulas.ProfitSharing.slnx` in Visual Studio
   - Set `Demoulas.ProfitSharing.AppHost` as startup project
   - Press `F5` to run

5. **UI Development**:
   ```pwsh
   cd src/ui
   npm install
   npm run dev
   ```

## Oracle 19 Notes

- The services use **Oracle.ManagedDataAccess.Core** (managed ODP.NET), so a local Oracle Instant Client is not required
- Optional GUI tools:
  - **DBeaver** - Installed via winget (recommended)
  - **Oracle SQL Developer / SQLcl** - Install manually from Oracle if preferred
- Connection verification:
  - Confirm network/VPN access to Oracle host and port
  - Use EZConnect format: `host:port/service_name`
  - If using TNS aliases, ensure `tnsnames.ora` exists in the expected location

## Optional Enhancements

- **Visual Studio Extensions**:
  - Productivity Power Tools 2022
  - Visual Studio Spell Checker (VS2022 and Later)
  - Install from Extensions → Manage Extensions in VS

- **Git LFS** (for large files):
  ```pwsh
  git lfs install
  ```

- **Pre-commit Hooks**:
  ```pwsh
  .\scripts\Install-GitHooks.ps1
  ```
  Automatically runs `dotnet format` on staged C# files
