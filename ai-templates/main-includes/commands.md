## Quick Commands

### PowerShell

```pwsh
# Build services
cd src/services; dotnet build Demoulas.ProfitSharing.slnx
# Run tests (quiet - no progress output, only failures and summary)
cd src/services; dotnet test -- --no-progress
# Start UI
cd src/ui; npm run dev
```

### Frontend (UI) - Run from `src/ui/`

```bash
# Install dependencies
npm install

# Development server (port 3100)
npm run dev

# Build for different environments
npm run build:prod  # Production build with TypeScript check
npm run build:qa    # QA environment
npm run build:uat   # UAT environment

# Code quality
npm run lint        # ESLint with max 0 warnings
npm run prettier    # Format code
npx tsc -b --noEmit # Tyescript compiler check

# Testing
npm run test        # Run Vitest with coverage
npx playwright test # Run E2E tests
```

### Backend (Services) - Run from `src/services/`

```bash
# Build solution
dotnet build Demoulas.ProfitSharing.slnx

# Run tests (quiet - no progress output, only failures and summary)
dotnet test -- --no-progress

# Run tests (verbose - all output with progress)
dotnet test

# Database management (using Data.Cli)
Demoulas.ProfitSharing.Data.Cli upgrade-db --connection-name ProfitSharing
Demoulas.ProfitSharing.Data.Cli drop-recreate-db --connection-name ProfitSharing

# EF Core migrations
dotnet ef migrations add {migrationName} --context ProfitSharingDbContext
dotnet ef migrations script --context ProfitSharingDbContext --output {FILE}
```
