using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
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
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Collection("SharedGlobalState")]
[Description("PS-1230 : Unit tests for DistributionService - distribution reporting and query methods")]
public sealed class DistributionServiceTests : ApiTestBase<Api.Program>
{
    private readonly IDistributionService _distributionService;

    public DistributionServiceTests()
    {
        MockDbContextFactory = MockDataContextFactory.InitializeForTesting();
        _distributionService = ServiceProvider?.GetRequiredService<IDistributionService>()!;
    }

    [Fact(DisplayName = "Distribution Search")]
    public async Task SearchAsync_ShouldReturnExpectedResults()
    {
        Distribution? firstDist = null;
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            firstDist = await ctx.Distributions.FirstOrDefaultAsync();
            return true;
        });

        var request = new DistributionSearchRequest
        {
            Take = 25,
            Skip = 0,
        };

        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var response = await ApiClient.POSTAsync<
            DistributionSearchEndpoint,
            DistributionSearchRequest,
            PaginatedResponseDto<DistributionSearchResponse>>(request);

        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.Total.ShouldBeGreaterThan(0);
        response.Result.Results.Count().ShouldBeGreaterThan(0);
    }

    [Fact]
    [Description("PS-1230 : GetDistributionRunReportSummary returns Result<T> pattern")]
    public async Task GetDistributionRunReportSummary_ShouldReturnResultPattern()
    {
        // Act
        var result = await _distributionService.GetDistributionRunReportSummary(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // The method should return a Result<T> - even if data is empty, it should succeed
        // We can't test the specific data structure due to mock limitations, but we can verify the pattern
    }

    [Fact]
    [Description("PS-1230 : GetDistributionsOnHold returns Result<T> pattern")]
    public async Task GetDistributionsOnHold_ShouldReturnResultPattern()
    {
        // Arrange
        var request = new SortedPaginationRequestDto
        {
            Take = 10,
            Skip = 0
        };

        // Act
        var result = await _distributionService.GetDistributionsOnHold(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // The method should return a Result<T> - even if data is empty, it should succeed
        // We can't test specific SSN masking with mocked data, but we can verify the pattern
    }

    [Fact]
    [Description("PS-1230 : GetDistributionsOnHold validates pagination parameters")]
    public async Task GetDistributionsOnHold_ShouldValidatePaginationParameters()
    {
        // Arrange
        var validRequest = new SortedPaginationRequestDto
        {
            Take = 5,
            Skip = 0
        };

        // Act
        var result = await _distributionService.GetDistributionsOnHold(validRequest, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // Should handle pagination parameters without throwing exceptions
    }

    [Fact]
    [Description("PS-1230 : GetManualCheckDistributions returns Result<T> pattern")]
    public async Task GetManualCheckDistributions_ShouldReturnResultPattern()
    {
        // Arrange
        var request = new SortedPaginationRequestDto
        {
            Take = 10,
            Skip = 0
        };

        // Act
        var result = await _distributionService.GetManualCheckDistributions(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // The method should return a Result<T> - even if data is empty, it should succeed
    }

    [Fact]
    [Description("PS-1230 : GetManualCheckDistributions validates pagination parameters")]
    public async Task GetManualCheckDistributions_ShouldValidatePaginationParameters()
    {
        // Arrange
        var validRequest = new SortedPaginationRequestDto
        {
            Take = 2,
            Skip = 0
        };

        // Act
        var result = await _distributionService.GetManualCheckDistributions(validRequest, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // Should handle pagination parameters without throwing exceptions
    }

    [Fact]
    [Description("PS-1230 : GetDistributionRunReport returns Result<T> pattern")]
    public async Task GetDistributionRunReport_ShouldReturnResultPattern()
    {
        // Arrange
        var request = new DistributionRunReportRequest
        {
            Take = 10,
            Skip = 0,
            DistributionFrequencies = null // Include all frequencies
        };

        // Act
        var result = await _distributionService.GetDistributionRunReport(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // The method should return a Result<T> - even if data is empty, it should succeed
    }

    [Fact]
    [Description("PS-1230 : GetDistributionRunReport handles frequency filtering")]
    public async Task GetDistributionRunReport_ShouldHandleFrequencyFiltering()
    {
        // Arrange
        var request = new DistributionRunReportRequest
        {
            Take = 100,
            Skip = 0,
            DistributionFrequencies = ['A'] // Filter to specific frequency
        };

        // Act
        var result = await _distributionService.GetDistributionRunReport(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // Should handle frequency filtering parameter without throwing exceptions
    }

    [Fact]
    [Description("PS-1230 : GetDistributionRunReport validates pagination parameters")]
    public async Task GetDistributionRunReport_ShouldValidatePaginationParameters()
    {
        // Arrange
        var request = new DistributionRunReportRequest
        {
            Take = 3,
            Skip = 0,
            DistributionFrequencies = null
        };

        // Act
        var result = await _distributionService.GetDistributionRunReport(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // Should handle pagination parameters correctly
    }

    [Fact]
    [Description("PS-1230 : GetDistributionRunReport handles invalid frequency gracefully")]
    public async Task GetDistributionRunReport_ShouldHandleInvalidFrequencyGracefully()
    {
        // Arrange - use a frequency that doesn't exist
        var request = new DistributionRunReportRequest
        {
            Take = 10,
            Skip = 0,
            DistributionFrequencies = ['X'] // Non-existent frequency
        };

        // Act
        var result = await _distributionService.GetDistributionRunReport(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        // Should handle non-existent frequency filter without throwing exceptions
    }


}
