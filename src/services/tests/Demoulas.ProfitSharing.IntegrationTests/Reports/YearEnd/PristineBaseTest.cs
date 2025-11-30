using System.Reflection;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

public abstract class PristineBaseTest
{
    protected readonly AccountingPeriodsService Aps = new();
    protected readonly CalendarService CalendarService;
    protected readonly TotalService TotalService;
    protected readonly PristineDataContextFactory DbFactory;
    protected readonly ITestOutputHelper TestOutputHelper;
    protected readonly IDemographicReaderService DemographicReaderService;
    protected readonly IFrozenService FrozenService;
    protected readonly IEmbeddedSqlService EmbeddedSqlService;
    protected readonly IHttpContextAccessor HttpContextAccessor = new HttpContextAccessor();
    protected readonly MemoryDistributedCache DistributedCache;
    protected readonly IPayProfitUpdateService PayProfitUpdateService;
    protected readonly YearEndService YearEndService;

    protected PristineBaseTest(ITestOutputHelper testOutputHelper)
    {
        // It is reasonable to ask why we are not using an injection framework here.
        // Presumably this is enough wiring to get the job done, without having to exclude things which would be problematic
        DbFactory = new PristineDataContextFactory();
        DistributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        CalendarService = new CalendarService(DbFactory, Aps, DistributedCache);
        FrozenService = new FrozenService(DbFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object, DistributedCache,
            new Mock<INavigationService>().Object);
        EmbeddedSqlService = new EmbeddedSqlService();
        DemographicReaderService = new DemographicReaderService(FrozenService, HttpContextAccessor);
        TotalService = new TotalService(DbFactory, CalendarService, EmbeddedSqlService, DemographicReaderService);
        TestOutputHelper = testOutputHelper;
        PayProfitUpdateService = new PayProfitUpdateService(DbFactory, NullLoggerFactory.Instance, TotalService, CalendarService);
        YearEndService = new YearEndService(DbFactory, CalendarService, PayProfitUpdateService, TotalService, DemographicReaderService);
    }

    public static string ReadEmbeddedResource(string resourceName)
    {
        if (resourceName.StartsWith('.'))
        {
            resourceName = "Demoulas.ProfitSharing.IntegrationTests.Resources" + resourceName;
        }
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using StreamReader reader = new(stream!);
        return reader.ReadToEnd();
    }
}
