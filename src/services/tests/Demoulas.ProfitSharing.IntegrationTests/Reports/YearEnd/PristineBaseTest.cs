using System.Reflection;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

public abstract class PristineBaseTest
{
    protected readonly AccountingPeriodsService Aps = new(NullLogger<AccountingPeriodsService>.Instance);
    protected readonly CalendarService CalendarService;
    protected readonly TotalService TotalService;
    protected readonly PristineDataContextFactory DbFactory;
    protected readonly ITestOutputHelper TestOutputHelper;
    protected readonly IDemographicReaderService DemographicReaderService;
    protected readonly IFrozenService FrozenService;
    protected readonly IEmbeddedSqlService EmbeddedSqlService;
    protected readonly IHttpContextAccessor HttpContextAccessor = new HttpContextAccessor();
    protected readonly MemoryDistributedCache DistributedCache;
    protected readonly IVestingScheduleService VestingScheduleService;
    protected readonly IPayProfitUpdateService PayProfitUpdateService;
    protected readonly YearEndService YearEndService;
    protected readonly IForfeitureAdjustmentService ForfeitureAdjustmentService;
    protected readonly TimeProvider TimeProvider = TimeProvider.System;


    protected PristineBaseTest(ITestOutputHelper testOutputHelper)
    {
        // It is reasonable to ask why we are not using an injection framework here.
        // Presumably this is enough wiring to get the job done, without having to exclude things which would be problematic
        DbFactory = new PristineDataContextFactory();
        DistributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        CalendarService = new CalendarService(DbFactory, Aps, DistributedCache);
        FrozenService = new FrozenService(DbFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object, DistributedCache,
            new Mock<INavigationService>().Object, TimeProvider.System);
        EmbeddedSqlService = new EmbeddedSqlService();
        DemographicReaderService = new DemographicReaderService(FrozenService, HttpContextAccessor);
        TotalService = new TotalService(DbFactory, CalendarService, EmbeddedSqlService, DemographicReaderService);
        TestOutputHelper = testOutputHelper;
        VestingScheduleService = new VestingScheduleService(DbFactory, DistributedCache);
        PayProfitUpdateService = new PayProfitUpdateService(DbFactory, NullLoggerFactory.Instance, TotalService, CalendarService, VestingScheduleService);
        YearEndService = new YearEndService(DbFactory, CalendarService, PayProfitUpdateService, TotalService, DemographicReaderService, TimeProvider);
        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IAuditService>();
        ForfeitureAdjustmentService = new ForfeitureAdjustmentService(DbFactory, TotalService, DemographicReaderService, TimeProvider, mockAppUser.Object, mockAuditService.Object);
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


    protected Task<Employee> GetEmployeeByBadgeAsync(int badge)
    {
        var year = TimeProvider.GetLocalYear();

        return DbFactory.UseReadOnlyContext(async ctx =>
        {
            var demographic = await ctx.Demographics
                                  .FirstOrDefaultAsync(d => d.BadgeNumber == badge)
                              ?? throw new InvalidOperationException($"No demographic found for badge {badge}");

            var payProfit = await ctx.PayProfits
                                .FirstOrDefaultAsync(pp => pp.DemographicId == demographic.Id && pp.ProfitYear == year)
                            ?? throw new InvalidOperationException($"No PayProfit found for badge {badge} year {year}");

            return new Employee { Demographic = demographic, PayProfit = payProfit };
        });
    }

#pragma warning disable VSTHRD002

    protected long BadgeToSsn(int badge)
    {
        return GetEmployeeByBadgeAsync(badge).GetAwaiter().GetResult().Demographic.Ssn;
    }
}
