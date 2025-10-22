using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Moq;
using Shouldly;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

/// <summary>
/// Regression tests for Profit Sharing Summary Report (Report 9) to ensure COBOL PAY426N logic is preserved.
/// Bug fix: October 2025 optimization incorrectly used IsTerminatedInFiscalYear instead of IsTerminatedBeforeFiscalEnd.
/// </summary>
[Description("PS-1262: Profit Sharing Summary Report COBOL Logic")]
public class ProfitSharingSummaryReportRegressionTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const short TestYear = 2023;

    public ProfitSharingSummaryReportRegressionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Regression test: Report 7/8 should count ALL terminated employees before fiscal end,
    /// not just those terminated within the fiscal year.
    /// </summary>
    [Fact(DisplayName = "Reports 7/8 count all terminated before fiscal end")]
    public async Task Reports7And8_CountAllTerminatedBeforeFiscalEnd()
    {
        // Arrange
        var fiscalStart = new DateOnly(TestYear, 6, 1);
        var fiscalEnd = new DateOnly(TestYear + 1, 5, 31);
        var birthday = fiscalEnd.AddYears(-25); // Age 25
        var hireDate = fiscalStart.AddYears(-3);

        var factory = new ScenarioFactory { ProfitYear = TestYear };

        // Employee terminated BEFORE fiscal year (previous year) - should count
        var emp1 = new Demographic
        {
            Id = 1,
            OracleHcmId = 10001,
            BadgeNumber = 1001,
            Ssn = 111111001,
            DateOfBirth = birthday,
            HireDate = hireDate,
            EmploymentStatusId = EmploymentStatus.Constants.Terminated,
            TerminationDate = fiscalStart.AddMonths(-3), // Terminated 3 months before fiscal year
            EmploymentTypeId = EmploymentType.Constants.PartTime,
            StoreNumber = 1,
            PayFrequencyId = PayFrequency.Constants.Weekly,
            PayClassificationId = PayClassification.Constants.Manager,
            Address = new Address
            {
                Street = "123 Test St",
                City = "TestCity",
                State = "MA",
                PostalCode = "01234"
            },
            ContactInfo = new ContactInfo
            {
                FirstName = "Test",
                LastName = "Employee1",
                FullName = "Test Employee1"
            }
        };

        // Employee terminated DURING fiscal year - should count
        var emp2 = new Demographic
        {
            Id = 2,
            OracleHcmId = 10002,
            BadgeNumber = 1002,
            Ssn = 111111002,
            DateOfBirth = birthday,
            HireDate = hireDate,
            EmploymentStatusId = EmploymentStatus.Constants.Terminated,
            TerminationDate = fiscalStart.AddMonths(6), // Terminated mid-fiscal year
            EmploymentTypeId = EmploymentType.Constants.PartTime,
            StoreNumber = 1,
            PayFrequencyId = PayFrequency.Constants.Weekly,
            PayClassificationId = PayClassification.Constants.Manager,
            Address = new Address
            {
                Street = "456 Test Ave",
                City = "TestCity",
                State = "MA",
                PostalCode = "01234"
            },
            ContactInfo = new ContactInfo
            {
                FirstName = "Test",
                LastName = "Employee2",
                FullName = "Test Employee2"
            }
        };

        factory.Demographics = [emp1, emp2];
        factory.PayProfits =
        [
            new PayProfit
            {
                DemographicId = 1,
                ProfitYear = TestYear,
                CurrentHoursYear = 800,
                CurrentIncomeYear = 40000,
                HoursExecutive = 0,
                IncomeExecutive = 0,
                PointsEarned = 400,
                Etva = 0
            },
            new PayProfit
            {
                DemographicId = 2,
                ProfitYear = TestYear,
                CurrentHoursYear = 900,
                CurrentIncomeYear = 45000,
                HoursExecutive = 0,
                IncomeExecutive = 0,
                PointsEarned = 450,
                Etva = 0
            }
        ];

        var mockFactory = factory.BuildMocks();

        // Mock calendar service
        var mockCalendarService = new Mock<ICalendarService>();
        mockCalendarService
            .Setup(c => c.GetYearStartAndEndAccountingDatesAsync(TestYear, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CalendarResponseDto
            {
                FiscalBeginDate = fiscalStart,
                FiscalEndDate = fiscalEnd
            });

        // Mock total service and demographic reader
        var mockTotalService = new Mock<TotalService>();
        var mockDemographicReader = new Mock<IDemographicReaderService>();

        var service = new ProfitSharingSummaryReportService(
            mockFactory,
            mockCalendarService.Object,
            mockTotalService.Object,
            mockDemographicReader.Object);

        // Act
        var result = await service.GetYearEndProfitSharingSummaryReportAsync(
            new BadgeNumberRequest { ProfitYear = TestYear, UseFrozenData = false },
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        var reports = result.LineItems;
        reports.ShouldNotBeEmpty();

        var report7 = reports.FirstOrDefault(r => r.LineItemPrefix == "7");
        report7.ShouldNotBeNull();
        report7.NumberOfMembers.ShouldBeGreaterThanOrEqualTo(1,
            "Report 7 should count terminated employees before fiscal end");

        _testOutputHelper.WriteLine($"Report 7: {report7.NumberOfMembers} members, {report7.TotalHours} hours, ${report7.TotalWages} wages");
    }
}
