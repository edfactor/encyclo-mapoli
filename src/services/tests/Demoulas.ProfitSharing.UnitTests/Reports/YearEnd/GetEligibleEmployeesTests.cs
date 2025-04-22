using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Eligibility;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class GetEligibleEmployeesTests : ApiTestBase<Program>
{
    // This test uses a single employee.   These references let us adjust the employee in each test.
    private readonly Demographic _d;
    private readonly DemographicHistory _dh;
    private readonly PayProfit _pp;
    
    private readonly short _testProfitYear;
    private readonly ProfitYearRequest _requestDto;
    private readonly ScenarioFactory _scenarioFactory;

    public GetEligibleEmployeesTests()
    {
        _scenarioFactory = new ScenarioFactory().EmployeeWithHistory(); // Sets up a single employee with demographic history
        MockDbContextFactory = _scenarioFactory.BuildMocks();
        _testProfitYear = _scenarioFactory.ProfitYear;
        _requestDto = new ProfitYearRequest { ProfitYear = _testProfitYear };
        _d = _scenarioFactory.Demographics[0];
        _pp = _scenarioFactory.PayProfits[0];
        _dh = _scenarioFactory.DemographicHistories[0];
    }
    
    [Fact]
    public async Task Unauthorized()
    {
        // Act
        TestResult<GetEligibleEmployeesResponse> response =
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
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            TestResult<GetEligibleEmployeesResponse> response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.ReportName.Should().Be($"Get Eligible Employees for Year {_testProfitYear}");
            EligibleEmployee dto = response.Result.Response.Results.First(e => e.BadgeNumber == _dh.BadgeNumber);
            dto.Should().BeEquivalentTo(new EligibleEmployee
                {
                    OracleHcmId = _d.OracleHcmId,
                    BadgeNumber = _dh.BadgeNumber,
                    FullName = _d.ContactInfo!.FullName!,
                    DepartmentId = _dh.DepartmentId,
                    Department = "Dairy"
                }
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
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            DateOnly yearEndDate = new(_testProfitYear, 12, 31);
            DateOnly birthDateOfExactly21YearsOld = yearEndDate.AddYears(-21);

            int expectedNumberReadOnFrozen = await c.PayProfits.Where(p => p.ProfitYear == _testProfitYear).CountAsync(CancellationToken.None);

            int expectedNumberNotSelected = await c.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == _testProfitYear)
                .Where(p => p.Demographic!.DateOfBirth > birthDateOfExactly21YearsOld /*too young*/
                            || p.CurrentHoursYear  < 1000 ||
                            p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
                .CountAsync(CancellationToken.None);

            int expectedNumberWritten = await c.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == _testProfitYear)
                .Where(p => p.Demographic!.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/ && p.CurrentHoursYear >= 1000 &&
                            p.Demographic!.EmploymentStatusId != EmploymentStatus.Constants.Terminated).CountAsync(CancellationToken.None);

            // Act
            TestResult<GetEligibleEmployeesResponse> response =
                await DownloadClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Response.Content.Should().NotBeNull();

            // Verify CSV file
            string csvData = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
            string[] lines = csvData.Split(["\r\n", "\n"], StringSplitOptions.None);
            // line 0 is today's date
            lines[0].Should().NotBeEmpty();
            lines[1].Should().Be($"Get Eligible Employees for Year {_testProfitYear}");
            lines[2].Should().BeEmpty(); // blank link
            lines[3].Should().Be($"Number read on FROZEN,{expectedNumberReadOnFrozen}");
            lines[4].Should().Be($"Number not selected,{expectedNumberNotSelected}");
            lines[5].Should().Be($"Number written,{expectedNumberWritten}");

            lines[6].Should().Be("ASSIGNMENT_ID,BADGE_PSN,NAME");

            lines.Skip(7).Should().Contain($"{_dh.DepartmentId},{_dh.BadgeNumber},\"{_d.ContactInfo!.FullName!}\"");

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task no_employees_too_young()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            // Too young to be eligible
            _dh.DateOfBirth = new DateOnly(_testProfitYear, 6, 1).AddYears(-15);

            // Act
            TestResult<GetEligibleEmployeesResponse> response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.Response.Results
                .Should()
                .NotContain(e => e.BadgeNumber == _dh.BadgeNumber);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task not_enough_hours()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            // Not enough hours worked
            _pp.CurrentHoursYear = 999;
            _pp.HoursExecutive = 0;

            // Act
            TestResult<GetEligibleEmployeesResponse> response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.Response.Results
                .Should()
                .NotContain(e => e.BadgeNumber == _dh.BadgeNumber);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task employee_terminated()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            _dh.EmploymentStatusId = EmploymentStatus.Constants.Terminated;

            // Act
            TestResult<GetEligibleEmployeesResponse> response =
                await ApiClient
                    .GETAsync<GetEligibleEmployeesEndpoint, ProfitYearRequest, GetEligibleEmployeesResponse>(_requestDto);

            // Assert
            response.Result.Response.Results
                .Should()
                .NotContain(e => e.BadgeNumber == _dh.BadgeNumber);

            return Task.CompletedTask;
        });
    }
}
