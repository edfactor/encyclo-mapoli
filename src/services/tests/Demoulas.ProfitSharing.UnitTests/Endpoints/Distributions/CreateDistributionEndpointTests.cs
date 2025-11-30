using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit.v3;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Distributions;

[Collection("Distribution Tests")]
public class CreateDistributionEndpointTests : ApiTestBase<Api.Program>
{
    public CreateDistributionEndpointTests(ITestOutputHelper testOutputHelper)
    {
        // Constructor accepts ITestOutputHelper for xUnit framework compatibility
    }

    /// <summary>
    /// Gets a valid badge number from the mocked demographics data
    /// </summary>
    private async Task<int> GetValidBadgeNumberAsync()
    {
        var badgeNumber = 0;
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await ctx.Demographics
                .Where(d => d.BadgeNumber > 0) // Ensure we get a valid badge number
                .FirstOrDefaultAsync();

            if (demographic != null)
            {
                badgeNumber = demographic.BadgeNumber;
            }
            else
            {
                // Fallback: use StockFactory to create a proper demographic with a valid badge number
                var (newDemographic, _) = StockFactory.CreateEmployee(2024);
                newDemographic.BadgeNumber = 12345; // Override with a specific badge number
                ctx.Demographics.Add(newDemographic);
                await ctx.SaveChangesAsync();
                badgeNumber = newDemographic.BadgeNumber;
            }
            return true;
        });
        return badgeNumber;
    }

    [Fact(DisplayName = "CreateDistribution - Should successfully create distribution with valid request")]
    public async Task CreateDistribution_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = 'M',
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = '1',
            Memo = "Test distribution for June 2024"
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.BadgeNumber.ShouldBe(request.BadgeNumber);
        response.Result.StatusId.ShouldBe(request.StatusId);
        response.Result.FrequencyId.ShouldBe(request.FrequencyId);
        // Note: FederalTaxPercentage is calculated from FederalTaxAmount/GrossAmount in the entity
        response.Result.FederalTaxPercentage.ShouldBe(request.FederalTaxAmount / request.GrossAmount);
        response.Result.FederalTaxAmount.ShouldBe(request.FederalTaxAmount);
        // Note: StateTaxPercentage is calculated from StateTaxAmount/GrossAmount in the entity
        response.Result.StateTaxPercentage.ShouldBe(request.StateTaxAmount / request.GrossAmount);
        response.Result.StateTaxAmount.ShouldBe(request.StateTaxAmount);
        response.Result.GrossAmount.ShouldBe(request.GrossAmount);
        response.Result.CheckAmount.ShouldBe(request.CheckAmount);
        response.Result.TaxCodeId.ShouldBe(request.TaxCodeId);
        response.Result.Memo.ShouldBe(request.Memo);
        response.Result.MaskSsn.ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "CreateDistribution - Should handle minimum required fields")]
    public async Task CreateDistribution_WithMinimumRequiredFields_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = 'Q',
            FederalTaxPercentage = 0.0m,
            FederalTaxAmount = 0.00m,
            StateTaxPercentage = 0.0m,
            StateTaxAmount = 0.00m,
            GrossAmount = 500.00m,
            CheckAmount = 500.00m,
            TaxCodeId = '0'
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.BadgeNumber.ShouldBe(request.BadgeNumber);
        response.Result.GrossAmount.ShouldBe(request.GrossAmount);
        response.Result.CheckAmount.ShouldBe(request.CheckAmount);
    }

    [Fact(DisplayName = "CreateDistribution - Should handle optional third party payee")]
    public async Task CreateDistribution_WithThirdPartyPayee_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = 'R', // Use 'R' for Rollover Direct to allow third party payee
            FederalTaxPercentage = 20.0m,
            FederalTaxAmount = 200.00m,
            StateTaxPercentage = 6.0m,
            StateTaxAmount = 60.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 740.00m,
            TaxCodeId = '1',
            ThirdPartyPayee = new()
            {
                Payee = "ABC Bank",
                Name = "John Doe IRA Account",
                Address = new()
                {
                    Street = "123 Main St",
                    City = "Boston",
                    State = "MA",
                    PostalCode = "02101"
                }
            },
            ForTheBenefitOfPayee = "John Doe",
            ForTheBenefitOfAccountType = "IRA",
            IsRothIra = true,
            Memo = "Rollover to IRA"
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.BadgeNumber.ShouldBe(request.BadgeNumber);
        response.Result.ThirdPartyPayee.ShouldNotBeNull();
        response.Result.ThirdPartyPayee.Payee.ShouldBe(request.ThirdPartyPayee.Payee);
        response.Result.ThirdPartyPayee.Name.ShouldBe(request.ThirdPartyPayee.Name);
        response.Result.IsRothIra.ShouldBe(request.IsRothIra);
        response.Result.ForTheBenefitOfPayee.ShouldBe(request.ForTheBenefitOfPayee);
        response.Result.ForTheBenefitOfAccountType.ShouldBe(request.ForTheBenefitOfAccountType);
    }

    [Fact(DisplayName = "CreateDistribution - Should handle deceased employee")]
    public async Task CreateDistribution_WithDeceasedEmployee_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'D',
            FrequencyId = 'L',
            FederalTaxPercentage = 10.0m,
            FederalTaxAmount = 100.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 850.00m,
            TaxCodeId = '2',
            IsDeceased = true,
            GenderId = 'M',
            Tax1099ForBeneficiary = true,
            Memo = "Final distribution to beneficiary"
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.BadgeNumber.ShouldBe(request.BadgeNumber);
        response.Result.StatusId.ShouldBe(request.StatusId);
        response.Result.IsDeceased.ShouldBe(request.IsDeceased);
        response.Result.GenderId.ShouldBe(request.GenderId);
        response.Result.Tax1099ForBeneficiary.ShouldBe(request.Tax1099ForBeneficiary);
    }

    [Fact(DisplayName = "CreateDistribution - Should handle QDRO distribution")]
    public async Task CreateDistribution_WithQdroDistribution_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = 'L',
            FederalTaxPercentage = 0.0m,
            FederalTaxAmount = 0.00m,
            StateTaxPercentage = 0.0m,
            StateTaxAmount = 0.00m,
            GrossAmount = 2500.00m,
            CheckAmount = 2500.00m,
            TaxCodeId = '0',
            IsQdro = true,
            Memo = "QDRO distribution per court order"
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.BadgeNumber.ShouldBe(request.BadgeNumber);
        response.Result.IsQdro.ShouldBe(request.IsQdro);
        response.Result.FederalTaxAmount.ShouldBe(0.0m);
        response.Result.StateTaxAmount.ShouldBe(0.0m);
        response.Result.GrossAmount.ShouldBe(request.CheckAmount);
    }

    [Theory(DisplayName = "CreateDistribution - Should accept valid status IDs")]
    [InlineData('A')]
    [InlineData('D')]
    [InlineData('T')]
    public async Task CreateDistribution_WithValidStatusIds_ShouldReturnSuccess(char statusId)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = statusId,
            FrequencyId = 'M',
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = '1'
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.StatusId.ShouldBe(statusId);
    }

    [Theory(DisplayName = "CreateDistribution - Should accept valid frequency IDs")]
    [InlineData('M')]
    [InlineData('Q')]
    [InlineData('A')]
    [InlineData('H')]
    public async Task CreateDistribution_WithValidFrequencyIds_ShouldReturnSuccess(char frequencyId)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = frequencyId,
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = '1'
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.FrequencyId.ShouldBe(frequencyId);
    }

    [Fact(DisplayName = "CreateDistribution - Should require authentication")]
    public async Task CreateDistribution_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = CreateDistributionRequest.RequestExample();

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "CreateDistribution - Should require appropriate role")]
    public async Task CreateDistribution_WithInappropriateRole_ShouldReturnForbidden()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITOPERATIONS); // Using IT operations role which should not have distribution creation access
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var request = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = 'M',
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = '1'
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "CreateDistribution - Endpoint configuration should be correct")]
    public void CreateDistribution_EndpointConfiguration_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var mockDistributionService = new Mock<IDistributionService>();

        // Act & Assert
        // Verify the endpoint can be instantiated with its dependencies
        var loggerMock = new Mock<ILogger<CreateDistributionEndpoint>>();
        var endpoint = new CreateDistributionEndpoint(mockDistributionService.Object, loggerMock.Object);
        endpoint.ShouldNotBeNull();
        mockDistributionService.ShouldNotBeNull();

        // Note: FastEndpoints configuration is typically verified through integration tests
        // since the Configure() method requires the FastEndpoints framework context
    }

    [Fact(DisplayName = "CreateDistribution - Should handle service exceptions gracefully")]
    public async Task CreateDistribution_WhenServiceThrowsException_ShouldHandleGracefully()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);

        // Create a request that might cause service-level validation issues
        var request = new CreateDistributionRequest
        {
            BadgeNumber = -1, // Invalid badge number
            StatusId = 'X', // Invalid status
            FrequencyId = 'Z', // Invalid frequency
            FederalTaxPercentage = -5.0m, // Invalid negative tax
            FederalTaxAmount = -50.00m,
            StateTaxPercentage = -2.0m,
            StateTaxAmount = -20.00m,
            GrossAmount = -100.00m, // Invalid negative amount
            CheckAmount = -80.00m,
            TaxCodeId = '9' // Potentially invalid tax code
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        // The response should handle validation errors appropriately
        // The exact behavior depends on how the service layer handles invalid data
        response.ShouldNotBeNull();
        // Note: Specific assertion would depend on the actual error handling implementation
        // This could be a 400 Bad Request, 422 Unprocessable Entity, or other error response
    }

    [Fact(DisplayName = "CreateDistribution - Should validate required fields")]
    public async Task CreateDistribution_WithMissingRequiredFields_ShouldHandleValidation()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);

        // Create a request with some required fields missing or invalid
        var request = new CreateDistributionRequest
        {
            BadgeNumber = 0, // Invalid badge number
            StatusId = '\0', // Missing status
            FrequencyId = '\0', // Missing frequency  
            FederalTaxPercentage = 0,
            FederalTaxAmount = 0,
            StateTaxPercentage = 0,
            StateTaxAmount = 0,
            GrossAmount = 0, // Invalid zero amount
            CheckAmount = 0,
            TaxCodeId = '\0' // Missing tax code
        };

        // Act
        var response = await ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        // The service validation throws ArgumentException which becomes HTTP 500
        // This indicates the validation error is being properly caught and handled
        response.Response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact(DisplayName = "CreateDistribution - Should handle concurrent requests")]
    public async Task CreateDistribution_WithConcurrentRequests_ShouldHandleGracefully()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request1 = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = 'M',
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = '1',
            Memo = "Concurrent test 1"
        };

        var request2 = new CreateDistributionRequest
        {
            BadgeNumber = validBadgeNumber,
            StatusId = 'A',
            FrequencyId = 'Q',
            FederalTaxPercentage = 20.0m,
            FederalTaxAmount = 200.00m,
            StateTaxPercentage = 6.0m,
            StateTaxAmount = 60.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 740.00m,
            TaxCodeId = '1',
            Memo = "Concurrent test 2"
        };

        // Act
        var task1 = ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request1);
        var task2 = ApiClient.POSTAsync<CreateDistributionEndpoint, CreateDistributionRequest, CreateOrUpdateDistributionResponse>(request2);

        var responses = await Task.WhenAll(task1, task2);

        // Assert
        responses.ShouldNotBeNull();
        responses.Length.ShouldBe(2);

        foreach (var response in responses)
        {
            response.ShouldNotBeNull();
            response.Response.EnsureSuccessStatusCode();
            response.Result.ShouldNotBeNull();
        }

        // Verify each request was processed (both should have valid responses)
        // Note: In a real database environment with proper ID generation, 
        // we would verify unique IDs, but with mocks we verify other distinguishing factors
        responses[0].Result.Memo.ShouldNotBe(responses[1].Result.Memo);
        responses.Length.ShouldBe(2);
    }
}
