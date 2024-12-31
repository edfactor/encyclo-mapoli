using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Base;
using Moq;

as.ProfitSharing.UnitTests.Base;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;

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
        }).CreateLogger<AtomFeedService>();
        AtomFeedService atomFeedService = new AtomFeedService(mockHttpClient.Object, log);

        _employeeSyncService = new EmployeeSyncService(
            mockHttpClient.Object,
            _mockDemographicsService.Object,
            atomFeedService,
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
        await _employeeSyncService.SynchronizeEmployeesAsync("Unit Test", cancellationToken);
        // Assert
        _mockDemographicsService.Verify(d => d.AddDemographicsStreamAsync(It.IsAny<IAsyncEnumerable<DemographicsRequest>>(), It.IsAny<byte>(), cancellationToken),
            Times.Once);
    }
}
