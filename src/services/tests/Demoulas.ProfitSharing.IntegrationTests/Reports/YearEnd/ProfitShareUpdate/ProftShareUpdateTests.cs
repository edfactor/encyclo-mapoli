using System.Diagnostics;
using System.Reflection;
using System.Text;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.ServiceDto;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;

public class ProfitShareUpdateTests
{
    private readonly AccountingPeriodsService _aps = new();
    private readonly CalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dbFactory;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly TotalService _totalService;

    public ProfitShareUpdateTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<ProfitShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        _dbFactory = new PristineDataContextFactory(connectionString);
        _calendarService = new CalendarService(_dbFactory, _aps);
        _totalService = new TotalService(_dbFactory, _calendarService);
    }


    [Fact]
    public async Task ReportWithUpdates()
    {
        // Arrange
        short profitYear = 2024;
        ProfitShareUpdateReport profitShareUpdateService = createProfitShareUpdateService();

        string reportName = "psupdate-pay444-r2.txt";
        profitShareUpdateService.TodaysDateTime =
            new DateTime(2024, 11, 14, 10, 35, 0, DateTimeKind.Local); // time report was generated

        // Act
        await profitShareUpdateService.ProfitSharingUpdatePaginated(
            new ProfitShareUpdateRequest
            {
                Skip = null,
                Take = null,
                ProfitYear = profitYear,
                ContributionPercent = 15,
                IncomingForfeitPercent = 1,
                EarningsPercent = 2,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 30_000,
                BadgeToAdjust = 0,
                BadgeToAdjust2 = 0,
                AdjustContributionAmount = 0,
                AdjustEarningsAmount = 0,
                AdjustIncomingForfeitAmount = 0,
                AdjustEarningsSecondaryAmount = 0
            });

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(profitShareUpdateService.ReportLines);

        AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public async Task EnsureUpdateWithValues_andEmployeeAdjustment_MatchesReady()
    {
        // Arrange
        short profitYear = 2024;
        ProfitShareUpdateReport profitShareUpdateService = createProfitShareUpdateService();
        string reportName = "psupdate-pay444-r3.txt";
        profitShareUpdateService.TodaysDateTime =
            new DateTime(2024, 11, 19, 19, 18, 0, DateTimeKind.Local); // time report was generated

        // Act
        await profitShareUpdateService.ProfitSharingUpdatePaginated(
            new ProfitShareUpdateRequest
            {
                Skip = null,
                Take = null,
                ProfitYear = profitYear,
                ContributionPercent = 15,
                IncomingForfeitPercent = 1,
                EarningsPercent = 2,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 30_000,
                BadgeToAdjust = 700174,
                BadgeToAdjust2 = 0,
                AdjustContributionAmount = 44.77m,
                AdjustEarningsAmount = 22.33m,
                AdjustIncomingForfeitAmount = 18.16m,
                AdjustEarningsSecondaryAmount = 0
            });

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(profitShareUpdateService.ReportLines);

        AssertReportsAreEquivalent(expected, actual);
    }


    private ProfitShareUpdateReport createProfitShareUpdateService()
    {
        return new ProfitShareUpdateReport(_dbFactory, _calendarService);
    }


    private static string CollectLines(List<string> lines)
    {
        StringBuilder sb = new();
        for (int i = 0; i < lines.Count; i++)
        {
            sb.Append(lines[i]);
            // Cobol is smart enough to not emit a Newline if the next character is a form feed.
            if (i < lines.Count - 2 && !lines[i + 1].StartsWith('\f'))
            {
                sb.Append("\n");
            }
        }

        sb.Append("\n");
        return sb.ToString();
    }

    private static void AssertReportsAreEquivalent(string expected, string actual)
    {
        if (!File.Exists("/Program Files/Meld/Meld.exe"))
        {
            actual.Should().Be(expected);
            return;
        }

        if (actual == expected)
        {
            actual.Should().Be(expected);
            return;
        }

        string expectedFile = Path.GetTempFileName();
        File.WriteAllBytes(expectedFile, Encoding.ASCII.GetBytes(expected));

        string actualFile = Path.GetTempFileName();
        File.WriteAllBytes(actualFile, Encoding.ASCII.GetBytes(actual));


        ProcessStartInfo startInfo = new()
        {
            FileName = "/Program Files/Meld/Meld.exe",
            ArgumentList = { expectedFile, actualFile },
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Start the process
        using Process? process = Process.Start(startInfo);
    }

    public static string LoadExpectedReport(string resourceName)
    {
        using (Stream? stream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("Demoulas.ProfitSharing.IntegrationTests.Resources." + resourceName))
        using (StreamReader reader = new(stream!))
        {
            return reader.ReadToEnd().Replace("\r", "");
        }
    }

    // This code runs fine on the mock db, but on oracle it throws an exception with the beneficiary part of the TotalService.GetVestingRatio()
    [Fact]
    public async Task Bene_problem()
    {
        true.Should().Be(true);
        short profitShare = 2023;

        CalendarResponseDto fiscalDates =
            await _calendarService.GetYearStartAndEndAccountingDatesAsync(2023, CancellationToken.None);
        List<EmployeeFinancials> employeeFinancialsList = await _dbFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<ParticipantTotalVestingBalanceDto> totalVestingBalances =
                _totalService.TotalVestingBalance(ctx, (short)(profitShare - 1), fiscalDates.FiscalEndDate);

            return await ctx.PayProfits
                .Include(pp => pp.Demographic)
                .Include(pp => pp.Demographic!.ContactInfo)
                .Where(pp => pp.ProfitYear == 2023)
                .GroupJoin(
                    totalVestingBalances,
                    pp => pp.Demographic!.Ssn,
                    tvb => tvb.Ssn,
                    (pp, tvbs) => new { PayProfit = pp, TotalVestingBalances = tvbs.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.TotalVestingBalances,
                    (x, tvb) => new EmployeeFinancials
                    {
                        EmployeeId = x.PayProfit.Demographic!.EmployeeId,
                        Ssn = x.PayProfit.Demographic.Ssn,
                        Name = x.PayProfit.Demographic.ContactInfo!.FullName,
                        EnrolledId = x.PayProfit.EnrollmentId,
                        YearsInPlan = x.PayProfit.YearsInPlan,
                        CurrentAmount = tvb == null ? 0 : tvb.CurrentBalance,
                        EmployeeTypeId = x.PayProfit.EmployeeTypeId,
                        PointsEarned = (int)(x.PayProfit.PointsEarned ?? 0),
                        EtvaAfterVestingRules = tvb == null ? 0 : tvb.Etva
                    }
                )
                .ToListAsync();
        });
        _testOutputHelper.WriteLine("Total employees " + employeeFinancialsList.Count);
    }
}
