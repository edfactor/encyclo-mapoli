using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Eligibility;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

internal sealed record TestEmployee
{
    public int Id { get; init; }
    public int OracleHcmId { get; init; }
    public int BadgeNumber { get; init; }
    public required string FullName { get; init; }
    public int Age { get; init; }
    public int HoursWorked { get; init; }
    public char EmploymentStatusId { get; init; }
}

public class GetEligibleEmployeesTests : ApiTestBase<Program>
{
    private const short TestProfitYear = 2023; // We use this year, so the mock calendar service will be happy
    private readonly ProfitYearRequest _requestDto = new ProfitYearRequest() { ProfitYear = TestProfitYear };

    [Fact]
    public async Task Unauthorized()
    {
        // Act
        var response =
            await ApiClient
                .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public Task happy_path_json()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            TestEmployee te = StockEmployee();
            await save(te, c);
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            var response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.ReportName.Should().Be($"Get Eligible Employees for Year {TestProfitYear}");
            var dto = response.Result.Response.Results.First(e => e.BadgeNumber == te.BadgeNumber);
            dto.Should().BeEquivalentTo(new GetEligibleEmployeesResponseDto { OracleHcmId = te.OracleHcmId, BadgeNumber = te.BadgeNumber, FullName = te.FullName }
            );

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task happy_path_csv()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            TestEmployee te = StockEmployee();
            await save(te, c);
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            var yearEndDate = new DateOnly(2023, 12, 31);
            var birthDateOfExactly21YearsOld = yearEndDate.AddYears(-21);

            int numberReadOnFrozen = await c.PayProfits.Where(p => p.ProfitYear == TestProfitYear).CountAsync(CancellationToken.None);

            int numberNotSelected = await c.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == TestProfitYear)
                .Where(p => p.Demographic!.DateOfBirth > birthDateOfExactly21YearsOld /*too young*/ || p.CurrentHoursYear < 1000 ||
                            p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
                .CountAsync(CancellationToken.None);

            int numberWritten = await c.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == TestProfitYear)
                .Where(p => p.Demographic!.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/ && p.CurrentHoursYear >= 1000 &&
                            p.Demographic!.EmploymentStatusId != EmploymentStatus.Constants.Terminated).CountAsync(CancellationToken.None);

            // Act
            var response =
                await DownloadClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Response.Content.Should().NotBeNull();

            // Verify CSV file
            string csvData = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
            string[] lines = csvData.Split(["\r\n", "\n"], StringSplitOptions.None);
            // line 0 is today's date
            lines[0].Should().NotBeEmpty();
            lines[1].Should().Be($"Get Eligible Employees for Year {TestProfitYear}");
            lines[2].Should().BeEmpty(); // blank link
            lines[3].Should().Be($"Number read on FROZEN,{numberReadOnFrozen}");
            lines[4].Should().Be($"Number not selected,{numberNotSelected}");
            lines[5].Should().Be($"Number written,{numberWritten}");

            lines[6].Should().Be("ORACLE_HCM_ID,BADGE_PSN,NAME");

            lines.Skip(7).Should().Contain($"{te.OracleHcmId},{te.BadgeNumber},\"{te.FullName}\"");

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task no_employees_too_young()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            TestEmployee te = StockEmployee() with { Age = 20 };
            await save(te, c);
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            var response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.Response.Results
                .Should()
                .NotContain(e => e.BadgeNumber == te.BadgeNumber);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task not_enough_hours()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            TestEmployee te = StockEmployee() with { HoursWorked = 999 };
            await save(te, c);
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            var response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.Response.Results
                .Should()
                .NotContain(e => e.BadgeNumber == te.BadgeNumber);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task employee_terminated()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            TestEmployee te = StockEmployee() with { EmploymentStatusId = EmploymentStatus.Constants.Terminated };
            await save(te, c);
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            var response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.Response.Results
                .Should()
                .NotContain(e => e.BadgeNumber == te.BadgeNumber);

            return Task.CompletedTask;
        });
    }

    private TestEmployee StockEmployee() => new TestEmployee
    {
        Id = 17,
        OracleHcmId = 7,
        BadgeNumber = 77,
        FullName = "Smith, Joe K",
        Age = 44,
        HoursWorked = 1010,
        EmploymentStatusId = EmploymentStatus.Constants.Active
    };


    private static async Task save(TestEmployee testEmployee, ProfitSharingDbContext ctx)
    {
        var pp = await ctx.PayProfits.Include(payProfit => payProfit.Demographic!)
            .ThenInclude(demographic => demographic.ContactInfo).Include(p => p.Demographic != null).FirstAsync(CancellationToken.None);
        pp.ProfitYear = TestProfitYear;
        pp.DemographicId = testEmployee.Id;
        pp.CurrentHoursYear = testEmployee.HoursWorked;
        var demo = pp.Demographic!;
        demo.OracleHcmId = testEmployee.OracleHcmId;
        demo.Id = testEmployee.Id;
        demo.ContactInfo.FullName = testEmployee.FullName;
        demo.BadgeNumber = testEmployee.BadgeNumber;
        demo.DateOfBirth = convertAgeToBirthDate(TestProfitYear, testEmployee.Age);
        demo.EmploymentStatusId = testEmployee.EmploymentStatusId;

        var df = new DemographicFaker();
        // The fake PayProfits entities share fake Demographic entities. (see demographicQueue in PayProfitFaker)
        // PayProfit and Demographic are 1-1 in the database, to prevent errors - we assign the PayProfits sharing this
        // Demographic new Demographics.
        var otherPayProfitsUsingOurDemograhic = await ctx.PayProfits.Where(ppo => ppo != pp && ppo.Demographic == demo).ToListAsync(CancellationToken.None);
        otherPayProfitsUsingOurDemograhic.ForEach(pp => pp.Demographic = df.Generate());

        await ctx.SaveChangesAsync(CancellationToken.None);
    }

    private static DateOnly convertAgeToBirthDate(short profitSharYear, int age)
    {
        return new DateOnly(profitSharYear - age, 6, 1); // drop them in June to avoid any fiscal year end weirdness.
    }
}
