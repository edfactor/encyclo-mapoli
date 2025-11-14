# Quick Start - New Developer Setup

## One-Time Setup (5-10 minutes)

**Prerequisites**: Windows 10/11, PowerShell, Internet connection (for tool downloads only)

### Steps

1. **Clone the repository**:
   ```powershell
   git clone https://bitbucket.org/demoulas/smart-profit-sharing
   cd smart-profit-sharing
   ```

2. **Open PowerShell as Administrator**

3. **Enable script execution** (one-time):
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
   ```

4. **Run the setup script**:
   ```powershell
   cd developer_setup
   .\Setup-DeveloperEnvironment.ps1
   ```

5. **Wait for completion** (5-15 minutes depending on internet speed)
   - Script installs: VS Code, Visual Studio 2022, Git, TortoiseGit, Postman, Volta, Web Deploy
   - Configures VS2022 workloads automatically
   - Shows next steps when complete

---

## After Setup

1. **Get secrets.json** from a team member
   - Place it in: `src/services/configuration/secrets.json`

2. **Add NuGet source** (see `src/services/README.md` for ArtifactoryCloud details)

3. **Open the solution**:
   ```powershell
   cd src/services
   # Open in VS Code
   code Demoulas.ProfitSharing.slnx
   # OR in Visual Studio
   start Demoulas.ProfitSharing.slnx
   ```

4. **Start development**:
   ```powershell
   # From project root
   aspire run
   ```

---

## Troubleshooting

### Script won't run
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
```

### "winget-config.json not found"
- Make sure you're running the script from the `developer_setup` folder
- Or copy `winget-config.json` to the same folder as the script

### VS2022 workloads didn't install
- Run manually from VS Installer: **Modify → ASP.NET and web development, Azure development, .NET desktop development**

### Volta not found
- Restart PowerShell and verify: `volta --version`
- If still missing, run: `winget install Volta.Volta --accept-package-agreements`

---

## Documentation

- **Full Setup Guide**: `developer_setup/README.md`
- **Services Documentation**: `src/services/README.md`
- **UI Documentation**: `src/ui/README.md`
