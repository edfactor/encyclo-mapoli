using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Reporting.Extensions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

internal static class ProfitSharingArchitectureFixture
{
    internal static readonly System.Reflection.Assembly EndpointsAssembly = typeof(ProfitSharingEndpoint).Assembly;
    internal static readonly System.Reflection.Assembly ServicesAssembly = typeof(YearEndService).Assembly;
    internal static readonly System.Reflection.Assembly DataAssembly = typeof(ProfitSharingDbContext).Assembly;
    internal static readonly System.Reflection.Assembly CommonAssembly = typeof(EndpointTelemetry).Assembly;
    internal static readonly System.Reflection.Assembly SecurityAssembly = typeof(PolicyRoleMap).Assembly;
    internal static readonly System.Reflection.Assembly ReportingAssembly = typeof(ReportingServiceCollectionExtensions).Assembly;
    internal static readonly System.Reflection.Assembly CachingServicesAssembly = typeof(ServicesExtension).Assembly;
    private static readonly System.Reflection.Assembly _efCoreAssembly = typeof(DbContext).Assembly;
    private static readonly System.Reflection.Assembly _fastEndpointsAssembly = typeof(Endpoint).Assembly;
    private static readonly System.Reflection.Assembly _aspNetCoreHttpAssembly = typeof(HttpContext).Assembly;
    private static readonly System.Reflection.Assembly _memoryCacheAssembly = typeof(IMemoryCache).Assembly;

    internal static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            EndpointsAssembly,
            ServicesAssembly,
            DataAssembly,
            CommonAssembly,
            SecurityAssembly,
            ReportingAssembly,
            CachingServicesAssembly,
            _efCoreAssembly,
            _fastEndpointsAssembly,
            _aspNetCoreHttpAssembly,
            _memoryCacheAssembly)
        .Build();
}
