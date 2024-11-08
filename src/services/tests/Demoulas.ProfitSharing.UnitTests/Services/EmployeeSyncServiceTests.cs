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

        _employeeSyncService = new EmployeeSyncService(
            mockHttpClient.Object,
            _mockDemographicsService.Object,
            mockDataContextFactory.Object,
            oracleHcmConfig,
            employeeValidator
        );
    }

    [Fact]
    public async Task SynchronizeEmployees_ShouldCallAddDemographicsStream()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        // Act
        await _employeeSyncService.SynchronizeEmployees("Unit Test", cancellationToken);
        // Assert
        _mockDemographicsService.Verify(d => d.AddDemographicsStream(It.IsAny<IAsyncEnumerable<DemographicsRequest>>(), It.IsAny<byte>(), cancellationToken),
            Times.Once);
    }
}
