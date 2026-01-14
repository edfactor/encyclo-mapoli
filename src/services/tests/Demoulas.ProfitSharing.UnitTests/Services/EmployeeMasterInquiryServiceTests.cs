using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.MasterInquiry;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
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
    private readonly ILoggerFactory _loggerFactory;
    private readonly Mock<IMissiveService> _mockMissiveService;
    private readonly Mock<IDemographicReaderService> _mockDemographicReader;
    private readonly ScenarioDataContextFactory _factory;

    public EmployeeMasterInquiryServiceTests()
    {
        // Use real LoggerFactory instead of mocking (extension methods can't be mocked)
        // Set minimum level to Warning to avoid noisy info logs during tests
        _loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning));

        _mockMissiveService = new Mock<IMissiveService>();
        _mockDemographicReader = new Mock<IDemographicReaderService>();

        // Use ScenarioDataContextFactory which provides mock contexts with MockQueryable support
        _factory = new ScenarioDataContextFactory();
    }

    private EmployeeMasterInquiryService CreateService()
    {
        return new EmployeeMasterInquiryService(
            _loggerFactory,
            _factory,
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

        // Setup demographic reader to return MockDbSet.Object for async support
        var mockDemographicsDbSet = demographicsList.BuildMockDbSet();
        _mockDemographicReader.Setup(r => r.BuildDemographicQuery(
            It.IsAny<IProfitSharingDbContext>(), false))
            .ReturnsAsync(mockDemographicsDbSet.Object);

        var service = CreateService();

        // Act
        var ssn = await service.FindEmployeeSsnByBadgeAsync(badgeNumber);

        // Assert
        ssn.ShouldBe(expectedSsn);
        _mockDemographicReader.Verify(r => r.BuildDemographicQuery(
            It.IsAny<IProfitSharingDbContext>(), false), Times.Once);
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

        // Setup demographic reader to return MockDbSet.Object for async support
        var mockDemographicsDbSet = demographicsList.BuildMockDbSet();
        _mockDemographicReader.Setup(r => r.BuildDemographicQuery(
            It.IsAny<IProfitSharingDbContext>(), false))
            .ReturnsAsync(mockDemographicsDbSet.Object);

        var service = CreateService();

        // Act
        var ssn = await service.FindEmployeeSsnByBadgeAsync(badgeNumber);

        // Assert
        ssn.ShouldBe(0);
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

        // Setup demographic reader to return empty MockDbSet.Object for async support
        var mockDemographicsDbSet = demographicsList.BuildMockDbSet();
        _mockDemographicReader.Setup(r => r.BuildDemographicQuery(
            It.IsAny<IProfitSharingDbContext>(), false))
            .ReturnsAsync(mockDemographicsDbSet.Object);

        var service = CreateService();

        // Act
        var result = await service.GetEmployeeDetailsAsync(employeeId, currentYear, previousYear);

        // Assert
        result.ssn.ShouldBe(0);
        result.memberDetails.ShouldNotBeNull();
        result.memberDetails.Id.ShouldBe(0);
    }
}
