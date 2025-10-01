using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Moq;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Demoulas.ProfitSharing.Services.Calendars;
using Demoulas.ProfitSharing.Services.Accounting;
using Demoulas.ProfitSharing.Services.Demographics;
using Demoulas.ProfitSharing.Services.EmbeddedSql;
using Demoulas.ProfitSharing.Services.Frozen;
using Demoulas.ProfitSharing.Services.Totals;

// This is a debug script to quickly check population counts
public static class PopulationDebug
{
    public static async Task CheckPopulation()
    {
        Console.WriteLine("Checking current population status...");

        // Mock services setup
        var distributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var dbFactory = /* need DB factory */;
        var calendarService = new CalendarService(dbFactory, new AccountingPeriodsService(), distributedCache);
        var totalService = new TotalService(dbFactory,
            calendarService, new EmbeddedSqlService(),
            new DemographicReaderService(new FrozenService(dbFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object), new HttpContextAccessor()));
        DemographicReaderService demographicReaderService = new(new FrozenService(dbFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object), new HttpContextAccessor());
        TerminatedEmployeeService smartService = new TerminatedEmployeeService(dbFactory, totalService, demographicReaderService);

        DateOnly startDate = new DateOnly(2025, 01, 4);
        DateOnly endDate = new DateOnly(2025, 12, 27);

        var smartData = await smartService.GetReportAsync(new StartAndEndDateRequest { BeginningDate = startDate, EndingDate = endDate, Take = int.MaxValue, SortBy = "name" }, CancellationToken.None);

        Console.WriteLine($"Current SMART employee count: {smartData.Response.Results.Count()}");

        // Check for specific missing employees we know about
        string[] missingEmployees = { "ARIAS MAVERICK", "BUCKLEY LUCA", "CRAIG OWEN", "DELAROSA MILLIE" };
        foreach (var name in missingEmployees)
        {
            var found = smartData.Response.Results.Any(e => e.Name.Contains(name));
            Console.WriteLine($"{name}: {(found ? "FOUND" : "STILL MISSING")}");
        }
    }
}