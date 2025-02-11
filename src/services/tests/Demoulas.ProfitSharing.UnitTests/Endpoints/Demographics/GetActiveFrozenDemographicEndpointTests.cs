using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Demographics;

public class GetActiveFrozenDemographicEndpointTests
{
    private readonly Mock<IFrozenService> _frozenServiceMock;
    private readonly GetActiveFrozenDemographicEndpoint _endpoint;

    public GetActiveFrozenDemographicEndpointTests()
    {
        _frozenServiceMock = new Mock<IFrozenService>();
        _endpoint = new GetActiveFrozenDemographicEndpoint(_frozenServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsActiveFrozenDemographic()
    {
        // Arrange
        var expectedResponse = new FrozenStateResponse
        {
            Id = 1,
            ProfitYear = 2023,
            FrozenBy = "TestUser",
            AsOfDateTime = DateTime.Now,
            IsActive = true
        };
        _frozenServiceMock.Setup(service => service.GetActiveFrozenDemographic(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(expectedResponse);

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse, result);
    }
}
