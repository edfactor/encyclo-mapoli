# scripts

This folder contains small helper scripts used by CI or developers.

## Install-GitHooks.ps1
Installs git hooks for the repository, including the pre-commit hook that runs `dotnet format` on staged C# files.

**Usage:**
```powershell
.\scripts\Install-GitHooks.ps1
```

**What it does:**
- Creates a pre-commit hook in `.git/hooks/pre-commit`
- The hook runs `dotnet format` on any staged C# files in `src/services` before allowing commits
- Ensures consistent code formatting across the team

**Note:** New developers should run this script after cloning the repository to set up the hooks.

## add-jira-comment.ps1
Posts a comment to a Jira issue using Jira Cloud REST API.
- Usage: see header in the script. Keep `JIRA_API_TOKEN` in CI secrets.

Security note: prefer using the organization's Atlassian MCP integration for automated Jira interactions where possible. If your CI cannot use MCP, store API tokens in the pipeline's protected variables.
