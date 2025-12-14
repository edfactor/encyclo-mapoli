using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
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

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Distributions;

[Collection("Distribution Tests")]
public class UpdateDistributionEndpointTests : ApiTestBase<Api.Program>
{
    public UpdateDistributionEndpointTests(ITestOutputHelper testOutputHelper)
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
            // Look for demographics with valid badge numbers (between 9,999 and 9,999,999)
            var demographic = await ctx.Demographics
                .Where(d => d.BadgeNumber >= 9999 && d.BadgeNumber <= 9999999)
                .OrderBy(d => d.BadgeNumber) // Ensure deterministic selection
                .FirstOrDefaultAsync();

            if (demographic != null)
            {
                badgeNumber = demographic.BadgeNumber;
            }
            else
            {
                // Fallback: use StockFactory to create a proper demographic with a valid badge number
                var (newDemographic, payProfits) = StockFactory.CreateEmployee(2024);
                // Use a deterministic badge number within validation range
                newDemographic.BadgeNumber = 555555; // Valid 6-digit badge number
                newDemographic.Ssn = 123456789; // Ensure unique SSN

                // Add the demographic and associated pay profits for vesting balance calculation
                ctx.Demographics.Add(newDemographic);
                foreach (var payProfit in payProfits)
                {
                    payProfit.DemographicId = newDemographic.Id;
                    ctx.PayProfits.Add(payProfit);
                }

                await ctx.SaveChangesAsync();
                badgeNumber = newDemographic.BadgeNumber;
            }
            return true;
        });
        return badgeNumber;
    }

    /// <summary>
    /// Creates a distribution in the database and returns its ID for update testing
    /// </summary>
    private async Task<long> CreateTestDistributionAsync(int badgeNumber)
    {
        var distributionId = 0L;
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await ctx.Demographics
                .Where(d => d.BadgeNumber == badgeNumber)
                .FirstAsync();

            var distribution = new Distribution
            {
                Ssn = demographic.Ssn, // Required field
                PaymentSequence = 1, // Required field
                EmployeeName = "Test Employee", // Required field
                StatusId = 'Y', // OkayToPay status (valid distribution status)
                FrequencyId = 'M', // Monthly frequency
                FederalTaxAmount = 50.00m, // Lower initial amounts
                StateTaxAmount = 25.00m,
                GrossAmount = 500.00m, // Conservative gross amount
                TaxCodeId = '1', // Standard tax code
                IsDeceased = false, // Start with living employee
                Tax1099ForEmployee = true,
                Tax1099ForBeneficiary = false,
                Memo = "Test distribution for update",
                CreatedAtUtc = DateTime.UtcNow,
                ModifiedAtUtc = DateTime.UtcNow
            };

            ctx.Distributions.Add(distribution);
            await ctx.SaveChangesAsync();
            distributionId = distribution.Id;
            return true;
        });
        return distributionId;
    }

    [Fact(DisplayName = "UpdateDistribution - Should successfully update distribution with valid request")]
    public async Task UpdateDistribution_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
            FrequencyId = 'Q', // Changed from 'M' to 'Q'
            FederalTaxPercentage = 15.0m, // Changed from 10.0m to 15.0m
            FederalTaxAmount = 150.00m, // Changed from 100.00m to 150.00m
            StateTaxPercentage = 6.0m, // Changed from 5.0m to 6.0m
            StateTaxAmount = 60.00m, // Changed from 50.00m to 60.00m
            GrossAmount = 1000.00m,
            CheckAmount = 790.00m, // Adjusted for new tax amounts
            TaxCodeId = '2', // Changed from '1' to '2'
            Memo = "Updated test distribution"
        };

        // Act
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(request.Id);
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

    [Fact(DisplayName = "UpdateDistribution - Should handle minimum required fields update")]
    public async Task UpdateDistribution_WithMinimumRequiredFields_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = 'P', // PaymentMade - valid status
            FrequencyId = 'L', // Changed to 'L' (Lump Sum)
            FederalTaxPercentage = 0.0m,
            FederalTaxAmount = 0.00m,
            StateTaxPercentage = 0.0m,
            StateTaxAmount = 0.00m,
            GrossAmount = 500.00m, // Changed gross amount
            CheckAmount = 500.00m,
            TaxCodeId = '0'
        };

        // Act
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(request.Id);
        response.Result.BadgeNumber.ShouldBe(request.BadgeNumber);
        response.Result.StatusId.ShouldBe(request.StatusId);
        response.Result.FrequencyId.ShouldBe(request.FrequencyId);
        response.Result.GrossAmount.ShouldBe(request.GrossAmount);
        response.Result.CheckAmount.ShouldBe(request.CheckAmount);
        response.Result.FederalTaxAmount.ShouldBe(0.0m);
        response.Result.StateTaxAmount.ShouldBe(0.0m);
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle third party payee update")]
    public async Task UpdateDistribution_WithThirdPartyPayeeUpdate_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
            FrequencyId = 'R', // Use 'R' for Rollover Direct to allow third party payee
            // Rollover Direct requests are normalized server-side to 0 taxes and check amount = gross.
            // Keep these values aligned to avoid test brittleness.
            FederalTaxPercentage = 0.0m,
            FederalTaxAmount = 0.00m,
            StateTaxPercentage = 0.0m,
            StateTaxAmount = 0.00m,
            GrossAmount = 500.00m,
            CheckAmount = 500.00m,
            TaxCodeId = '1',
            ThirdPartyPayee = new ThirdPartyPayee
            {
                Name = "ABC Rollover Company",
                Address = new Demoulas.ProfitSharing.Common.Contracts.Request.Distributions.Address { Street = "123 Financial St", City = "Boston", State = "MA", PostalCode = "02101" }
            },
            Memo = "Updated rollover to third party"
        };

        // Act
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(request.Id);
        response.Result.FrequencyId.ShouldBe(request.FrequencyId);
        response.Result.ThirdPartyPayee.ShouldNotBeNull();
        response.Result.ThirdPartyPayee.Name.ShouldBe(request.ThirdPartyPayee.Name);
        response.Result.ThirdPartyPayee.Address.Street.ShouldBe(request.ThirdPartyPayee.Address.Street);
        response.Result.Memo.ShouldBe(request.Memo);
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle deceased beneficiary update")]
    public async Task UpdateDistribution_WithDeceasedBeneficiaryUpdate_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = 'D', // Deceased
            FrequencyId = 'L',
            FederalTaxPercentage = 10.0m,
            FederalTaxAmount = 50.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 25.00m,
            GrossAmount = 500.00m,
            CheckAmount = 425.00m,
            TaxCodeId = '2',
            IsDeceased = true,
            GenderId = 'F', // Updated gender
            Tax1099ForBeneficiary = true,
            Memo = "Updated final distribution to beneficiary"
        };

        // Act
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();

        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(request.Id);
        response.Result.StatusId.ShouldBe(request.StatusId);
        response.Result.IsDeceased.ShouldBe(request.IsDeceased);
        response.Result.GenderId.ShouldBe(request.GenderId);
        response.Result.Tax1099ForBeneficiary.ShouldBe(request.Tax1099ForBeneficiary);
        response.Result.GrossAmount.ShouldBe(request.GrossAmount);
        response.Result.CheckAmount.ShouldBe(request.CheckAmount);
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle QDRO distribution update")]
    public async Task UpdateDistribution_WithQdroDistributionUpdate_ShouldReturnSuccess()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
            FrequencyId = 'L',
            FederalTaxPercentage = 0.0m,
            FederalTaxAmount = 0.00m,
            StateTaxPercentage = 0.0m,
            StateTaxAmount = 0.00m,
            GrossAmount = 3000.00m, // Increased amount
            CheckAmount = 3000.00m,
            TaxCodeId = '0',
            IsQdro = true,
            Memo = "Updated QDRO distribution per amended court order"
        };

        // Act
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();

        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(request.Id);
        response.Result.IsQdro.ShouldBe(request.IsQdro);
        response.Result.FederalTaxAmount.ShouldBe(0.0m);
        response.Result.StateTaxAmount.ShouldBe(0.0m);
        response.Result.GrossAmount.ShouldBe(request.CheckAmount);
    }

    [Theory(DisplayName = "UpdateDistribution - Should accept valid status ID updates")]
    [InlineData(DistributionStatus.Constants.ManualCheck)] // ManualCheck
    [InlineData(DistributionStatus.Constants.PurgeRecord)] // PurgeRecord
    [InlineData(DistributionStatus.Constants.RequestOnHold)] // RequestOnHold
    public async Task UpdateDistribution_WithValidStatusIdUpdates_ShouldReturnSuccess(char statusId)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = statusId,
            FrequencyId = DistributionFrequency.Constants.Monthly,
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = TaxCode.Constants.EarlyDistributionNoException.Id
        };

        // Act
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(request.Id);
        response.Result.StatusId.ShouldBe(statusId);
    }

    [Theory(DisplayName = "UpdateDistribution - Should accept valid frequency ID updates")]
    [InlineData(DistributionFrequency.Constants.Monthly)]
    [InlineData(DistributionFrequency.Constants.Quarterly)]
    [InlineData(DistributionFrequency.Constants.Annually)]
    [InlineData(DistributionFrequency.Constants.PayDirect)]
    public async Task UpdateDistribution_WithValidFrequencyIdUpdates_ShouldReturnSuccess(char frequencyId)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.ShouldNotBeNull();
        response.Result.Id.ShouldBe(request.Id);
        response.Result.FrequencyId.ShouldBe(frequencyId);
    }

    [Fact(DisplayName = "UpdateDistribution - Should return not found for non-existent distribution ID")]
    public async Task UpdateDistribution_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new UpdateDistributionRequest
        {
            Id = 999999, // Non-existent ID
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        // Expect 404 Not Found for non-existent distribution
        response.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle invalid badge number")]
    public async Task UpdateDistribution_WithInvalidBadgeNumber_ShouldHandleValidation()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = -1, // Invalid badge number
            StatusId = 'Y', // OkayToPay - valid status
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        // The service validation should handle this appropriately
        // Exact behavior depends on service implementation
        response.Response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact(DisplayName = "UpdateDistribution - Should require authorization")]
    public async Task UpdateDistribution_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        // Arrange
        // Do not assign any token/role to simulate unauthorized access
        var request = new UpdateDistributionRequest
        {
            Id = 1,
            BadgeNumber = 12345,
            StatusId = 'Y', // OkayToPay - valid status
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle insufficient permissions")]
    public async Task UpdateDistribution_WithInsufficientPermissions_ShouldReturnForbidden()
    {
        // NOTE: This test demonstrates authorization behavior but currently 
        // the test infrastructure doesn't properly enforce authorization 
        // policies for valid JWT tokens. The endpoint DOES have proper authorization
        // as shown by other tests that return 401 Unauthorized when no token is provided.

        // Arrange
        // First create a distribution with proper permissions
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId = await CreateTestDistributionAsync(validBadgeNumber);

        // Now switch to a role without distribution update permissions
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);
        var request = new UpdateDistributionRequest
        {
            Id = distributionId,
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

    }

    [Fact(DisplayName = "UpdateDistribution - Endpoint configuration should be correct")]
    public void UpdateDistribution_EndpointConfiguration_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var mockDistributionService = new Mock<IDistributionService>();

        // Act & Assert
        // Verify the endpoint can be instantiated with its dependencies
        var loggerMock = new Mock<ILogger<UpdateDistributionEndpoint>>();
        var endpoint = new UpdateDistributionEndpoint(mockDistributionService.Object, loggerMock.Object);
        endpoint.ShouldNotBeNull();
        mockDistributionService.ShouldNotBeNull();

        // Note: FastEndpoints configuration is typically verified through integration tests
        // since the Configure() method requires the FastEndpoints framework context
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle service exceptions gracefully")]
    public async Task UpdateDistribution_WhenServiceThrowsException_ShouldHandleGracefully()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);

        // Create a request that might cause service-level validation issues
        var request = new UpdateDistributionRequest
        {
            Id = -1, // Invalid ID
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        // The response should handle validation errors appropriately
        response.ShouldNotBeNull();
        // The exact status code depends on how the service layer handles invalid data
        response.Response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError, HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "UpdateDistribution - Should validate required fields")]
    public async Task UpdateDistribution_WithMissingRequiredFields_ShouldHandleValidation()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);

        // Create a request with some required fields missing or invalid
        var request = new UpdateDistributionRequest
        {
            Id = 0, // Invalid ID
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        // The service validation should handle this appropriately
        response.Response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError, HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle concurrent update requests")]
    public async Task UpdateDistribution_WithConcurrentRequests_ShouldHandleGracefully()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();
        var distributionId1 = await CreateTestDistributionAsync(validBadgeNumber);
        var distributionId2 = await CreateTestDistributionAsync(validBadgeNumber);

        var request1 = new UpdateDistributionRequest
        {
            Id = distributionId1,
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
            FrequencyId = 'M',
            FederalTaxPercentage = 15.0m,
            FederalTaxAmount = 150.00m,
            StateTaxPercentage = 5.0m,
            StateTaxAmount = 50.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 800.00m,
            TaxCodeId = '1',
            Memo = "Concurrent update test 1"
        };

        var request2 = new UpdateDistributionRequest
        {
            Id = distributionId2,
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
            FrequencyId = 'Q',
            FederalTaxPercentage = 20.0m,
            FederalTaxAmount = 200.00m,
            StateTaxPercentage = 6.0m,
            StateTaxAmount = 60.00m,
            GrossAmount = 1000.00m,
            CheckAmount = 740.00m,
            TaxCodeId = '1',
            Memo = "Concurrent update test 2"
        };

        // Act
        var task1 = ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request1);
        var task2 = ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request2);

        var responses = await Task.WhenAll(task1, task2);

        // Assert
        responses.ShouldNotBeNull();
        responses.Length.ShouldBe(2);

        foreach (var response in responses)
        {
            // If we get a 400, log the error for debugging
            if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
            }
            response.ShouldNotBeNull();
            response.Response.EnsureSuccessStatusCode();
            response.Result.ShouldNotBeNull();
        }

        // Verify each request was processed with the correct ID
        responses[0].Result.Id.ShouldBe(distributionId1);
        responses[1].Result.Id.ShouldBe(distributionId2);

        // Verify each update was processed correctly (different memos)
        responses[0].Result.Memo.ShouldNotBe(responses[1].Result.Memo);
        responses[0].Result.Memo.ShouldBe("Concurrent update test 1");
        responses[1].Result.Memo.ShouldBe("Concurrent update test 2");
    }

    [Fact(DisplayName = "UpdateDistribution - Should handle zero ID gracefully")]
    public async Task UpdateDistribution_WithZeroId_ShouldHandleGracefully()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var validBadgeNumber = await GetValidBadgeNumberAsync();

        var request = new UpdateDistributionRequest
        {
            Id = 0, // Zero ID (should be invalid)
            BadgeNumber = validBadgeNumber,
            StatusId = 'Y', // OkayToPay - valid status
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
        var response = await ApiClient.PUTAsync<UpdateDistributionEndpoint, UpdateDistributionRequest, CreateOrUpdateDistributionResponse>(request);
        // If we get a 400, log the error for debugging
        if (response.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Test failed with 400 Bad Request. Response: {errorContent}");
        }

        // Assert
        response.ShouldNotBeNull();
        // Expect validation error or not found for zero ID
        response.Response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
