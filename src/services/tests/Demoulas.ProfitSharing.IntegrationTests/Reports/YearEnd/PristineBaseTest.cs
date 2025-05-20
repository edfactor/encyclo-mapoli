using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.ItOperations;
using Demoulas.Security;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

public abstract class PristineBaseTest
{
    protected readonly AccountingPeriodsService Aps = new();
    protected readonly CalendarService CalendarService;
    protected readonly TotalService TotalService;
    protected readonly PristineDataContextFactory DbFactory;
    protected readonly ITestOutputHelper TestOutputHelper;

    protected PristineBaseTest(ITestOutputHelper testOutputHelper)
    {
        DbFactory = new PristineDataContextFactory();
        CalendarService = new CalendarService(DbFactory, Aps);
        TotalService = new TotalService(DbFactory, 
            CalendarService, new EmbeddedSqlService(), 
            new DemographicReaderService(new FrozenService(DbFactory), new HttpContextAccessor()));
        TestOutputHelper = testOutputHelper;
    }
}
