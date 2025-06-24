# Demoulas Profit Sharing API

Platform for managing profit sharing, payroll, and employee data at Demoulas. Supports Oracle database integration, background processing, and modern .NET development practices.

---

## Table of Contents
- [Project Overview](#project-overview)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Build and Test](#build-and-test)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Suggested Visual Studio Extensions](#suggested-visual-studio-extensions)
- [Good Reads](#good-reads)
- [Contributing](#contributing)
- [Contact](#contact)

---

## Project Overview
This solution provides a set of APIs and background worker services for Demoulas profit sharing, payroll, and employee management. It leverages .NET 9, .NET Standard 2.0, Entity Framework Core, and Oracle, with support for modern development and deployment workflows.

## Prerequisites
- Visual Studio Professional 2022 (17.14.7) or higher ([Download](https://visualstudio.microsoft.com/downloads/))
  - Workloads: ASP.NET and web development, Azure, .NET desktop development
- (Optional) JetBrains Rider 2025.1.0 or higher ([Download](https://www.jetbrains.com/rider/))
- (Optional) Latest version of Chrome ([Download](https://www.google.com/chrome/))
- Access to ArtifactoryCloud NuGet package source
- Oracle database access (for development/testing)
- `secrets.json` file from a team member ([Safe storage of app secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#manage-user-secrets-with-visual-studio))

## Setup Instructions
1. Clone the repository:git clone https://bitbucket.org/demoulas/smart-profit-sharing
2. Open the solution:
   - Navigate to `src/Services`
   - Open `Demoulas.ProfitSharing.slnx` in Visual Studio
3. Set `Demoulas.ProfitSharing.AppHost` as the startup project
4. Ensure ArtifactoryCloud NuGet source is configured:dotnet nuget list source   
   - If missing, follow [this guide](https://demoulas.atlassian.net/wiki/spaces/JFD/pages/133726274/Add+Custom+NuGet+source+for+JFrog+Cloud)
5. Obtain `secrets.json` from a team member
6. Press F5/Run

## Build and Test
- Build the solution:dotnet build Demoulas.ProfitSharing.Api.csproj --configuration Release --runtime win-x64 /p:SourceRevisionId=$(git rev-parse --short HEAD)- Run tests:dotnet test Demoulas.ProfitSharing.Tests.csproj --configuration debug
## Usage
- To publish an image:dotnet publish -c Release -p:PublishProfile=FolderProfile -o ./publishOutput
PowerShell -ExecutionPolicy Bypass -File .\utilities\generateBuildInfo.ps1
Compress-Archive -Path ./publishOutput/* -DestinationPath ./Demoulas.ProfitSharing.Api.zip -Force
- EntityFramework tools:dotnet tool update --global dotnet-ef
dotnet ef migrations add {migrationName} --context ProfitSharingDbContext
dotnet ef migrations script --context ProfitSharingDbContext --output {FILE}
- Database management:
  - In place upgrade: `Demoulas.ProfitSharing.Data.Cli upgrade-db --connection-name ProfitSharing`
  - Drop/rebuild schema: `Demoulas.ProfitSharing.Data.Cli drop-recreate-db --connection-name ProfitSharing`
  - Import from Ready: `Demoulas.ProfitSharing.Data.Cli import-from-ready --connection-name ProfitSharing --sql-file ".\src\database\ready_import\SQL copy all from ready to smart ps.sql" --source-schema "PROFITSHARE"`
  - Generate DGML: `Demoulas.ProfitSharing.Data.Cli generate-dgml --connection-name ProfitSharing --output-file ProfitSharing.dgml`
  - Generate markdown: `Demoulas.ProfitSharing.Data.Cli generate-markdown --connection-name ProfitSharing --output-file ProfitSharing.md`

## Project Structure
- `src/Services/` - Main API and worker service projects
- `src/Services/tests/` - Unit and integration tests
- `src/database/` - Database scripts and migrations

## Technologies Used
- [Fast-Endpoints](https://fast-endpoints.com/) | [REPR Design Pattern](https://deviq.com/design-patterns/repr-design-pattern)
- [EntityFramework.Core 9 w/Oracle](https://www.nuget.org/packages/Oracle.EntityFrameworkCore)
- [.editorconfig](https://learn.microsoft.com/en-us/visualstudio/ide/create-portable-custom-editor-options?view=vs-2022)
- [Directory.Build.Props](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022)
- [Centralized package management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [Fluent Assertions](https://fluentassertions.com/)
- [RabbitMq](https://hub.docker.com/r/masstransit/rabbitmq)
- [MassTransit](https://masstransit.io/)
- [Bogus](https://github.com/bchavez/Bogus)
- [Riok.Mapperly](https://mapperly.riok.app)
- [Serilog](https://serilog.net/)
- [.NET Feature Management](https://github.com/microsoft/FeatureManagement-Dotnet)

## Suggested Visual Studio Extensions
- [Productivity Power Tools 2022](https://marketplace.visualstudio.com/items?itemName=VisualStudioPlatformTeam.ProductivityPowerPack2022)
- [Visual Studio Spell Checker (VS2022 and Later)](https://marketplace.visualstudio.com/items?itemName=EWoodruff.VisualStudioSpellCheckerVS2022andLater)

## Good Reads
- [Minimal APIs overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-8.0)
- [RESTful web API design](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [Web API implementation](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-implementation)
- [Caching guidance](https://learn.microsoft.com/en-us/azure/architecture/best-practices/caching)
- [Performance testing and antipatterns for cloud applications](https://learn.microsoft.com/en-us/azure/architecture/antipatterns/#catalog-of-antipatterns)
- [Use feature flags in an ASP.NET Core app](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core)
- [EF.Core Bulk updates](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates)
- [Async return types (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types)

## Contributing
Contributions are welcome! Please open issues or submit pull requests for improvements or bug fixes.

## Contact
For support or questions, reach out to the project maintainers via Bitbucket or your team lead.
