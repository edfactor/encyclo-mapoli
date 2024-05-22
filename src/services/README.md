# Demoulas Accounts Receivable API project #

<hr/>

## Setup Instructions ##

### Getting Started
Install 3rd Party Libraries:

1. Visual Studio Professional 2022 (17.9.4) or higher installed - https://visualstudio.microsoft.com/downloads/

    ![Workload](./setup_images/workload.PNG)
    - Install the "ASP.net and web development", "Azure" workload, and ".NET desktop development" workloads
    - If VS requires any additional workloads or packages, you will be prompted at startup.

2. OPTIONAL - Latest version of Chrome web browser - https://www.google.com/chrome/


### Build and Test
1. Clone the git repository https://stash.demoulasmarketbasket.net/projects/NGA/repos/smart-accounts-receivable 
2. Open the Demoulas.AccountsReceivable solution
    - API Navigate to the \src\Services folder
    - Locate Demoulas.AccountsReceivable.sln and open with Visual Studio.
2. Set the 'Demoulas.AccountsReceivable.Api' project as your startup project
    - Find the project, right click and choose "set as startup project"
3. Get secrets.json from one of the team members
    - [Safe storage of app secrets in development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#manage-user-secrets-with-visual-studio)
4. F5/Run, it's that simple.

# Note-worthy technology #

1. [Safe storage of app secrets in development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#enable-secret-storage)
2. [Fast-Endpoints](https://fast-endpoints.com/)
3. [EntityFramework.Core 8 w/Oracle](https://www.nuget.org/packages/Oracle.EntityFrameworkCore)
4. [.editorconfig](https://learn.microsoft.com/en-us/visualstudio/ide/create-portable-custom-editor-options?view=vs-2022)
5. [Directory.Build.Props](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022)
6. [Centralized package management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
7. [BenchmarkDotNet](https://benchmarkdotnet.org/)
8. [Makes use of .NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
9. [Fluent Assertions](https://fluentassertions.com/)
10. [RabbitMq](https://hub.docker.com/r/masstransit/rabbitmq)
11. [MassTransit](https://masstransit.io/)
12. [Bogus](https://github.com/bchavez/Bogus)
13. [Riok.Mapperly](https://mapperly.riok.app)
14. [Serilog](https://serilog.net/)
15. [.NET Feature Management](https://github.com/microsoft/FeatureManagement-Dotnet)


<hr/>

### Suggested Visual Studio Extensions

- [Productivity Power Tools 2022](https://marketplace.visualstudio.com/items?itemName=VisualStudioPlatformTeam.ProductivityPowerPack2022)
- [Visual Studio Spell Checker (VS2022 and Later)](https://marketplace.visualstudio.com/items?itemName=EWoodruff.VisualStudioSpellCheckerVS2022andLater)

<hr/>

### Good reads
- [Minimal APIs overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-8.0)
- [RESTful web API design](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [Web API implementation](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-implementation)
- [Caching guidance](https://learn.microsoft.com/en-us/azure/architecture/best-practices/caching)
- [Performance testing and antipatterns for cloud applications](https://learn.microsoft.com/en-us/azure/architecture/antipatterns/#catalog-of-antipatterns)
- [Use feature flags in an ASP.NET Core app](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core)
- [EF.Core Bulk updates](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates)
- [Async return types (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types)

<hr/>

### Databases
- Oracle

### EntityFramework Tools
dotnet tool update --global dotnet-ef


### EntityFramework Migrations
dotnet ef migrations add {migrationName} --context OracleDbContext


### EntityFramework scaffold
dotnet ef dbcontext scaffold "{Data Source}" Oracle.EntityFrameworkCore -o Models -t AR_ACTIVE -t AR_ADJUSTMENT -t AR_DEPOSIT -t AR_DEPOSIT_DETAIL -t AR_TEMP


<hr/>

# Docker Publish Example
## Reference [dotnet publish](https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container?pivots=dotnet-8-0)
dotnet publish -c Release -p:PublishProfile=FolderProfile -o ./publishOutput
PowerShell -ExecutionPolicy Bypass -File .\utilities\generateBuildInfo.ps1

Compress-Archive -Path ./publishOutput/* -DestinationPath ./Demoulas.AccountsReceivable.Api.zip -Force



# Build command line
[dotnet build](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build)
[.NET Runtime Identifier Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)
using Powershell: dotnet build Demoulas.AccountsReceivable.Api.csproj --configuration Release --runtime win-x64 /p:SourceRevisionId=$(git rev-parse --short HEAD)



# Run tests
dotnet test Demoulas.AccountsReceivable.Tests.csproj --configuration debug
