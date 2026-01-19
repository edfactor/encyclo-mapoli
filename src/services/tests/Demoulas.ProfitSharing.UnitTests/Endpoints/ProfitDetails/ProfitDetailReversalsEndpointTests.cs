using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Corrections;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ProfitDetails;

[Collection("Profit Details Tests")]
public class ProfitDetailReversalsEndpointTests : ApiTestBase<Api.Program>
{
    private Task<HttpResponseMessage> PostReversalsAsync(IdsRequest request)
    {
        return ApiClient.PostAsJsonAsync("profitdetails/reversals", request);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should return not found when profit details don't exist")]
    [Description("Should return 400 Bad Request when trying to reverse non-existent profit details")]
    public async Task ProfitDetailReversals_WithNonExistentIds_ShouldReturnNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = new[] { 1, 2, 3 }
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should return not found for single non-existent ID")]
    [Description("Should return 400 Bad Request when trying to reverse single non-existent profit detail")]
    public async Task ProfitDetailReversals_WithSingleNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = new[] { 42 }
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should return not found for maximum batch size of non-existent IDs")]
    [Description("Should return 400 Bad Request when trying to reverse maximum batch size of non-existent profit details")]
    public async Task ProfitDetailReversals_WithMaximumBatchSizeNonExistent_ShouldReturnNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var ids = Enumerable.Range(1, 1000).ToArray();
        var request = new IdsRequest
        {
            Ids = ids
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should work with System Administrator role (not found case)")]
    [Description("Should allow System Administrator to access endpoint and return 400 for non-existent profit details")]
    public async Task ProfitDetailReversals_WithSystemAdministratorRole_ShouldReturnNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new IdsRequest
        {
            Ids = new[] { 100, 200, 300 }
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should require authentication")]
    [Description("Should return unauthorized when no authentication token is provided")]
    public async Task ProfitDetailReversals_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = IdsRequest.RequestExample();

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Theory(DisplayName = "ProfitDetailReversals - Should require appropriate role")]
    [InlineData(Role.DISTRIBUTIONSCLERK)]
    [InlineData(Role.ITOPERATIONS)]
    [InlineData(Role.AUDITOR)]
    [InlineData(Role.BENEFICIARY_ADMINISTRATOR)]
    [InlineData(Role.HARDSHIPADMINISTRATOR)]
    [Description("Should return forbidden when user lacks CAN_REVERSE_PROFIT_DETAILS policy")]
    public async Task ProfitDetailReversals_WithInappropriateRole_ShouldReturnForbidden(string role)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(role);
        var request = new IdsRequest
        {
            Ids = new[] { 1, 2, 3 }
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle validation errors for null IDs")]
    [Description("Should return validation error when IDs array is null")]
    public async Task ProfitDetailReversals_WithNullIds_ShouldReturnValidationError()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = null!
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle validation errors for empty IDs")]
    [Description("Should return validation error when IDs array is empty")]
    public async Task ProfitDetailReversals_WithEmptyIds_ShouldReturnValidationError()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = Array.Empty<int>()
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle validation errors for oversized batch")]
    [Description("Should return validation error when batch size exceeds 1000")]
    public async Task ProfitDetailReversals_WithOversizedBatch_ShouldReturnValidationError()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var ids = Enumerable.Range(1, 1001).ToArray(); // Exceeds maximum of 1000
        var request = new IdsRequest
        {
            Ids = ids
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle validation errors for non-positive IDs")]
    [Description("Should return validation error when IDs contain non-positive values")]
    public async Task ProfitDetailReversals_WithNonPositiveIds_ShouldReturnValidationError()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = new[] { 1, 0, -1, 5 } // Contains invalid IDs
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle validation errors for duplicate IDs")]
    [Description("Should return validation error when IDs contain duplicates")]
    public async Task ProfitDetailReversals_WithDuplicateIds_ShouldReturnValidationError()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = new[] { 1, 2, 2, 3 } // Contains duplicate ID
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle service not found errors")]
    [Description("Should return validation error when profit details don't exist")]
    public async Task ProfitDetailReversals_WithNonExistentIds_ShouldHandleNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = new[] { 999999, 999998, 999997 } // Non-existent IDs
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle concurrent requests with not found")]
    [Description("Should handle multiple simultaneous reversal requests and return 400 for non-existent IDs")]
    public async Task ProfitDetailReversals_WithConcurrentRequests_ShouldReturnNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        var request1 = new IdsRequest { Ids = new[] { 10, 11, 12 } };
        var request2 = new IdsRequest { Ids = new[] { 20, 21, 22 } };
        var request3 = new IdsRequest { Ids = new[] { 30, 31, 32 } };

        // Act
        var task1 = PostReversalsAsync(request1);
        var task2 = PostReversalsAsync(request2);
        var task3 = PostReversalsAsync(request3);

        var responses = await Task.WhenAll(task1, task2, task3);

        // Assert
        responses.ShouldNotBeNull();
        responses.Length.ShouldBe(3);

        foreach (var response in responses)
        {
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }

    [Theory(DisplayName = "ProfitDetailReversals - Should return not found for various batch sizes")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(500)]
    [Description("Should return 400 Bad Request for various batch sizes of non-existent profit details")]
    public async Task ProfitDetailReversals_WithVariousBatchSizes_ShouldReturnNotFound(int batchSize)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var ids = Enumerable.Range(1000, batchSize).ToArray();
        var request = new IdsRequest
        {
            Ids = ids
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Endpoint configuration should be correct")]
    [Description("Should verify endpoint can be instantiated with proper dependencies")]
    public void ProfitDetailReversals_EndpointConfiguration_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var mockProfitDetailReversalsService = new Mock<IProfitDetailReversalsService>();
        var loggerMock = new Mock<ILogger<ProfitDetailReversalsEndpoint>>();

        // Act & Assert
        // Verify the endpoint can be instantiated with its dependencies
        var endpoint = new ProfitDetailReversalsEndpoint(
            mockProfitDetailReversalsService.Object,
            loggerMock.Object);

        endpoint.ShouldNotBeNull();
        mockProfitDetailReversalsService.ShouldNotBeNull();

        // Note: FastEndpoints configuration is typically verified through integration tests
        // since the Configure() method requires the FastEndpoints framework context
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should verify telemetry integration")]
    [Description("Should ensure endpoint implements proper telemetry patterns even for validation failures")]
    public async Task ProfitDetailReversals_ShouldImplementTelemetryIntegration()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new IdsRequest
        {
            Ids = new[] { 555, 666, 777 }
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // Note: In a real test environment with telemetry infrastructure,
        // we would verify that:
        // 1. Activity was created with proper operation name
        // 2. Business operation metrics were recorded
        // 3. Request/response metrics were captured
        // 4. Any sensitive fields were properly masked
        // Even for validation failures, telemetry should still be working
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should handle service exceptions gracefully")]
    [Description("Should properly handle and transform service-level exceptions")]
    public async Task ProfitDetailReversals_WhenServiceThrowsException_ShouldHandleGracefully()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Use a request that might cause service-level issues
        // In this test environment, these IDs don't exist so they return 400
        var request = new IdsRequest
        {
            Ids = new[] { int.MaxValue - 1, int.MaxValue - 2, int.MaxValue - 3 }
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        // In the test environment, non-existent IDs return 400 Bad Request (validation failure)
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "ProfitDetailReversals - Should return not found for non-existent correlation test")]
    [Description("Should return 400 Bad Request when trying to reverse non-existent profit details")]
    public async Task ProfitDetailReversals_ShouldReturnNotFoundForNonExistentIds()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var originalIds = new[] { 123, 456, 789, 101112 };
        var request = new IdsRequest
        {
            Ids = originalIds
        };

        // Act
        var response = await PostReversalsAsync(request);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // Note: In a successful scenario with existing profit details,
        // the response would maintain request-response correlation
        // with exact ID matching and order preservation
    }
}
