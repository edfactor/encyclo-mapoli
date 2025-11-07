# Developer Setup

This project uses .NET 9 for services and Node 20.4 (via Volta) for the UI. The steps below install common tooling and get you running quickly.

## Quick Start (Automated)

### Prerequisites
- Windows 10/11
- Administrator privileges
- [Windows Package Manager (winget)](https://www.microsoft.com/p/app-installer/9nblggh4nns1) installed

### Enable PowerShell Script Execution

If you get the error: **"cannot be loaded because running scripts is disabled on this system"**

1. Open **PowerShell as Administrator**
   ```powershell
   Start-Process pwsh -ArgumentList '-NoExit' -Verb RunAs
   ```

2. Set execution policy to allow local scripts:
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
   ```

3. Verify it worked:
   ```powershell
   Get-ExecutionPolicy
   # Should return: RemoteSigned
   ```

### Run Setup Script

Open **PowerShell as Administrator** and run:

```pwsh
cd D:\source\Demoulas\smart-profit-sharing\developer_setup
.\Setup-DeveloperEnvironment.ps1
```

The script automatically:
- ✓ Installs all required development tools
- ✓ Configures Visual Studio 2022 with required workloads
- ✓ Verifies Volta and Node.js setup
- ✓ Displays next steps

---

## Manual Installation

### 1) Install Required Tools with winget

Run in PowerShell (**as Administrator**):

```pwsh
winget import --import-file .\developer_setup\winget-config.json --accept-package-agreements --accept-source-agreements
```

What this installs:
- **Visual Studio Code** - Code editor
- **Visual Studio 2022 Professional** - Full IDE (with workload configuration)
- **Volta** - Node.js version manager (pins to package.json version)
- **Git & TortoiseGit** - Version control
- **Postman** - API testing
- **Web Deploy** - Deployment tooling

Note: This config is streamlined for profit-sharing development. SQL Server tooling was intentionally removed (we use Oracle).

### 2) Configure Visual Studio Workloads

Visual Studio 2022 requires these workloads for development:
- ✓ **ASP.NET and web development** - For API development
- ✓ **Azure development** - For cloud services
- ✓ **NET desktop development** - For WPF/Forms support

**Option A: Automatic Configuration (Recommended)**

The setup script handles this automatically. If you installed manually, run:

```pwsh
# Find your Visual Studio installation
$vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath

# Install required workloads
& "$vsPath\Common7\Tools\vsconfig.exe" `
  --add Microsoft.VisualStudio.Workload.NetWeb `
  --add Microsoft.VisualStudio.Workload.Azure `
  --add Microsoft.VisualStudio.Workload.ManagedDesktop `
  --quiet --norestart
```

**Option B: Manual Configuration**

1. Open **Visual Studio Installer**
2. Click "Modify" on Visual Studio 2022 Professional
3. Check these workloads:
   - ☑ ASP.NET and web development
   - ☑ Azure development
   - ☑ .NET desktop development
4. Click "Modify" to install

### 3) Finalize CLI Tooling

```pwsh
# Pin Node.js to the version specified by Volta in ui/package.json
volta install node@LTS

# EF Core tooling (global)
dotnet tool update --global dotnet-ef

# Optional: Install git hooks (pre-commit runs dotnet format)
.\scripts\Install-GitHooks.ps1
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

### Script Execution Issues

**Error: "cannot be loaded because running scripts is disabled on this system"**

This happens because PowerShell's execution policy prevents running unsigned scripts. Fix it:

```powershell
# Open PowerShell as Administrator
Start-Process pwsh -ArgumentList '-NoExit' -Verb RunAs

# Allow scripts for current user (recommended)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force

# Verify it worked
Get-ExecutionPolicy
# Should return: RemoteSigned

# Now try running the setup script again
cd D:\source\Demoulas\smart-profit-sharing\developer_setup
.\Setup-DeveloperEnvironment.ps1
```

**Why this works:**
- `RemoteSigned` allows locally-created scripts to run
- `-Scope CurrentUser` applies only to your user account (doesn't affect system)
- This is the safest and most developer-friendly setting

**Alternative (System-wide, requires admin):**
```powershell
# Only do this if RemoteSigned doesn't work and you're on a corporate machine
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine -Force
```

**Other execution policy errors:**

If you get "not digitally signed" even after RemoteSigned:
```powershell
# Temporarily bypass for this script only
powershell -ExecutionPolicy Bypass -File .\Setup-DeveloperEnvironment.ps1
```

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
