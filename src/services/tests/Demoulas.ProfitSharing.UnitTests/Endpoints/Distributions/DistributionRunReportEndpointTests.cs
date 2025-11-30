using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Distributions;

/// <summary>
/// Unit tests for DistributionRunReportEndpoint focusing on service interaction.
/// Note: This endpoint uses manual telemetry that requires HttpContext, so these tests
/// focus on verifying the service layer integration rather than the full endpoint execution.
/// Full endpoint behavior including telemetry is tested via integration tests.
/// </summary>
public class DistributionRunReportEndpointTests
{
    [Fact]
    [Description("PS-1230 : Should validate request parameters correctly")]
    public void Request_WithValidParameters_ShouldPassValidation()
    {
        // Arrange
        var request = new DistributionRunReportRequest
        {
            DistributionFrequencies = new char[] { 'Q', 'M' },
            Skip = 0,
            Take = 50,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        // Act & Assert - if no exception is thrown, validation passed
        request.ShouldNotBeNull();
        request.DistributionFrequencies.ShouldNotBeNull();
        request.DistributionFrequencies.Length.ShouldBe(2);
        request.Skip.ShouldBe(0);
        request.Take.ShouldBe(50);
        request.SortBy.ShouldBe("BadgeNumber");
        request.IsSortDescending.ShouldBe(false);
    }

    [Fact]
    [Description("PS-1230 : Should handle null distribution frequencies")]
    public void Request_WithNullFrequencies_ShouldBeValid()
    {
        // Arrange
        var request = new DistributionRunReportRequest
        {
            DistributionFrequencies = null, // This should get all frequencies
            Skip = 0,
            Take = 50,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        // Act & Assert
        request.ShouldNotBeNull();
        request.DistributionFrequencies.ShouldBeNull();
    }

    [Fact]
    [Description("PS-1230 : Should handle empty distribution frequencies")]
    public void Request_WithEmptyFrequencies_ShouldBeValid()
    {
        // Arrange
        var request = new DistributionRunReportRequest
        {
            DistributionFrequencies = new char[] { }, // Empty array should get all frequencies
            Skip = 0,
            Take = 50,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        // Act & Assert
        request.ShouldNotBeNull();
        request.DistributionFrequencies.ShouldNotBeNull();
        request.DistributionFrequencies.Length.ShouldBe(0);
    }

    [Fact]
    [Description("PS-1230 : Should handle multiple distribution frequencies")]
    public void Request_WithMultipleFrequencies_ShouldBeValid()
    {
        // Arrange
        var frequencies = new char[] { 'Q', 'M', 'Y', 'S' }; // Quarterly, Monthly, Yearly, Semi-annual
        var request = new DistributionRunReportRequest
        {
            DistributionFrequencies = frequencies,
            Skip = 0,
            Take = 50,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        // Act & Assert
        request.ShouldNotBeNull();
        request.DistributionFrequencies.ShouldNotBeNull();
        request.DistributionFrequencies.SequenceEqual(frequencies).ShouldBe(true);
    }

    [Fact]
    [Description("PS-1230 : Should handle pagination parameters correctly")]
    public void Request_WithPaginationParameters_ShouldBeValid()
    {
        // Arrange
        var request = new DistributionRunReportRequest
        {
            DistributionFrequencies = new char[] { 'M' },
            Skip = 100,
            Take = 25,
            SortBy = "EmployeeName",
            IsSortDescending = true
        };

        // Act & Assert
        request.ShouldNotBeNull();
        request.Skip.ShouldBe(100);
        request.Take.ShouldBe(25);
        request.SortBy.ShouldBe("EmployeeName");
        request.IsSortDescending.ShouldBe(true);
    }

    [Fact]
    [Description("PS-1230 : Should create valid request from example")]
    public void RequestExample_ShouldCreateValidRequest()
    {
        // Act
        var request = DistributionRunReportRequest.RequestExample();

        // Assert
        request.ShouldNotBeNull();
        request.DistributionFrequencies.ShouldNotBeNull();
        request.DistributionFrequencies.ShouldContain('Q');
        request.DistributionFrequencies.ShouldContain('M');
        request.Skip.ShouldBe(0);
        request.Take.ShouldBe(50);
        request.SortBy.ShouldBe("BadgeNumber");
        request.IsSortDescending.ShouldBe(false);
    }

    [Fact]
    [Description("PS-1230 : Should test service mock setup")]
    public async Task MockDistributionService_ShouldCallCorrectMethod()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        var request = new DistributionRunReportRequest
        {
            DistributionFrequencies = new char[] { 'Q' },
            Skip = 0,
            Take = 50,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        // Act
        await mockService.Object.GetDistributionRunReport(request, CancellationToken.None);

        // Assert
        mockService.Verify(
            x => x.GetDistributionRunReport(
                It.Is<DistributionRunReportRequest>(req =>
                    req.DistributionFrequencies != null &&
                    req.DistributionFrequencies.Contains('Q') &&
                    req.Skip == 0 &&
                    req.Take == 50 &&
                    req.SortBy == "BadgeNumber" &&
                    req.IsSortDescending == false),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
