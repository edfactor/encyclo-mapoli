using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.MasterInquiry;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for EmployeeMasterInquiryService using factory pattern
/// Tests employee-specific query logic extracted from MasterInquiryService
/// </summary>
[Description("PS-1720: Unit tests for EmployeeMasterInquiryService refactoring with factory pattern")]
public sealed class EmployeeMasterInquiryServiceTests
{
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly Mock<ILogger<EmployeeMasterInquiryService>> _mockLogger;
    private readonly Mock<IMissiveService> _mockMissiveService;
    private readonly Mock<IDemographicReaderService> _mockDemographicReader;
    private readonly Mock<IProfitSharingDataContextFactory> _mockFactory;
    private readonly Mock<ProfitSharingReadOnlyDbContext> _mockContext;

    public EmployeeMasterInquiryServiceTests()
    {
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLogger = new Mock<ILogger<EmployeeMasterInquiryService>>();
        _mockLoggerFactory.Setup(f => f.CreateLogger<EmployeeMasterInquiryService>())
            .Returns(_mockLogger.Object);

        _mockMissiveService = new Mock<IMissiveService>();
        _mockDemographicReader = new Mock<IDemographicReaderService>();

        // Setup factory and mock context
        _mockFactory = new Mock<IProfitSharingDataContextFactory>();
        _mockContext = new Mock<ProfitSharingReadOnlyDbContext>();
    }

    private EmployeeMasterInquiryService CreateService()
    {
        return new EmployeeMasterInquiryService(
            _mockLoggerFactory.Object,
            _mockFactory.Object,
            _mockMissiveService.Object,
            _mockDemographicReader.Object);
    }

    [Fact]
    [Description("PS-1720: FindEmployeeSsnByBadgeAsync should return SSN when badge exists")]
    public async Task FindEmployeeSsnByBadgeAsync_WhenBadgeExists_ReturnsSsn()
    {
        // Arrange
        const int badgeNumber = 12345;
        const int expectedSsn = 111111111;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dob = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));

        var demographicsList = new List<Demographic>
        {
            new()
            {
                Id = 1, Ssn = expectedSsn, BadgeNumber = badgeNumber, PayFrequencyId = 1, EmploymentStatusId = 'A',
                OracleHcmId = 1000, StoreNumber = 1, PayClassificationId = "1", HireDate = today,
                DateOfBirth = dob,
                ContactInfo = new ContactInfo { FirstName = "John", LastName = "Doe", FullName = "Doe, John" },
                Address = new Address { Street = "123 Main St", City = "Boston", State = "MA", PostalCode = "02101" }
            }
        };
        var demographics = demographicsList.AsQueryable();

        // Setup factory to execute the lambda with our mock context
        _mockFactory.Setup(f => f.UseReadOnlyContext(
            It.IsAny<Func<ProfitSharingReadOnlyDbContext, Task<int>>>(),
            It.IsAny<CancellationToken>()))
            .Returns<Func<ProfitSharingReadOnlyDbContext, Task<int>>, CancellationToken>(
                async (func, ct) =>
                {
                    _mockDemographicReader.Setup(r => r.BuildDemographicQuery(_mockContext.Object))
                        .ReturnsAsync(demographics);
                    return await func(_mockContext.Object);
                });

        var service = CreateService();

        // Act
        var ssn = await service.FindEmployeeSsnByBadgeAsync(badgeNumber);

        // Assert
        ssn.ShouldBe(expectedSsn);
        _mockFactory.Verify(f => f.UseReadOnlyContext(
            It.IsAny<Func<ProfitSharingReadOnlyDbContext, Task<int>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Description("PS-1720: FindEmployeeSsnByBadgeAsync should return 0 when badge not found")]
    public async Task FindEmployeeSsnByBadgeAsync_WhenBadgeNotFound_ReturnsZero()
    {
        // Arrange
        const int badgeNumber = 99999;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dob = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));

        var demographicsList = new List<Demographic>
        {
            new()
            {
                Id = 1, Ssn = 111111111, BadgeNumber = 12345, PayFrequencyId = 1, EmploymentStatusId = 'A',
                OracleHcmId = 1000, StoreNumber = 1, PayClassificationId = "1", HireDate = today,
                DateOfBirth = dob,
                ContactInfo = new ContactInfo { FirstName = "John", LastName = "Doe", FullName = "Doe, John" },
                Address = new Address { Street = "123 Main St", City = "Boston", State = "MA", PostalCode = "02101" }
            }
        };
        var demographics = demographicsList.AsQueryable();

        _mockFactory.Setup(f => f.UseReadOnlyContext(
            It.IsAny<Func<ProfitSharingReadOnlyDbContext, Task<int>>>(),
            It.IsAny<CancellationToken>()))
            .Returns<Func<ProfitSharingReadOnlyDbContext, Task<int>>, CancellationToken>(
                async (func, ct) =>
                {
                    _mockDemographicReader.Setup(r => r.BuildDemographicQuery(_mockContext.Object))
                        .ReturnsAsync(demographics);
                    return await func(_mockContext.Object);
                });

        var service = CreateService();

        // Act
        var ssn = await service.FindEmployeeSsnByBadgeAsync(badgeNumber);

        // Assert
        ssn.ShouldBe(0);
    }

    [Fact]
    [Description("PS-1720: GetEmployeeDetailsAsync should return employee details when employee exists")]
    public async Task GetEmployeeDetailsAsync_WhenEmployeeExists_ReturnsDetails()
    {
        // Arrange
        const int employeeId = 1;
        const short currentYear = 2025;
        const short previousYear = 2024;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dob = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));

        var demographicsList = new List<Demographic>
        {
            new()
            {
                Id = employeeId, Ssn = 111111111, BadgeNumber = 12345, PayFrequencyId = 1, EmploymentStatusId = 'A',
                OracleHcmId = 1000, StoreNumber = 1, PayClassificationId = "1", HireDate = today,
                DateOfBirth = dob, FullTimeDate = today,
                ContactInfo = new ContactInfo { FirstName = "John", LastName = "Doe", FullName = "Doe, John", PhoneNumber = "555-1234" },
                Address = new Address { Street = "123 Main St", City = "Boston", State = "MA", PostalCode = "02101" },
                PayProfits = new List<PayProfit>
                {
                    new() { DemographicId = employeeId, ProfitYear = currentYear, CurrentHoursYear = 2000, Etva = 50000, EnrollmentId = 1 },
                    new() { DemographicId = employeeId, ProfitYear = previousYear, CurrentHoursYear = 1950, Etva = 48000, PsCertificateIssuedDate = today }
                },
                EmploymentStatus = new EmploymentStatus { Id = 'A', Name = "Active" }
            }
        };
        var demographics = demographicsList.AsQueryable();

        _mockFactory.Setup(f => f.UseReadOnlyContext(
            It.IsAny<Func<ProfitSharingReadOnlyDbContext, Task<(int, MemberDetails?)>>>(),
            It.IsAny<CancellationToken>()))
            .Returns<Func<ProfitSharingReadOnlyDbContext, Task<(int, MemberDetails?)>>, CancellationToken>(
                async (func, ct) =>
                {
                    _mockDemographicReader.Setup(r => r.BuildDemographicQuery(_mockContext.Object))
                        .ReturnsAsync(demographics);
                    _mockMissiveService.Setup(m => m.DetermineMissivesForSsns(It.IsAny<List<int>>(), currentYear, ct))
                        .ReturnsAsync(new Dictionary<int, List<int>>());
                    return await func(_mockContext.Object);
                });

        var service = CreateService();

        // Act
        var result = await service.GetEmployeeDetailsAsync(employeeId, currentYear, previousYear);

        // Assert
        result.ssn.ShouldBe(111111111);
        result.memberDetails.ShouldNotBeNull();
        result.memberDetails.Id.ShouldBe(employeeId);
        result.memberDetails.FirstName.ShouldBe("John");
        result.memberDetails.LastName.ShouldBe("Doe");
        result.memberDetails.IsEmployee.ShouldBeTrue();
        result.memberDetails.YearToDateProfitSharingHours.ShouldBe(2000);
        result.memberDetails.ReceivedContributionsLastYear.ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1720: GetEmployeeDetailsAsync should return empty details when employee not found")]
    public async Task GetEmployeeDetailsAsync_WhenEmployeeNotFound_ReturnsZeroSsnAndEmptyDetails()
    {
        // Arrange
        const int employeeId = 999;
        const short currentYear = 2025;
        const short previousYear = 2024;

        var demographicsList = new List<Demographic>();
        var demographics = demographicsList.AsQueryable();

        _mockFactory.Setup(f => f.UseReadOnlyContext(
            It.IsAny<Func<ProfitSharingReadOnlyDbContext, Task<(int, MemberDetails?)>>>(),
            It.IsAny<CancellationToken>()))
            .Returns<Func<ProfitSharingReadOnlyDbContext, Task<(int, MemberDetails?)>>, CancellationToken>(
                async (func, ct) =>
                {
                    _mockDemographicReader.Setup(r => r.BuildDemographicQuery(_mockContext.Object))
                        .ReturnsAsync(demographics);
                    return await func(_mockContext.Object);
                });

        var service = CreateService();

        // Act
        var result = await service.GetEmployeeDetailsAsync(employeeId, currentYear, previousYear);

        // Assert
        result.ssn.ShouldBe(0);
        result.memberDetails.ShouldNotBeNull();
        result.memberDetails.Id.ShouldBe(0);
    }
}
