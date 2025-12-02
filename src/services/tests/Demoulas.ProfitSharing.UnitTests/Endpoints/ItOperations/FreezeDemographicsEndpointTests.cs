using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;
using Microsoft.Extensions.Logging;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;

public class FreezeDemographicsEndpointTests
{
    private readonly Mock<IFrozenService> _frozenServiceMock;
    private readonly FreezeDemographicsEndpoint _endpoint;

    public FreezeDemographicsEndpointTests()
    {
        _frozenServiceMock = new Mock<IFrozenService>();
        var appUserMock = new Mock<IAppUser>();
        appUserMock.Setup(u => u.UserName).Returns("TestUser");
        var loggerMock = new Mock<ILogger<FreezeDemographicsEndpoint>>();
        _endpoint = new FreezeDemographicsEndpoint(_frozenServiceMock.Object, appUserMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFrozenStateResponse()
    {
        // Arrange
        var request = new SetFrozenStateRequest
        {
            AsOfDateTime = DateTime.Today,
            ProfitYear = (short)DateTime.Today.Year
        };

        var expectedResponse = new FrozenStateResponse
        {
            Id = 1,
            ProfitYear = request.ProfitYear,
            FrozenBy = "TestUser",
            AsOfDateTime = request.AsOfDateTime,
            IsActive = true
        };

        _frozenServiceMock
            .Setup(service => service.FreezeDemographics(request.ProfitYear, request.AsOfDateTime, expectedResponse.FrozenBy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse.Id, response.Id);
        Assert.Equal(expectedResponse.ProfitYear, response.ProfitYear);
        Assert.Equal(expectedResponse.FrozenBy, response.FrozenBy);
        Assert.Equal(expectedResponse.AsOfDateTime, response.AsOfDateTime);
        Assert.Equal(expectedResponse.IsActive, response.IsActive);
    }
}
