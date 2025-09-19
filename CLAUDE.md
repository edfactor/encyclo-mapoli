# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Smart Profit Sharing is a full-stack application with:
- **Backend**: .NET 9 services using Fast-Endpoints, EntityFramework Core 9 with Oracle
- **Frontend**: React 19 + TypeScript with Vite, Material-UI, Redux Toolkit
- **Authentication**: Okta integration
- **Database**: Oracle 19

## Essential Commands

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

# Testing
npm run test        # Run Vitest with coverage
npx playwright test # Run E2E tests
```

### Backend (Services) - Run from `src/services/`
```bash
# Build solution
dotnet build Demoulas.ProfitSharing.slnx

# Run tests
dotnet test Demoulas.ProfitSharing.Tests.csproj --configuration debug

# Database management (using Data.Cli)
Demoulas.ProfitSharing.Data.Cli upgrade-db --connection-name ProfitSharing
Demoulas.ProfitSharing.Data.Cli drop-recreate-db --connection-name ProfitSharing

# EF Core migrations
dotnet ef migrations add {migrationName} --context ProfitSharingDbContext
dotnet ef migrations script --context ProfitSharingDbContext --output {FILE}
```

## Architecture

### Frontend Architecture
- **State Management**: Redux Toolkit stores in `src/ui/src/reduxstore/`
- **Routing**: React Router v7 with protected routes
- **Components**: Reusable components in `src/ui/src/components/`
- **Pages**: Feature-specific pages in `src/ui/src/pages/`
- **API Integration**: Services communicate with .NET backend
- **Forms**: React Hook Form with Yup validation
- **UI Library**: Material-UI v7 components, custom `smart-ui-library` package
- **Path Aliases**: Use `@/`, `components/`, `utils/`, `pages/`, etc. (configured in vite.config.ts)

### Backend Architecture
- **API Pattern**: Fast-Endpoints with REPR pattern
- **Database**: Oracle with EF Core 9, migrations in Data project
- **Projects**:
  - `Demoulas.ProfitSharing.AppHost`: .NET Aspire host (startup project)
  - `Demoulas.ProfitSharing.Services`: Main API endpoints
  - `Demoulas.ProfitSharing.Data`: EF Core models and migrations
  - `Demoulas.ProfitSharing.Data.Cli`: Database management CLI
- **Testing**: Shouldly assertions, Bogus for test data generation
- **Mapping**: Riok.Mapperly for object mapping
- **Logging**: Serilog structured logging
- **Security**: NetEscapades security headers, feature flags

## Development Workflow

1. **Branch Strategy**: Feature branches from `develop`, PR to `develop`
2. **Git Hooks**: Pre-commit hooks configured (.pre-commit-config.yaml)
3. **CI/CD**: Bitbucket Pipelines (bitbucket-pipelines.yml)
4. **Node Version**: Managed by Volta (v22.16.0 specified in package.json)
5. **Package Management**: Central NuGet packages (Directory.Packages.props)

## Testing Strategy

- **Unit Tests**: Vitest for frontend (`npm run test`), dotnet test for backend
- **E2E Tests**: Playwright configured with authentication setup
- **Test Data**: Bogus library for backend test data generation
- **Coverage**: Frontend coverage via Vitest, reports in HTML format

## Key Dependencies

### Frontend
- React 19.1.0, TypeScript 5.8.3
- Material-UI v7, AG-Grid for data tables
- Redux Toolkit for state, React Hook Form for forms
- Okta Auth/React for authentication
- Date handling: date-fns, dayjs

### Backend
- .NET 9, Fast-Endpoints
- Oracle.EntityFrameworkCore for database
- Serilog for logging, MassTransit/RabbitMQ for messaging
- Feature management for feature flags

## Environment Configuration

- Frontend environments: `.env` files with Vite modes (production, qa, uat)
- Backend secrets: Use .NET user-secrets (get from team member)
- Oracle connection: Requires VPN access, uses EZConnect strings
- NuGet: Ensure "ArtifactoryCloud" source is configured

## Important Notes

- Always verify parent directories before creating new files/folders
- Quote file paths with spaces in bash commands
- Run lint/typecheck before committing changes
- Frontend dev server runs on port 3100
- Use existing patterns and libraries rather than introducing new ones