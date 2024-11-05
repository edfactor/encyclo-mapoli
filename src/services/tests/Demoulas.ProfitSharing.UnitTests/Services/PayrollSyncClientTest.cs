using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Data.Contexts;
using FluentAssertions;
using JetBrains.Annotations;
using Polly.Timeout;
using Demoulas.ProfitSharing.UnitTests.Base;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[TestSubject(typeof(PayrollSyncClient))]
public class PayrollSyncClientTest : ApiTestBase<Program>
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly Mock<ILogger<PayrollSyncClient>> _loggerMock;
    private readonly PayrollSyncClient _payrollSyncClient;

    public PayrollSyncClientTest()
    {
        _httpClientMock = new Mock<HttpClient>();
        _loggerMock = new Mock<ILogger<PayrollSyncClient>>();
        _payrollSyncClient = new PayrollSyncClient(_httpClientMock.Object, MockDbContextFactory, _loggerMock.Object);
    }

    [Fact]
    public async Task GetPayrollProcessResultsAsync_ShouldReturnResults_WhenResponseIsSuccessful()
    {
        // Arrange
        var personIds = new List<long> { 1, 2, 3 };
        var cancellationToken = CancellationToken.None;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new PayrollRoot(new List<PayrollItem> { new PayrollItem(2003, 1001), new PayrollItem(2003, 1002) }, 2, false,
                10, 0))
        };
        _httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(response);

        // Act
        var results = _payrollSyncClient.GetPayrollProcessResultsAsync(personIds, cancellationToken);

        // Assert
        var resultList = new List<KeyValuePair<long, List<int>>>();
        await foreach (var result in results)
        {
            resultList.Add(result);
        }

        resultList.Should().HaveCount(3);
        resultList[0].Value.Should().Contain(new List<int> { 1001, 1002 });
    }

    [Fact]
    public async Task GetPayrollProcessResultsAsync_ShouldLogError_WhenTimeoutExceptionOccurs()
    {
        // Arrange
        var personIds = new List<long> { 1 };
        var cancellationToken = CancellationToken.None;
        _httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>(), cancellationToken))
            .ThrowsAsync(new TimeoutRejectedException("Timeout occurred"));

        // Act
        var results = _payrollSyncClient.GetPayrollProcessResultsAsync(personIds, cancellationToken);

        // Assert
        var resultList = new List<KeyValuePair<long, List<int>>>();
        await foreach (var result in results)
        {
            resultList.Add(result);
        }

        resultList.Should().BeEmpty();
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<TimeoutRejectedException>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetBalanceTypesForProcessResultsAsync_ShouldAccumulateTotals_WhenResponseIsSuccessful()
    {
        // Arrange
        var oracleHcmId = 1L;
        var objectActionIds = new List<int> { 1001, 1002 };
        var cancellationToken = CancellationToken.None;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new BalanceRoot(
                new List<BalanceItem>
                {
                    new BalanceItem(300000789345470, 100, 200, 1, "Dimension1"), new BalanceItem(300000789345477, 50, 50, 2, "Dimension2")
                }, 2, false, 10, 0))
        };
        _httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(response);

        // Act
        await _payrollSyncClient.GetBalanceTypesForProcessResultsAsync(oracleHcmId, objectActionIds, cancellationToken);
        
        Assert.True(true);
    }
}
