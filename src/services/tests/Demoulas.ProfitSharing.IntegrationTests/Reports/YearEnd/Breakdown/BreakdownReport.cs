using System.Diagnostics;
using System.Reflection;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.IntegrationTests.Fixtures;
using Demoulas.ProfitSharing.IntegrationTests.Helpers;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Reports.Breakdown;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;

// ReSharper disable once NotAccessedField.Local
#pragma warning disable S4487
#pragma warning disable IDE0052
#pragma warning disable AsyncFixer01
#pragma warning disable S2699
#pragma warning disable CS1998 // Async method lacks 'await' operators && will run synchronously
#pragma warning disable S125
#pragma warning disable S1172
#pragma warning disable S1144
#pragma warning disable S3358
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public class BreakdownReport : TestClassBase
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public BreakdownReport(ITestOutputHelper testOutputHelper, IntegrationTestsFixture fixture) : base(fixture)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }


    [Fact]
    public async Task CreateBreakdownReport()
    {
        short currentYear = 2024; // profitShareUpdateRequest.ProfitYear
        short priorYear = (short)(currentYear - 1);

        var configuration = new ConfigurationBuilder().AddUserSecrets<TotalServiceIntegrationTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        PristineDataContextFactory dataContextFactory = new PristineDataContextFactory(connectionString, true);


        var calendarService = new CalendarService(dataContextFactory, new AccountingPeriodsService());
        var totalService = new TotalService(dataContextFactory, calendarService);

        var sw = new Stopwatch();
        sw.Start();

        await dataContextFactory.UseReadOnlyContext<object>(async ctx =>
        {
            var dates = await calendarService.GetYearStartAndEndAccountingDatesAsync(currentYear);

            var summariesBySSn = ctx.ProfitDetails
                .Where(x => x.ProfitYear == currentYear)
                .GroupBy(details => details.Ssn)
                .Select(g => new
                {
                    g.Key,
                    TotalContributions = g.Sum(x => x.Contribution),
                    TotalEarnings = g.Sum(x => x.Earnings),
                    TotalForfeitures = g.Sum(x =>
                        x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                            ? x.Forfeiture
                            : (x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id ? -x.Forfeiture : 0)),
                    TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0),
                    Distribution = g.Sum(x =>
                        (x.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id ||
                         x.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments.Id ||
                         x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id)
                            ? -x.Forfeiture
                            : 0),
                    BeneficiaryAllocation = g.Sum(x => (x.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id)
                        ? -x.Forfeiture
                        : (x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id)
                            ? x.Contribution
                            : 0)
                })
                // .AsEnumerable()
                .Select(e => new
                {
                    Ssn = e.Key,
                    e.TotalContributions,
                    e.TotalEarnings,
                    e.TotalForfeitures,
                    e.TotalPayments,
                    e.Distribution,
                    e.BeneficiaryAllocation
                });

            // Lets get employees with Store, manger or not, && EC (ie plan type)
            var employeesQuery = ctx.PayProfits.Include(p => p.Demographic)
                .Where(pp => pp.ProfitYear == currentYear)
                .Join(totalService.GetVestingRatio(ctx, currentYear, dates.FiscalEndDate),
                    pp => pp.Demographic!.Ssn,
                    vr => vr.Ssn,
                    (pp, vr) => new { pp, VestingRatio = vr.Ratio })
                .Join(totalService.GetTotalBalanceSet(ctx, priorYear),
                    ppAndVestingRatio => ppAndVestingRatio.pp.Demographic!.Ssn,
                    tbs => tbs.Ssn,
                    (ppAndRatio, tbs) => new { ppAndRatio.pp, ppAndRatio.VestingRatio, BeginningBalance = tbs.Total })
                .Join(summariesBySSn,
                    collected => collected.pp.Demographic!.Ssn,
                    transactionSums => transactionSums.Ssn,
                    (collected, transactionSums) => new
                    {
                        collected.pp,
                        d = collected.pp.Demographic,
                        collected.VestingRatio,
                        collected.BeginningBalance,
                        transactionSums,
                        EndingBalance = collected.BeginningBalance + transactionSums.TotalContributions + transactionSums.TotalEarnings + transactionSums.TotalForfeitures +
                                        transactionSums.Distribution + transactionSums.BeneficiaryAllocation
                    }
                )
                .Select(coll => new MemberYearSummary
                {
                    BadgeNumber = coll.d!.BadgeNumber,
                    FullName = coll.d.ContactInfo.FullName!,
                    Ssn = coll.d.Ssn,
                    PayFrequencyId = coll.d.PayFrequencyId,
                    EnrollmentId = coll.pp.EnrollmentId,
                    StoreNumber = coll.d.StoreNumber,
                    DepartmentId = coll.d.DepartmentId,
                    PayClassificationId = coll.d.PayClassificationId,
                    BeginningBalance = coll.BeginningBalance,
                    Earnings = coll.transactionSums.TotalEarnings,
                    Contributions = coll.transactionSums.TotalContributions,
                    Forfeiture = coll.transactionSums.TotalForfeitures,
                    Distributions = coll.transactionSums.Distribution,
                    EndingBalance = coll.EndingBalance,
                    VestedAmount = coll.EndingBalance * coll.VestingRatio,
                    VestedPercentage = coll.VestingRatio * 100,
                    EmploymentStatusId = coll.d.EmploymentStatusId
                });

            var employees = (await employeesQuery.ToListAsync());
/*            foreach(var employee in employees)
            {
                if (employee.StoreNumber != 2)
                {
                    continue;
                }
                _testOutputHelper.WriteLine(employee.FullName+" "+employee.PayFrequencyId);
            }*/

            var groupedEmployees = employees
                .GroupBy(m => m.StoreNumber)
                .OrderBy(g => g.Key)
                .Select(storeGroup => new { StoreNumber = storeGroup.Key, employees = storeGroup.OrderBy(e => e.FullName).ToList() })
                .ToList();

            bool first = true;

            foreach (var store in groupedEmployees)
            {
                if (first)
                {
                    _testOutputHelper.WriteLine("DJDE JDE=LANIQS,JDL=DFLT4,END,;");
                    first = false;
                }
                else
                {
                    _testOutputHelper.WriteLine("FF");
                }

                _testOutputHelper.WriteLine("\nQPAY066TA               PROFIT SHARING BREAKDOWN REPORT    DATE FEB 18, 2025  YEAR:   2024.0     PAGE:   00001\n");

                _testOutputHelper.WriteLine($"Store: {store.StoreNumber}\n");

                _testOutputHelper.WriteLine("BADGE #     EMPLOYEE NAME              BEGINNING     EARNINGS         CONT         FORF        DIST.       ENDING      V E S T E D E");
                _testOutputHelper.WriteLine("                                         BALANCE                                                          BALANCE       AMOUNT  %  C");

                _testOutputHelper.WriteLine("\nSTORE MANAGEMENT\n");
                foreach (var employee in store.employees.Where(mys =>
                             EmployeeCategory(mys.DepartmentId, mys.PayClassificationId) != 1999
                             && mys.EmploymentStatusId == EmploymentStatus.Constants.Active
                             && mys.EndingBalance != 0
                         )
                        )
                {
                    PrintEmployee(employee);
                }

                _testOutputHelper.WriteLine("\nASSOCIATES\n");
                foreach (var employee in store.employees.Where(mys =>
                             EmployeeCategory(mys.DepartmentId, mys.PayClassificationId) == 1999
                             && mys.EmploymentStatusId == EmploymentStatus.Constants.Active
                             && mys.EndingBalance != 0
                         ))
                {
                    PrintEmployee(employee);
                }
            }

            return "j";
        });
    }


    public static string PropertiesToString(object instance)
    {
        if (instance == null)
        {
            return "big null";
        }

        return instance.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(p => $"{p.Name}: {p.GetValue(instance)}")
            .Aggregate((a, b) => $"{a}, {b}");
    }

    public static int EmployeeCategory(byte dept, byte pclass)
    {
        var sort = 1999;
        if (dept == 1 && pclass == 1)
        {
            sort = 10;
        }

        if (dept == 1 && pclass == 2)
        {
            sort = 20;
        }

        if (dept == 1 && pclass == 30)
        {
            sort = 30;
        }

        if (dept == 1 && pclass == 10)
        {
            sort = 40;
        }

        if (dept == 1 && pclass == 31)
        {
            sort = 50;
        }

        if (dept == 2 && pclass == 1)
        {
            sort = 60;
        }

        if (dept == 2 && pclass == 2)
        {
            sort = 70;
        }

        if (dept == 4 && pclass == 1)
        {
            sort = 80;
        }

        if (dept == 3 && pclass == 01)
        {
            sort = 90;
        }

        if (dept == 5 && pclass == 01)
        {
            sort = 100;
        }

        if (dept == 7 && pclass == 01)
        {
            sort = 110;
        }

        if (dept == 6 && pclass == 01)
        {
            sort = 120;
        }

        return sort;
    }


    void PrintEmployee(MemberYearSummary member)
    {
        // BADGE #     EMPLOYEE NAME              BEGINNING     EARNINGS         CONT         FORF        DIST.       ENDING      V E S T E D E
        //                                          BALANCE                                                          BALANCE       AMOUNT  %  C
        // 700453 BANKS, JASON               11,778.95       590.00     3,330.00       888.00                 16,586.95     9,452.17  60
        // 700441 BLACKBURN, LEVI            17,111.19       855.00     3,075.00       820.00                 21,861.19    21,861.19 100

        string formattedLine =
            $"     {member.BadgeNumber,-5} {member.FullName,-25} " +
            $"{(member.BeginningBalance != 0 ? member.BeginningBalance.ToString("N2") : ""),12} " +
            $"{(member.Earnings != 0 ? member.Earnings.ToString("N2") : ""),12} " +
            $"{(member.Contributions != 0 ? member.Contributions.ToString("N2") : ""),12} " +
            $"{(member.Forfeiture != 0 ? member.Forfeiture.ToString("N2") : ""),12} " +
            $"{(member.Distributions != 0 ? member.Distributions.ToString("N2") : ""),12} " +
            $"{(member.EndingBalance != 0 ? member.EndingBalance.ToString("N2") : ""),12} " +
            $"{(member.VestedAmount != 0 ? member.VestedAmount.ToString("N2") : ""),12} " +
            $"{(member.VestedPercentage != 0 ? member.VestedPercentage.ToString("N0") : ""),3}";

        _testOutputHelper.WriteLine(formattedLine);
    }

#pragma warning disable xUnit1013
    public void ProcessStoreType(int storeType, int? sSort = null)

    {
        string wType;

        if (StoreTypes.RetailStores.Contains(storeType) ||
            StoreTypes.WarehouseAll.Contains(storeType) ||
            storeType == StoreTypes.Drivers ||
            StoreTypes.Headquarters.Contains(storeType) ||
            storeType == StoreTypes.LeeDrug)
        {
            wType = "STORE MANAGEMENT";
            GenerateDetail(wType);
        }
        else if (storeType == StoreTypes.PsPensionRetired)
        {
            wType = "RETIRED - DRAWING PENSION";
            GenerateDetail(wType);
        }
        else if (storeType == StoreTypes.PsPensionActive)
        {
            wType = "ACTIVE - DRAWING PENSION";
            GenerateDetail(wType);
        }
        else if (storeType == StoreTypes.PsTerminatedEmployees && sSort.HasValue)
        {
            wType = sSort switch
            {
                1 => "INACTIVE - WORKMAN’S COMPENSATION",
                2 => "INACTIVE - TRANSFERRED",
                3 => "INACTIVE - FMLA-APPROVED",
                4 => "INACTIVE - PERSONAL OR FAMILY REASON",
                5 => "INACTIVE - HEALTH REASONS - NON FMLA",
                6 => "INACTIVE - MILITARY",
                7 => "INACTIVE - SCHOOL OR SPORTS",
                8 => "INACTIVE - OFF FOR SUMMER",
                9 => "INACTIVE - INJURED",
                10 => "TERMINATED WEEKLYS AND TERMINATED MONTHLYS",
                11 => "OTHER",
                _ => ""
            };

            GenerateDetail(wType);
        }
        else if (storeType == StoreTypes.PsTerminatedEmployeesZero)
        {
            wType = "INACT/TERM WEEKLYS AND TERM MONTHLYS - NO BALANCE";
            GenerateDetail(wType);
        }
        else if (storeType == StoreTypes.PsMonthlyEmployees)
        {
            wType = "ACTIVE/INACTIVE MONTHLYS";
            GenerateDetail(wType);
        }
    }

#pragma warning disable S2325
    private void GenerateDetail(string wType)
    {
        // Implementation of GENERATE DETAIL-W-TYPE equivalent
        Console.WriteLine(wType);
    }
}
