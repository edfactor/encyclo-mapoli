using System.Reflection;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(loggerMock.Object);
        SetHttpContext(_endpoint, new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        });
    }

    private static void SetHttpContext(FreezeDemographicsEndpoint endpoint, HttpContext httpContext)
    {
        var endpointType = endpoint.GetType();
        var property = endpointType.GetProperty(
            "HttpContext",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property is not null && property.SetMethod is not null)
        {
            property.SetValue(endpoint, httpContext);
            return;
        }

        var fields = endpointType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (typeof(HttpContext).IsAssignableFrom(field.FieldType))
            {
                field.SetValue(endpoint, httpContext);
                return;
            }
        }
    }

    private static Task<FrozenStateResponse> InvokeHandleRequestAsync(
        FreezeDemographicsEndpoint endpoint,
        SetFrozenStateRequest request,
        CancellationToken cancellationToken)
    {
        var method = endpoint.GetType().GetMethod(
            "HandleRequestAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var task = (Task<FrozenStateResponse>)method!
            .Invoke(endpoint, new object[] { request, cancellationToken })!;
        return task;
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
        var response = await InvokeHandleRequestAsync(_endpoint, request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse.Id, response.Id);
        Assert.Equal(expectedResponse.ProfitYear, response.ProfitYear);
        Assert.Equal(expectedResponse.FrozenBy, response.FrozenBy);
        Assert.Equal(expectedResponse.AsOfDateTime, response.AsOfDateTime);
        Assert.Equal(expectedResponse.IsActive, response.IsActive);
    }
}
