using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Demographics;
public class GetFrozenDemographicsEndpointTests
{
    private readonly Mock<IFrozenService> _frozenServiceMock;
    private readonly GetFrozenDemographicsEndpoint _endpoint;

    public GetFrozenDemographicsEndpointTests()
    {
        _frozenServiceMock = new Mock<IFrozenService>();
        _endpoint = new GetFrozenDemographicsEndpoint(_frozenServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsFrozenDemographics()
    {
        // Arrange
        var frozenDemographics = new List<FrozenStateResponse>
        {
            new FrozenStateResponse { Id = 1, ProfitYear = 2022, FrozenBy = "User1", AsOfDateTime = DateTime.Now, IsActive = true },
            new FrozenStateResponse { Id = 2, ProfitYear = 2023, FrozenBy = "User2", AsOfDateTime = DateTime.Now, IsActive = false }
        };
        _frozenServiceMock.Setup(service => service.GetFrozenDemographics(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(frozenDemographics);

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.Equal(frozenDemographics, result);
    }
}
