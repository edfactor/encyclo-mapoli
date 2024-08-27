using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.ProfitSharing.UnitTests.Base;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests;

public class EmployeeSyncServiceTests : IClassFixture<ApiTestBase<Program>>
{
    private readonly Mock<IDemographicsServiceInternal> _mockDemographicsService;
    private readonly Mock<IProfitSharingDataContextFactory> _mockDataContextFactory;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly Mock<IBaseCacheService<LookupTableCache<byte>>>  _mockAccountCache;
    private readonly Mock<IBaseCacheService<LookupTableCache<byte>>> _mockDepCache;
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly EmployeeSyncService _employeeSyncService;
    private readonly OracleEmployeeValidator _employeeValidator;

    public EmployeeSyncServiceTests()
    {
        _mockDemographicsService = new Mock<IDemographicsServiceInternal>();
        _mockDataContextFactory = new Mock<IProfitSharingDataContextFactory>();
        _mockAccountCache = new Mock<IBaseCacheService<LookupTableCache<byte>>>();
        _mockDepCache = new Mock<IBaseCacheService<LookupTableCache<byte>>>();
        _oracleHcmConfig = new OracleHcmConfig {Url = "localhost"};
        _employeeValidator = new OracleEmployeeValidator(_mockAccountCache.Object, _mockDepCache.Object);
        _mockHttpClient = new Mock<HttpClient>();
        _employeeSyncService = new EmployeeSyncService(
            _mockHttpClient.Object,
            _mockDemographicsService.Object,
            _mockDataContextFactory.Object,
            _oracleHcmConfig,
            _employeeValidator
        );
    }

    [Fact]
    public async Task SynchronizeEmployees_ShouldCallAddDemographicsStream()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        // Act
        await _employeeSyncService.SynchronizeEmployees(cancellationToken);
        // Assert
        _mockDemographicsService.Verify(d => d.AddDemographicsStream(It.IsAny<IAsyncEnumerable<DemographicsRequest>>(), It.IsAny<byte>(), cancellationToken),
            Times.Once);
    }
    // Additional tests for other methods and scenarios can be added here
}
