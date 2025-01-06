using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Clients;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.ProfitSharing.UnitTests.Base;
using Moq;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.UnitTests.Services;

public class EmployeeSyncServiceTests : IClassFixture<ApiTestBase<Program>>
{
    private readonly Mock<IDemographicsServiceInternal> _mockDemographicsService;
    private readonly IEmployeeSyncService _employeeSyncService;

    public EmployeeSyncServiceTests()
    {
        _mockDemographicsService = new Mock<IDemographicsServiceInternal>();
        Mock<IProfitSharingDataContextFactory> mockDataContextFactory = new Mock<IProfitSharingDataContextFactory>();
        Mock<IBaseCacheService<LookupTableCache<byte>>> mockAccountCache = new Mock<IBaseCacheService<LookupTableCache<byte>>>();
        Mock<IBaseCacheService<LookupTableCache<byte>>> mockDepCache = new Mock<IBaseCacheService<LookupTableCache<byte>>>();
        OracleHcmConfig oracleHcmConfig = new OracleHcmConfig { BaseAddress = "localhost", DemographicUrl = string.Empty };
        OracleEmployeeValidator employeeValidator = new OracleEmployeeValidator(mockAccountCache.Object, mockDepCache.Object);
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        var log = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
        }).CreateLogger<AtomFeedClient>();
        AtomFeedClient atomFeedClient = new AtomFeedClient(mockHttpClient.Object, log);
        OracleEmployeeDataSyncClient employeeDataSyncClient = new OracleEmployeeDataSyncClient(mockHttpClient.Object, oracleHcmConfig);

        _employeeSyncService = new EmployeeSyncService(
            _mockDemographicsService.Object,
            atomFeedClient,
            employeeDataSyncClient,
            mockDataContextFactory.Object,
            oracleHcmConfig,
            employeeValidator
        );
    }

    [Fact]
    public async Task SynchronizeEmployees_ShouldCallAddDemographicsStream()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        // Act
        await _employeeSyncService.ExecuteFullSyncAsync("Unit Test", cancellationToken);
        // Assert
        _mockDemographicsService.Verify(d => d.AddDemographicsStreamAsync(It.IsAny<IAsyncEnumerable<DemographicsRequest>>(), It.IsAny<byte>(), cancellationToken),
            Times.Once);
    }
}
