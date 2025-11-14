using System.ComponentModel;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.Reports;

[Collection("SharedGlobalState")]
public sealed class AdhocTerminatedEmployeesServiceTests : ApiTestBase<Program>
{
    private IAdhocTerminatedEmployeesService Service => ServiceProvider?.GetRequiredService<IAdhocTerminatedEmployeesService>()!;

    public AdhocTerminatedEmployeesServiceTests()
    {
    }

    [Fact]
    [Description("PS-1894 : Terminated employees report should honor date range filtering")]
    public async Task GetTerminatedEmployees_ShouldFilterByTerminationDateRange()
    {
        // Arrange - Create employees with different termination dates
        var profitYear = (short)2025;
        var earlyTerminationDate = new DateOnly(2024, 3, 6);  // Outside range (too early)
        var withinRangeDate1 = new DateOnly(2025, 1, 15);     // Inside range
        var withinRangeDate2 = new DateOnly(2025, 6, 30);     // Inside range
        var lateTerminationDate = new DateOnly(2026, 1, 10);  // Outside range (too late)

        var earlyEmployee = StockFactory.CreateEmployee(profitYear);
        earlyEmployee.demographic.BadgeNumber = 1001;
        earlyEmployee.demographic.TerminationDate = earlyTerminationDate;
        earlyEmployee.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        earlyEmployee.demographic.TerminationCodeId = TerminationCode.Constants.LeftOnOwn;
        earlyEmployee.demographic.TerminationCode = new TerminationCode { Id = TerminationCode.Constants.LeftOnOwn, Name = "Left On Own" };

        var rangeEmployee1 = StockFactory.CreateEmployee(profitYear);
        rangeEmployee1.demographic.BadgeNumber = 1002;
        rangeEmployee1.demographic.TerminationDate = withinRangeDate1;
        rangeEmployee1.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        rangeEmployee1.demographic.TerminationCodeId = TerminationCode.Constants.Stealing;
        rangeEmployee1.demographic.TerminationCode = new TerminationCode { Id = TerminationCode.Constants.Stealing, Name = "Stealing" };

        var rangeEmployee2 = StockFactory.CreateEmployee(profitYear);
        rangeEmployee2.demographic.BadgeNumber = 1003;
        rangeEmployee2.demographic.TerminationDate = withinRangeDate2;
        rangeEmployee2.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        rangeEmployee2.demographic.TerminationCodeId = TerminationCode.Constants.AnotherJob;
        rangeEmployee2.demographic.TerminationCode = new TerminationCode { Id = TerminationCode.Constants.AnotherJob, Name = "Another Job" };

        var lateEmployee = StockFactory.CreateEmployee(profitYear);
        lateEmployee.demographic.BadgeNumber = 1004;
        lateEmployee.demographic.TerminationDate = lateTerminationDate;
        lateEmployee.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        lateEmployee.demographic.TerminationCodeId = TerminationCode.Constants.JobAbandonment;
        lateEmployee.demographic.TerminationCode = new TerminationCode { Id = TerminationCode.Constants.JobAbandonment, Name = "Job Abandonment" };

        MockDbContextFactory = new ScenarioFactory
        {
            Demographics = new List<Demographic>
            {
                earlyEmployee.demographic,
                rangeEmployee1.demographic,
                rangeEmployee2.demographic,
                lateEmployee.demographic
            }
        }.BuildMocks();

        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2025, 1, 4),
            EndingDate = new DateOnly(2025, 12, 27),
            Skip = 0,
            Take = 100,
            SortBy = "FullName",
            IsSortDescending = false
        };

        // Act
        var result = await Service.GetTerminatedEmployees(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Response.ShouldNotBeNull();
        result.Response.Results.ShouldNotBeNull();

        // Verify only employees within the date range are returned
        result.Response.Results.Count().ShouldBe(2);
        result.Response.Total.ShouldBe(2);

        // Verify termination dates are within the requested range
        var allResultsWithinRange = result.Response.Results.All(e =>
            e.TerminationDate >= request.BeginningDate &&
            e.TerminationDate <= request.EndingDate);

        allResultsWithinRange.ShouldBeTrue();

        // Verify early and late terminated employees are excluded
        var resultBadges = result.Response.Results.Select(e => e.BadgeNumber).ToList();
        resultBadges.ShouldContain(rangeEmployee1.demographic.BadgeNumber);
        resultBadges.ShouldContain(rangeEmployee2.demographic.BadgeNumber);
        resultBadges.ShouldNotContain(earlyEmployee.demographic.BadgeNumber);
        resultBadges.ShouldNotContain(lateEmployee.demographic.BadgeNumber);
    }

    [Fact]
    [Description("PS-1894 : Terminated employees report should return correct response metadata")]
    public async Task GetTerminatedEmployees_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var profitYear = (short)2025;
        var terminationDate = new DateOnly(2025, 6, 15);
        var employee = StockFactory.CreateEmployee(profitYear);
        employee.demographic.TerminationDate = terminationDate;
        employee.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        employee.demographic.TerminationCodeId = 'T';
        employee.demographic.TerminationCode = new TerminationCode { Id = 'T', Name = "Terminated" };

        MockDbContextFactory = new ScenarioFactory
        {
            Demographics = new List<Demographic> { employee.demographic }
        }.BuildMocks();

        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2025, 1, 1),
            EndingDate = new DateOnly(2025, 12, 31),
            Skip = 0,
            Take = 100,
            SortBy = "FullName",
            IsSortDescending = false
        };

        // Act
        var result = await Service.GetTerminatedEmployees(request, CancellationToken.None);

        // Assert
        result.ReportName.ShouldBe("Adhoc Terminated Employee Report");
        result.ReportDate.Year.ShouldBe(DateTime.Now.Year);
        result.StartDate.ShouldBe(request.BeginningDate);
        result.EndDate.ShouldBe(request.EndingDate);
    }

    [Fact]
    [Description("PS-1894 : Terminated employees report should exclude retired employees")]
    public async Task GetTerminatedEmployees_ShouldExcludeRetiredEmployees()
    {
        // Arrange
        var profitYear = (short)2025;
        var terminationDate = new DateOnly(2025, 6, 15);

        var terminatedEmployee = StockFactory.CreateEmployee(profitYear);
        terminatedEmployee.demographic.BadgeNumber = 2001;
        terminatedEmployee.demographic.TerminationDate = terminationDate;
        terminatedEmployee.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        terminatedEmployee.demographic.TerminationCodeId = 'T';
        terminatedEmployee.demographic.TerminationCode = new TerminationCode { Id = 'T', Name = "Terminated" };

        var retiredEmployee = StockFactory.CreateEmployee(profitYear);
        retiredEmployee.demographic.BadgeNumber = 2002;
        retiredEmployee.demographic.TerminationDate = terminationDate;
        retiredEmployee.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        retiredEmployee.demographic.TerminationCodeId = TerminationCode.Constants.Retired;
        retiredEmployee.demographic.TerminationCode = new TerminationCode { Id = TerminationCode.Constants.Retired, Name = "Retired" };

        MockDbContextFactory = new ScenarioFactory
        {
            Demographics = new List<Demographic>
            {
                terminatedEmployee.demographic,
                retiredEmployee.demographic
            }
        }.BuildMocks();

        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2025, 1, 1),
            EndingDate = new DateOnly(2025, 12, 31),
            Skip = 0,
            Take = 100,
            SortBy = "FullName",
            IsSortDescending = false
        };

        // Act
        var result = await Service.GetTerminatedEmployees(request, CancellationToken.None);

        // Assert
        result.Response.Results.Count().ShouldBe(1);
        result.Response.Results.First().BadgeNumber.ShouldBe(terminatedEmployee.demographic.BadgeNumber);
    }

    [Fact]
    [Description("PS-1894 : Terminated employees report should mask SSN")]
    public async Task GetTerminatedEmployees_ShouldMaskSsn()
    {
        // Arrange
        var terminationDate = new DateOnly(2025, 6, 15);
        var employee = StockFactory.CreateEmployee((short)terminationDate.Year);
        employee.demographic.TerminationDate = terminationDate;
        employee.demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        employee.demographic.TerminationCodeId = 'T';
        employee.demographic.TerminationCode = new TerminationCode { Id = 'T', Name = "Terminated" };

        MockDbContextFactory = new ScenarioFactory
        {
            Demographics = new List<Demographic> { employee.demographic }
        }.BuildMocks();

        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2025, 1, 1),
            EndingDate = new DateOnly(2025, 12, 31),
            Skip = 0,
            Take = 100,
            SortBy = "FullName",
            IsSortDescending = false
        };

        // Act
        var result = await Service.GetTerminatedEmployees(request, CancellationToken.None);

        // Assert
        result.Response.Results.Count().ShouldBe(1);
        var employeeSsn = result.Response.Results.First().Ssn;

        // SSN should be masked in the format XXX-XX-####
        employeeSsn.ShouldNotBeNullOrEmpty();
        employeeSsn.ShouldStartWith("XXX-XX-");
    }
}
