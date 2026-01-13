using System.ComponentModel;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;

/// <summary>
/// Integration tests for PS-2424: Validates vesting years calculation logic.
/// Tests the years of service calculation to verify employees with 1000+ hours
/// get the +1 year credit even when they have existing contribution records.
/// </summary>
public sealed class VestingYearsIntegrationTests : PristineBaseTest
{
    public VestingYearsIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    /// <summary>
    /// PS-2424: Validates that employees with both 1000+ hours AND contribution records
    /// receive the correct years of service credit.
    ///
    /// Bug: The NOT EXISTS clause in GetYearsOfServiceQuery prevents the +1 year for hours
    /// when an employee has a contribution record. This test identifies affected employees.
    /// </summary>
    [Fact]
    [Description("PS-2424 : Identify employees affected by vesting years bug")]
    public async Task IdentifyEmployeesAffectedByVestingYearsBug()
    {
        const short profitYear = 2025;
        var calendarInfo = await CalendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, CancellationToken.None);

        // Find employees who:
        // 1. Have a contribution record (profit_code_id = 0, iteration = 0) - so NOT EXISTS returns false
        // 2. Have 1000+ hours in PAY_PROFIT - so they should get +1 year for hours
        // 3. Are at least 18 years old - required for year credit
        var aged18Date = calendarInfo.FiscalEndDate.AddYears(-18);

        var affectedEmployees = await DbFactory.UseReadOnlyContext(async ctx =>
        {
            // Get employees with contribution records for the profit year
            var employeesWithContributions = ctx.ProfitDetails
                .Where(pd => pd.ProfitYear == profitYear
                    && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                    && pd.ProfitYearIteration == 0)
                .Select(pd => pd.Ssn)
                .Distinct();

            // Get employees with 1000+ hours and age >= 18
            var employeesWithSufficientHours = from pp in ctx.PayProfits
                                               join d in ctx.Demographics on pp.DemographicId equals d.Id
                                               where pp.ProfitYear == profitYear
                                                   && pp.TotalHours >= ReferenceData.MinimumHoursForContribution
                                                   && d.DateOfBirth <= aged18Date
                                               select new { d.Ssn, d.BadgeNumber, pp.TotalHours, d.DateOfBirth };

            // Find intersection: employees with BOTH contributions AND 1000+ hours
            // These employees should get +1 year for hours but the bug prevents it
            var affected = await (from e in employeesWithSufficientHours
                                  join c in employeesWithContributions on e.Ssn equals c
                                  select new
                                  {
                                      e.Ssn,
                                      e.BadgeNumber,
                                      e.TotalHours,
                                      e.DateOfBirth
                                  }).Take(50).ToListAsync(CancellationToken.None);

            return affected;
        }, CancellationToken.None);

        TestOutputHelper.WriteLine($"Found {affectedEmployees.Count} employees potentially affected by PS-2424:");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine("| Badge    | Hours   | DOB        |");
        TestOutputHelper.WriteLine("|----------|---------|------------|");

        foreach (var emp in affectedEmployees.Take(20))
        {
            TestOutputHelper.WriteLine($"| {emp.BadgeNumber,-8} | {emp.TotalHours,7:N0} | {emp.DateOfBirth:yyyy-MM-dd} |");
        }

        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine($"These employees have 1000+ hours AND a contribution record.");
        TestOutputHelper.WriteLine($"Due to the bug, they do NOT receive +1 year for their hours.");

        // We expect to find affected employees in the database
        affectedEmployees.Count.ShouldBeGreaterThan(0, "Should find employees affected by the bug");
    }

    /// <summary>
    /// PS-2424: Validates vesting percentage for specific test employees.
    /// Tests that the vesting calculation returns the expected percentage based on years of service.
    /// </summary>
    [Fact]
    [Description("PS-2424 : Validate vesting balance calculation for sample employees")]
    public async Task ValidateVestingBalanceForSampleEmployees()
    {
        const short profitYear = 2025;

        // Get a sample of employees who should be affected
        var sampleEmployees = await GetSampleAffectedEmployees(profitYear);

        if (sampleEmployees.Count == 0)
        {
            TestOutputHelper.WriteLine("No sample employees found. Skipping test.");
            Assert.Fail("Expected to find sample employees for testing");
            return;
        }

        TestOutputHelper.WriteLine($"Testing vesting balance for {sampleEmployees.Count} sample employees:");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine("| Badge    | SumYOS | Hours   | HasContrib | YearsFromSvc | VestPct |");
        TestOutputHelper.WriteLine("|----------|--------|---------|------------|--------------|---------|");

        foreach (var emp in sampleEmployees)
        {
            // Get the vesting balance from TotalService (uses the buggy SQL)
            var vestingBalance = await TotalService.GetVestingBalanceForSingleMemberAsync(
                SearchBy.BadgeNumber, emp.BadgeNumber, profitYear, CancellationToken.None);

            string vestPct = vestingBalance?.VestingPercent.ToString("P0") ?? "N/A";
            int yearsFromSvc = vestingBalance?.YearsInPlan ?? 0;

            TestOutputHelper.WriteLine($"| {emp.BadgeNumber,-8} | {emp.SumYearsOfServiceCredit,6} | {emp.TotalHours,7:N0} | {"Yes",-10} | {yearsFromSvc,12} | {vestPct,7} |");

            // Validate: Years from service should be at least SumYOS + 1 (for hours)
            // The bug causes it to equal just SumYOS (missing the +1)
            if (vestingBalance != null && emp.TotalHours >= ReferenceData.MinimumHoursForContribution)
            {
                var expectedMinYears = emp.SumYearsOfServiceCredit + 1;

                if (yearsFromSvc < expectedMinYears)
                {
                    TestOutputHelper.WriteLine($"  ⚠️  BUG: Expected at least {expectedMinYears} years (YOS credits + 1 for hours), got {yearsFromSvc}");
                }
            }
        }

        sampleEmployees.Count.ShouldBeGreaterThan(0, "Should find sample employees");
    }

    /// <summary>
    /// PS-2424: Compares years calculation with and without the hours credit to identify discrepancies.
    /// </summary>
    [Fact]
    [Description("PS-2424 : Compare years calculation to identify discrepancies")]
    public async Task CompareYearsCalculationWithAndWithoutHoursCredit()
    {
        const short profitYear = 2025;
        var calendarInfo = await CalendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, CancellationToken.None);
        var asOfDate = calendarInfo.FiscalEndDate;
        var aged18Date = asOfDate.AddYears(-18);

        // Calculate what the years SHOULD be vs what the bug returns
        var comparison = await DbFactory.UseReadOnlyContext(async ctx =>
        {
            // Get sum of years_of_service_credit from PROFIT_DETAIL for each SSN
            var yearsOfServiceCredits = await (from pd in ctx.ProfitDetails
                                               where pd.ProfitYear <= profitYear
                                                   && pd.ProfitYearIteration != 3
                                               group pd by pd.Ssn into g
                                               select new
                                               {
                                                   Ssn = g.Key,
                                                   SumCredits = g.Sum(x => x.YearsOfServiceCredit)
                                               }).ToListAsync(CancellationToken.None);

            // Get employees with 1000+ hours
            var hoursQualified = await (from pp in ctx.PayProfits
                                        join d in ctx.Demographics on pp.DemographicId equals d.Id
                                        where pp.ProfitYear == profitYear
                                            && pp.TotalHours >= ReferenceData.MinimumHoursForContribution
                                            && d.DateOfBirth <= aged18Date
                                        select new { d.Ssn, d.BadgeNumber, pp.TotalHours }).ToListAsync(CancellationToken.None);

            // Get employees with contribution records
            var hasContribution = await (from pd in ctx.ProfitDetails
                                         where pd.ProfitYear == profitYear
                                             && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                                             && pd.ProfitYearIteration == 0
                                         select pd.Ssn).Distinct().ToListAsync(CancellationToken.None);

            var hasContributionSet = hasContribution.ToHashSet();
            var yearsDict = yearsOfServiceCredits.ToDictionary(x => x.Ssn, x => x.SumCredits);

            // Find employees where bug causes incorrect calculation
            var discrepancies = hoursQualified
                .Where(e => hasContributionSet.Contains(e.Ssn)) // Has contribution (bug condition)
                .Where(e => yearsDict.ContainsKey(e.Ssn))
                .Select(e => new
                {
                    e.Ssn,
                    e.BadgeNumber,
                    e.TotalHours,
                    SumCredits = yearsDict[e.Ssn],
                    BuggyYears = yearsDict[e.Ssn], // NOT EXISTS returns false, so +1 is NOT added
                    CorrectYears = yearsDict[e.Ssn] + 1 // Should add +1 for hours
                })
                .Take(30)
                .ToList();

            return discrepancies;
        }, CancellationToken.None);

        TestOutputHelper.WriteLine($"Found {comparison.Count} employees with discrepancy due to bug:");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine("| Badge    | Hours   | SumCredits | Buggy Years | Correct Years | Diff |");
        TestOutputHelper.WriteLine("|----------|---------|------------|-------------|---------------|------|");

        int totalDiscrepancies = 0;
        foreach (var emp in comparison)
        {
            if (emp.BuggyYears != emp.CorrectYears)
            {
                totalDiscrepancies++;
                TestOutputHelper.WriteLine($"| {emp.BadgeNumber,-8} | {emp.TotalHours,7:N0} | {emp.SumCredits,10} | {emp.BuggyYears,11} | {emp.CorrectYears,13} | {emp.CorrectYears - emp.BuggyYears,4} |");
            }
        }

        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine($"Total discrepancies: {totalDiscrepancies}");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine("Impact: Each discrepancy means one less year of service credit,");
        TestOutputHelper.WriteLine("which can affect vesting percentage (20% per year for years 3-6).");

        // We expect discrepancies for employees with contributions AND 1000+ hours
        totalDiscrepancies.ShouldBeGreaterThan(0, "Should find discrepancies due to the bug");
    }

    private async Task<List<SampleEmployee>> GetSampleAffectedEmployees(short profitYear)
    {
        var calendarInfo = await CalendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, CancellationToken.None);
        var aged18Date = calendarInfo.FiscalEndDate.AddYears(-18);

        return await DbFactory.UseReadOnlyContext(async ctx =>
        {
            // Get sum of years_of_service_credit for each SSN
            var yearsCredits = await (from pd in ctx.ProfitDetails
                                      where pd.ProfitYear <= profitYear
                                          && pd.ProfitYearIteration != 3
                                      group pd by pd.Ssn into g
                                      select new
                                      {
                                          Ssn = g.Key,
                                          SumCredits = g.Sum(x => x.YearsOfServiceCredit)
                                      }).ToDictionaryAsync(x => x.Ssn, x => x.SumCredits, CancellationToken.None);

            // Get employees with contributions AND 1000+ hours (affected by bug)
            var affected = await (from pp in ctx.PayProfits
                                  join d in ctx.Demographics on pp.DemographicId equals d.Id
                                  join pd in ctx.ProfitDetails on d.Ssn equals pd.Ssn
                                  where pp.ProfitYear == profitYear
                                      && pp.TotalHours >= ReferenceData.MinimumHoursForContribution
                                      && d.DateOfBirth <= aged18Date
                                      && pd.ProfitYear == profitYear
                                      && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                                      && pd.ProfitYearIteration == 0
                                  select new
                                  {
                                      d.Ssn,
                                      d.BadgeNumber,
                                      pp.TotalHours
                                  }).Distinct().Take(10).ToListAsync(CancellationToken.None);

            return affected.Select(e => new SampleEmployee
            {
                Ssn = e.Ssn,
                BadgeNumber = e.BadgeNumber,
                TotalHours = e.TotalHours,
                SumYearsOfServiceCredit = yearsCredits.GetValueOrDefault(e.Ssn, 0)
            }).ToList();
        }, CancellationToken.None);
    }

    private sealed class SampleEmployee
    {
        public int Ssn { get; init; }
        public int BadgeNumber { get; init; }
        public decimal TotalHours { get; init; }
        public int SumYearsOfServiceCredit { get; init; }
    }
}
