# Developer Setup - Quick Reference Card

## 🚀 NEW DEVELOPER? Start Here!

### One-Minute Setup

```powershell
# 1. Open PowerShell as Administrator
Start-Process pwsh -Verb RunAs

# 2. Enable scripts (first time only)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force

# 3. Run setup (from anywhere - downloads config from GitHub)
& (Invoke-WebRequest -Uri "https://raw.githubusercontent.com/demoulas/smart-profit-sharing/develop/developer_setup/Setup-DeveloperEnvironment.ps1" -UseBasicParsing).Content
```

That's it! The script will:

- ✓ Install all development tools
- ✓ Configure Visual Studio 2022
- ✓ Show you the next steps

---

## ⚠️ Common Issues & Fixes

### "Cannot run scripts" Error

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
```

### "winget not found" Error

Download: https://www.microsoft.com/p/app-installer/9nblggh4nns1

### Installation Hangs

- Let it finish (may take 10-20 minutes)
- Don't interrupt the process
- Check for prompts/popups

---

## ✅ Next Steps After Setup

1. **Get secrets.json**

   - Ask team member for the file
   - Place in: `src/services/configuration/secrets.json`

2. **Add NuGet Source**

   - Follow: `developer_setup/README.md`
   - Add ArtifactoryCloud source with your credentials

3. **Clone Repository**

   ```powershell
   git clone https://bitbucket.org/demoulas/smart-profit-sharing
   cd smart-profit-sharing
   ```

4. **Open Solution**
   - File → Open → `src/services/Demoulas.ProfitSharing.slnx`
   - Right-click `Demoulas.ProfitSharing.AppHost` → Set as Startup Project
   - Press F5

---

## 📚 Documentation

| Document                         | Purpose                 |
| -------------------------------- | ----------------------- |
| `README.md`                      | Complete setup guide    |
| `Setup-DeveloperEnvironment.ps1` | Automated setup script  |
| `winget-config.json`             | Package configuration   |
| `src/services/README.md`         | Backend setup details   |
| `CLAUDE.md`                      | Architecture & patterns |

---

## 🔧 Manual Fallback (If Script Fails)

```powershell
# Install each tool individually
winget install Microsoft.VisualStudioCode --accept-package-agreements
winget install Microsoft.VisualStudio.2022.Professional --accept-package-agreements
winget install Git.Git --accept-package-agreements
winget install TortoiseGit.TortoiseGit --accept-package-agreements
winget install Postman.Postman --accept-package-agreements
winget install Volta.Volta --accept-package-agreements
winget install Microsoft.WebDeploy --accept-package-agreements

# Then configure Visual Studio manually:
# - Open Visual Studio Installer
# - Click Modify on Visual Studio 2022 Professional
# - Select: ASP.NET & Web, Azure, .NET Desktop workloads
# - Click Modify
```

---

## 🆘 Still Having Issues?

1. **Verify Prerequisites**

   ```powershell
   dotnet --version          # Should be 9.x
   git --version             # Should exist
   volta --version           # Should exist
   ```

2. **Check Paths**

   - Secrets: `src/services/configuration/secrets.json`
   - Solution: `src/services/Demoulas.ProfitSharing.slnx`

3. **Get Help**
   - Check `README.md` Troubleshooting section
   - Ask in Slack
   - Contact team lead

---

## 💡 Useful Commands

```powershell
# Build services
cd src/services
dotnet build Demoulas.ProfitSharing.slnx

# Run tests
# Tests use xUnit v3 + Microsoft Testing Platform (MTP) via src/services/global.json.
# Run from src/services and use the new dotnet test syntax.
dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj
# Filter examples (MTP/xUnit options; NOT VSTest `--filter`):
# dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj --filter-class *OracleHcmDiagnostics*

# Start UI dev server
cd src/ui
npm run dev

# Pin Node version (Volta)
volta install node@LTS

# Update .NET tools
dotnet tool update --global dotnet-ef
```

---

## 📋 What Gets Installed

- Visual Studio Code
- Visual Studio 2022 Professional
- Git & TortoiseGit
- Postman
- Volta (Node.js manager)
- Web Deploy
- .NET 9 SDK

**Total**: ~3-5 GB depending on existing installations

---

_For complete details, see `developer_setup/README.md`_
