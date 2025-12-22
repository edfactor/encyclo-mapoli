# scripts

This folder contains small helper scripts used by CI or developers.

## Validate-Pipeline.ps1

**⚠️ IMPORTANT: Run this before committing changes to `bitbucket-pipelines.yml`**

Validates the Bitbucket pipelines YAML syntax to catch errors before pushing to remote.

**Usage:**

```powershell
# Validate pipeline file
.\scripts\Validate-Pipeline.ps1

# Auto-install Node.js if missing
.\scripts\Validate-Pipeline.ps1 -InstallIfMissing
```

**What it does:**

- Checks if Node.js/npx is installed (required for validation)
- Validates `bitbucket-pipelines.yml` syntax using js-yaml
- Reports any YAML syntax errors found
- Distinguishes between errors in your changes vs. pre-existing issues

**When to use:**

- Before committing any changes to `bitbucket-pipelines.yml`
- When adding new pipeline steps or custom pipelines
- When modifying existing pipeline configuration
- As part of pre-commit validation workflow

**Notes:**

- js-yaml may be stricter than Bitbucket's parser
- Some pre-existing YAML patterns may trigger warnings
- Focus on errors in sections YOU modified
- Compare error line numbers against your `git diff`

**Requirements:** Node.js 16+ (will prompt to install if missing)

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
