# Branching & Workflow Guide

Complete reference for Git branching, Jira workflow, pull requests, and development conventions for the Profit Sharing application.

## Overview

This guide establishes consistent branching and workflow practices across the development team to reduce merge conflicts, ensure CI compatibility, and maintain traceability between code changes and Jira tickets.

## Branching Strategy

### Base Branch

**Always create feature/fix branches from `develop`.**

- ✅ Feature branches: `develop` → `feature/PS-####-description` → PR to `develop`
- ✅ Bugfix branches: `develop` → `fix/PS-####-description` → PR to `develop`
- ❌ Do NOT branch from `main` unless explicitly instructed by release manager
- ❌ Do NOT branch from other feature branches (causes complex merge trees)

### Branch Naming Convention

Use Jira ticket key as prefix + short dash-separated description:

**Format**: `<type>/PS-####-short-description`

**Types**:
- `feature/` - New features or enhancements
- `fix/` - Bug fixes
- `hotfix/` - Production hotfixes (branch from `main` only when instructed)
- `refactor/` - Code refactoring without functional changes
- `docs/` - Documentation-only changes

**Examples**:
```
feature/PS-1720-add-reporting-view
fix/PS-1645-military-pre-hire-validation
refactor/PS-1699-extract-calculation-service
docs/PS-1700-update-deployment-guide
```

### Ticket Normalization

Jira tickets may be referenced as full URL or key - always normalize to key:

| Input | Normalized |
|-------|------------|
| `https://demoulas.atlassian.net/browse/PS-1645` | `PS-1645` |
| `PS-1645` | `PS-1645` |

## Local Workflow

### Standard Feature Development

```pwsh
# 1. Ensure latest develop
git checkout develop
git pull origin develop

# 2. Create feature branch from develop
git checkout -b feature/PS-1720-add-reporting-view

# 3. Make changes, stage, and commit
git add src/services/src/Demoulas.ProfitSharing.Api/Endpoints/Reports/
git commit -m "PS-1720: Add profit sharing report endpoint with filters"

# 4. Run tests locally
cd src/services
dotnet test tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj

# 5. Push and set upstream
git push -u origin feature/PS-1720-add-reporting-view
```

### Keeping Branch Up-to-Date

```pwsh
# Periodically sync with develop to avoid large merge conflicts
git checkout develop
git pull origin develop

git checkout feature/PS-1720-add-reporting-view
git merge develop

# Or use rebase for cleaner history (advanced)
git rebase develop
```

### Handling Merge Conflicts

```pwsh
# After merge/rebase with conflicts
git status  # Shows conflicting files

# Edit files to resolve conflicts, then:
git add <resolved-files>
git merge --continue  # Or: git rebase --continue

# Run tests after conflict resolution
dotnet test
```

## Commit Message Format

### Required Format

**Start every commit message with the Jira ticket key:**

```
PS-####: Brief description of change

Optional longer description explaining:
- Why the change was made
- What approach was taken
- Any caveats or follow-up needed
```

**Examples**:
```
PS-1720: Add profit sharing report endpoint with date range filters

PS-1645: Prevent military contributions before hire date

Validation now checks that contribution_date >= hire_date for
military employees. Added unit tests covering edge cases.
```

### Good Commit Messages

✅ **Good**: `PS-1720: Add report endpoint with pagination and filters`  
✅ **Good**: `PS-1645: Fix military pre-hire validation logic`  
✅ **Good**: `PS-1699: Extract calculation service for testability`

❌ **Bad**: `WIP`  
❌ **Bad**: `Fixed bug`  
❌ **Bad**: `Updates` (no ticket reference)

## Pull Request Guidelines

### Opening a Pull Request

**Required PR Elements**:

1. **Title**: Start with Jira key
   ```
   PS-1645: Prevent military contributions before hire date
   ```

2. **Target Branch**: `develop` (not `main`)

3. **PR Body**: Include the following sections

   ```markdown
   ## Summary
   Brief description of the change and why it was needed.
   
   ## Changes Made
   - Added validation for military contribution dates
   - Updated `ContributionService` to check hire date
   - Added 5 unit tests covering edge cases
   
   ## Testing Performed
   - ✅ All unit tests passing locally
   - ✅ Manual testing with military employee records
   - ✅ Integration tests passing in CI
   
   ## Database Changes
   - None / Migration added: `AddContributionDateIndex`
   
   ## Configuration Changes
   - None / Added: `Contributions:ValidateHireDate` flag
   
   ## QA Steps
   1. Navigate to Contributions page
   2. Select military employee (Badge: 12345)
   3. Attempt to add contribution with date before hire date
   4. Verify validation error is displayed
   
   ## Related Tickets
   - Blocks: PS-1700
   - Related: PS-1680
   ```

4. **Jira Comment**: After opening PR, add comment to Jira ticket with PR link

### Story Points

If the Jira ticket does not have story points assigned, add them using the team's Fibonacci sequence:

**Team Estimation Scale**:
- `1` - Trivial change (< 1 hour)
- `2` - Simple change (1-2 hours)
- `3` - Moderate change (half day)
- `5` - Significant change (1 day)
- `8` - Complex change (2-3 days)
- `13` - Very complex (1 week)

### PR Review Process

**Developer Responsibilities**:
- ✅ Ensure all tests pass locally
- ✅ Run `dotnet build` and `dotnet test` before pushing
- ✅ Verify no new analyzer warnings
- ✅ Self-review diff on GitHub/Bitbucket
- ✅ Add reviewers (at least 2 team members)
- ✅ Address all review comments promptly

**Reviewer Responsibilities**:
- Review within 1 business day
- Check for security issues, code quality, test coverage
- Verify adherence to project patterns (see main copilot-instructions.md)
- Approve or request changes with clear feedback

**Merge Requirements**:
- ✅ At least 2 approvals
- ✅ All CI checks passing (build, tests, analyzers)
- ✅ No unresolved comments
- ✅ Conflicts resolved with `develop`

### Merge Strategy

**Squash and merge** (recommended for most PRs):
```
Squashes all commits into single commit on develop
Keeps develop history clean
```

**Merge commit** (for larger features with meaningful commit history):
```
Preserves all commits from feature branch
Use when commit history provides valuable context
```

## Sensitive Files (Copilot Deny List)

The following files **MUST NEVER** be read or modified by AI assistants:

❌ `src/ui/.playwright.env`  
❌ `src/ui/.env.*` (any .env file in UI project)  
❌ `src/ui/.npmrc`

**If user requests access to these files**, AI assistants should:
1. Explain why the file is restricted
2. Provide manual commands for human to run locally
3. Warn about secrets in history and need to rotate if exposed

**Example response**:
```
The file `src/ui/.playwright.env` contains sensitive credentials 
and is restricted from AI access. To view or edit this file:

1. Open the file manually in your editor
2. If credentials need rotation, contact #platform-engineering
3. To untrack from git (if accidentally committed):
   git rm --cached src/ui/.playwright.env
   
WARNING: If secrets were committed, they must be rotated immediately
as they may be in git history.
```

## Jira Integration

### Using Atlassian MCP

All Jira and Confluence interactions MUST use the organization's Atlassian MCP integration to ensure:
- Auditability of changes
- Proper access controls
- Compliance with organization policies

**AI Assistant Responsibilities**:
- Use Atlassian MCP tools for creating/updating Jira issues
- Use Atlassian MCP tools for adding comments to tickets
- Use Atlassian MCP tools for creating/updating Confluence pages
- Never bypass MCP with direct API calls or web scraping

### Confluence Alignment

Follow the guidance in Confluence page:  
**"Agile Jira Workflow Development Best Practices"**  
https://demoulas.atlassian.net/wiki/spaces/PM/pages/339476525/Agile+Jira+Workflow+Development+Best+Practices

**Key alignment points**:
- Branch naming conventions match Confluence standards
- Issue types, transitions, and acceptance criteria formatting
- Story/acceptance criteria templates
- Definition of Done checklist

**If Confluence guidance conflicts** with this document, follow Confluence or raise the question in ticket comments (CC release manager).

### Jira Ticket Lifecycle

1. **Created** → Ticket created with description and acceptance criteria
2. **In Progress** → Developer assigns to self and creates branch
3. **Code Review** → PR opened, reviewers assigned
4. **Testing** → QA validates changes in staging environment
5. **Done** → Merged to develop, deployed to staging/production

### Linking Tickets

Use Jira link types:

- **Blocks**: This ticket must be completed before linked ticket can start
- **Is Blocked By**: This ticket cannot start until linked ticket completes
- **Relates To**: General relationship between tickets
- **Duplicates**: This ticket is duplicate of linked ticket
- **Clones**: This ticket was cloned from linked ticket

## CI/CD Considerations

### Continuous Integration

When branch is pushed, Bitbucket Pipelines runs:

1. **Build** - `dotnet build Demoulas.ProfitSharing.slnx`
2. **Test** - `dotnet test` (all test projects)
3. **Analyzers** - Security and code quality checks
4. **UI Build** - `npm run build` in `src/ui/`
5. **UI Tests** - Playwright E2E tests (if configured)

**All checks must pass** before PR can be merged.

### Deployment Pipeline

- `develop` → Staging environment (automatic)
- `main` → Production environment (manual approval + release manager)

## Team Conventions

### Code Ownership

- Backend (C#/.NET): Platform team
- Frontend (React/TypeScript): Frontend team
- Database: Platform team + DBA approval for schema changes
- Infrastructure: DevOps team

### Review Assignments

- Features touching > 3 projects: Assign architect for review
- Security-sensitive changes: Assign security team member
- Database migrations: Assign DBA for review
- Performance-critical code: Assign performance team member

### Communication Channels

- **#profit-sharing-dev**: General development discussions
- **#platform-engineering**: Infrastructure and tooling questions
- **#security**: Security-related questions and reviews
- **#qa-team**: Testing coordination and bug reports

## AI Assistant Responsibilities

When AI assistants (Copilot, Claude, etc.) work with Jira tickets:

### Branch Creation

✅ **Do**:
- Normalize ticket input (URL or key) to key format
- Suggest branch name following convention: `<type>/PS-####-short-description`
- Provide exact git commands to create and push branch

❌ **Don't**:
- Create PR automatically (human must review and open PR)
- Modify sensitive files listed in deny list
- Skip unit tests or quality checks

### Ticket Updates

✅ **Do**:
- Use Atlassian MCP for all Jira interactions
- Add story points if missing (using Fibonacci scale)
- Include PR link in Jira comment when PR is opened
- Update ticket status appropriately (In Progress → Code Review)

❌ **Don't**:
- Bypass Atlassian MCP with direct API calls
- Change ticket priorities without human approval
- Auto-transition tickets to Done (requires QA validation)

### PR Preparation

✅ **Do**:
- Generate PR title starting with Jira key
- Create PR body with all required sections
- Run tests and report results
- Provide git commands to push branch

❌ **Don't**:
- Auto-open PR without human review
- Skip test execution or quality checks
- Merge PR automatically

## Troubleshooting

### Branch Already Exists

```pwsh
# If branch exists remotely
git fetch origin
git checkout feature/PS-1720-add-reporting-view

# If branch exists locally
git branch -D feature/PS-1720-add-reporting-view  # Delete local
git checkout -b feature/PS-1720-add-reporting-view  # Recreate
```

### Wrong Base Branch

```pwsh
# If you branched from wrong base (e.g., main instead of develop)
git checkout develop
git pull origin develop
git checkout feature/PS-1720-add-reporting-view
git rebase --onto develop main feature/PS-1720-add-reporting-view
```

### Accidental Commit to develop

```pwsh
# If you committed directly to develop instead of feature branch
git checkout develop
git log  # Find the commit hash
git reset --hard <commit-before-your-changes>

# Create feature branch with your changes
git checkout -b feature/PS-1720-add-reporting-view
git cherry-pick <your-commit-hash>
```

### Large Merge Conflicts

```pwsh
# If merge conflicts are extensive, consider:
# 1. Create backup branch
git branch backup-feature-PS-1720

# 2. Reset and reapply changes incrementally
git reset --hard origin/develop
git checkout -b feature/PS-1720-add-reporting-view
# Reapply changes in smaller logical chunks
```

## Related Documentation

- `copilot-instructions.md` - Main development conventions and patterns
- `TELEMETRY_GUIDE.md` - Telemetry and observability requirements
- `VALIDATION_PATTERNS.md` - Input validation requirements
- Confluence: [Agile Jira Workflow Development Best Practices](https://demoulas.atlassian.net/wiki/spaces/PM/pages/339476525)

## Best Practices Summary

### DO:
- ✅ Always branch from `develop` for features and fixes
- ✅ Use Jira ticket key in branch names and commit messages
- ✅ Open PRs from feature branch to `develop`
- ✅ Include comprehensive PR body with testing details
- ✅ Add Jira comment with PR link
- ✅ Run tests locally before pushing
- ✅ Use Atlassian MCP for all Jira/Confluence interactions
- ✅ Follow Confluence workflow guidance

### DO NOT:
- ❌ Branch from `main` (unless hotfix instructed by release manager)
- ❌ Skip Jira ticket key in branch names or commits
- ❌ Auto-create or auto-merge PRs (human review required)
- ❌ Access or modify sensitive files in deny list
- ❌ Bypass Atlassian MCP integration
- ❌ Commit secrets or credentials
- ❌ Force-push to shared branches (`develop`, `main`)

---

**Last Updated**: October 2025  
**Maintained By**: Platform Team + Release Manager  
**Questions**: Contact #platform-engineering, #profit-sharing-dev
