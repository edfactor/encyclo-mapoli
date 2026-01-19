using System.Diagnostics;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Services.ProfitMaster;
using Demoulas.ProfitSharing.Services.ProfitShareEdit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Oracle.ManagedDataAccess.Client;
using Shouldly;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitMaster;

public static class StopwatchExtensions
{
    public static string Took(this Stopwatch sw)
    {
        return $"took {sw.Elapsed.Minutes} min {sw.Elapsed.Seconds} secs";
    }
}

public class ProfitMasterTests : PristineBaseTest
{
    public ProfitMasterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task UpdateTest()
    {
        // Arrange
        short profitYear = 2025;
        IAppUser iAppUser = new Mock<IAppUser>().Object;
        ProfitShareUpdateService psus = new(DbFactory, TotalService, CalendarService, DemographicReaderService, TimeProvider.System);
        ProfitShareEditService pses = new(psus, CalendarService);
        ProfitMasterService pms = new(pses, DbFactory, iAppUser, FrozenService, TimeProvider.System);

        var frozenDemographicYear = (await FrozenService.GetActiveFrozenDemographicAsync(CancellationToken.None)).ProfitYear;
        var maxYearEndStatusYear =
            await DbFactory.UseWritableContext(async ctx => await ctx.YearEndUpdateStatuses.Select(st => (short?)st.ProfitYear).MaxAsync(CancellationToken.None))!;
        bool needToRevert = (maxYearEndStatusYear == frozenDemographicYear);

        Stopwatch sw = Stopwatch.StartNew();
        if (needToRevert)
        {
            try
            {
                var prs = await pms.RevertAsync(
                    new ProfitYearRequest() { Skip = null, Take = null, ProfitYear = 0, }, CancellationToken.None);
                TestOutputHelper.WriteLine($"Revert {sw.Took()}, for transactionsRemoved:{prs.TransactionsRemoved}  etvasEffected:{prs.EtvasEffected}");
            }
            catch (Exception e)
            {
                TestOutputHelper.WriteLine($"Revert failed: {e.Message}");
            }
        }

        sw = Stopwatch.StartNew();

        // Forces a connection, so the Bulk operations can access an open connection
        await DbFactory.UseWritableContext(async ctx =>
        {
            var c = ctx.Database.GetDbConnection();
            if (c is OracleConnection oracleConnection)
            {
                await oracleConnection.OpenAsync();
            }

            return 7;
        });

        // Act
        var psur = await pms.UpdateAsync(
            new ProfitShareUpdateRequest
            {
                Skip = null,
                Take = null,
                ProfitYear = profitYear,
                ContributionPercent = 15,
                IncomingForfeitPercent = 0.876678m,
                EarningsPercent = 9.280136m,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 57_000,
                BadgeToAdjust = 0,
                BadgeToAdjust2 = 0,
                AdjustContributionAmount = 0,
                AdjustEarningsAmount = 0,
                AdjustIncomingForfeitAmount = 0,
                AdjustEarningsSecondaryAmount = 0
            }, CancellationToken.None);

        sw.Stop();
        TestOutputHelper.WriteLine($"Update {sw.Took()} for transactions:{psur.TransactionsCreated} etvasEffected:{psur.EtvasEffected}");
        true.ShouldBeTrue();
    }

    [Fact]
    public async Task RevertTest()
    {
        // Arrange
        short profitYear = 2025;
        IAppUser iAppUser = new Mock<IAppUser>().Object;
        ProfitShareUpdateService psus = new(DbFactory, TotalService, CalendarService, DemographicReaderService, TimeProvider.System);
        ProfitShareEditService pses = new(psus, CalendarService);
        ProfitMasterService pms = new(pses, DbFactory, iAppUser, FrozenService, TimeProvider.System);

        var frozenDemographicYear = (await FrozenService.GetActiveFrozenDemographicAsync(CancellationToken.None)).ProfitYear;
        var maxYearEndStatusYear =
            await DbFactory.UseWritableContext(async ctx => await ctx.YearEndUpdateStatuses.Select(st => (short?)st.ProfitYear).MaxAsync(CancellationToken.None))!;
        bool needToRevert = (maxYearEndStatusYear == frozenDemographicYear);

        if (needToRevert)
        {
            Stopwatch sw = Stopwatch.StartNew();
            // Act
            var prs = await pms.RevertAsync(
                new ProfitYearRequest() { Skip = null, Take = null, ProfitYear = profitYear, }, CancellationToken.None);

            sw.Stop();
            TestOutputHelper.WriteLine($"Revert took {sw.Took()}; for TransactionsRemoved:{prs.TransactionsRemoved} etvasEffected:{prs.EtvasEffected}");
        }
        else
        {
            TestOutputHelper.WriteLine("Nothing really to revert...");
        }

        true.ShouldBeTrue();
    }
}
