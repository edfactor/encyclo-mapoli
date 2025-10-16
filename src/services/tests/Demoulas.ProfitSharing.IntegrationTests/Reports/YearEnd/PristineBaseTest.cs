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

    protected PristineBaseTest(ITestOutputHelper testOutputHelper)
    {
        DbFactory = new PristineDataContextFactory();
        DistributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        CalendarService = new CalendarService(DbFactory, Aps, DistributedCache);
        FrozenService = new FrozenService(DbFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object, DistributedCache, new Mock<INavigationService>().Object);
        EmbeddedSqlService = new EmbeddedSqlService();
        DemographicReaderService = new DemographicReaderService(FrozenService, HttpContextAccessor);
        TotalService = new TotalService(DbFactory, CalendarService, EmbeddedSqlService, DemographicReaderService);
        TestOutputHelper = testOutputHelper;
    }
}
