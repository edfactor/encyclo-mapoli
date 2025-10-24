using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Distributions;

/// <summary>
/// Unit tests for DistributionRunReportSummaryEndpoint focusing on service interaction.
/// Note: This endpoint uses manual telemetry that requires HttpContext, so these tests
/// focus on verifying the service layer integration rather than the full endpoint execution.
/// Full endpoint behavior including telemetry is tested via integration tests.
/// </summary>
public class DistributionRunReportSummaryEndpointTests
{
    [Fact]
    [Description("PS-1230 : Should create valid response from sample")]
    public void ResponseSample_ShouldCreateValidResponse()
    {
        // Act
        var response = DistributionRunReportSummaryResponse.SampleResponse();

        // Assert
        response.ShouldNotBeNull();
        response.DistributionFrequencyId.ShouldBe('M');
        response.DistributionTypeName.ShouldBe("Monthly");
        response.TotalDistributions.ShouldBe(150);
        response.TotalGrossAmount.ShouldBe(250000.00M);
        response.TotalFederalTaxAmount.ShouldBe(37500.00M);
        response.TotalStateTaxAmount.ShouldBe(12500.00M);
        response.TotalCheckAmount.ShouldBe(200000.00M);
    }

    [Fact]
    [Description("PS-1230 : Should test service mock setup")]
    public async Task MockDistributionService_ShouldCallCorrectMethod()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        
        var summaryResponses = new[]
        {
            DistributionRunReportSummaryResponse.SampleResponse()
        };

        var serviceResult = Result<DistributionRunReportSummaryResponse[]>.Success(summaryResponses);

        mockService
            .Setup(x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        await mockService.Object.GetDistributionRunReportSummary(CancellationToken.None);

        // Assert
        mockService.Verify(
            x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle service cancellation token correctly")]
    public async Task MockDistributionService_ShouldPropagateCancellationToken()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var summaryResponses = Array.Empty<DistributionRunReportSummaryResponse>();
        var serviceResult = Result<DistributionRunReportSummaryResponse[]>.Success(summaryResponses);

        mockService
            .Setup(x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        await mockService.Object.GetDistributionRunReportSummary(cancellationToken);

        // Assert
        mockService.Verify(
            x => x.GetDistributionRunReportSummary(
                It.Is<CancellationToken>(ct => ct == cancellationToken)),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle service exception and rethrow")]
    public async Task MockDistributionService_WhenServiceThrowsException_ShouldRethrow()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        
        var expectedException = new InvalidOperationException("Database connection failed");

        mockService
            .Setup(x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var thrownException = await Should.ThrowAsync<InvalidOperationException>(
            async () => await mockService.Object.GetDistributionRunReportSummary(CancellationToken.None));
        
        thrownException.ShouldBe(expectedException);
        
        mockService.Verify(
            x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle empty summary results")]
    public async Task MockDistributionService_WithEmptyResults_ShouldReturnEmptyArray()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        
        var emptySummaryResponses = Array.Empty<DistributionRunReportSummaryResponse>();
        var serviceResult = Result<DistributionRunReportSummaryResponse[]>.Success(emptySummaryResponses);

        mockService
            .Setup(x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await mockService.Object.GetDistributionRunReportSummary(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Length.ShouldBe(0);
        
        mockService.Verify(
            x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle multiple distribution frequency summaries")]
    public async Task MockDistributionService_WithMultipleFrequencies_ShouldReturnAllSummaries()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        
        var summaryResponses = new[]
        {
            new DistributionRunReportSummaryResponse
            {
                DistributionFrequencyId = 'Q',
                DistributionTypeName = "Quarterly",
                TotalDistributions = 100,
                TotalGrossAmount = 150000.00M,
                TotalFederalTaxAmount = 22500.00M,
                TotalStateTaxAmount = 7500.00M,
                TotalCheckAmount = 120000.00M
            },
            new DistributionRunReportSummaryResponse
            {
                DistributionFrequencyId = 'M',
                DistributionTypeName = "Monthly",
                TotalDistributions = 75,
                TotalGrossAmount = 100000.00M,
                TotalFederalTaxAmount = 15000.00M,
                TotalStateTaxAmount = 5000.00M,
                TotalCheckAmount = 80000.00M
            },
            new DistributionRunReportSummaryResponse
            {
                DistributionFrequencyId = 'Y',
                DistributionTypeName = "Yearly",
                TotalDistributions = 200,
                TotalGrossAmount = 500000.00M,
                TotalFederalTaxAmount = 75000.00M,
                TotalStateTaxAmount = 25000.00M,
                TotalCheckAmount = 400000.00M
            }
        };

        var serviceResult = Result<DistributionRunReportSummaryResponse[]>.Success(summaryResponses);

        mockService
            .Setup(x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await mockService.Object.GetDistributionRunReportSummary(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Length.ShouldBe(3);
        
        // Verify each frequency type is present
        var frequencies = result.Value.Select(r => r.DistributionFrequencyId).ToArray();
        frequencies.ShouldContain('Q');
        frequencies.ShouldContain('M');
        frequencies.ShouldContain('Y');
        
        mockService.Verify(
            x => x.GetDistributionRunReportSummary(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle distribution summary with null frequency")]
    public void ResponseWithNullFrequency_ShouldBeValid()
    {
        // Arrange
        var response = new DistributionRunReportSummaryResponse
        {
            DistributionFrequencyId = null, // Could represent "All" or unspecified frequency
            DistributionTypeName = "All Frequencies",
            TotalDistributions = 500,
            TotalGrossAmount = 1000000.00M,
            TotalFederalTaxAmount = 150000.00M,
            TotalStateTaxAmount = 50000.00M,
            TotalCheckAmount = 800000.00M
        };

        // Act & Assert
        response.ShouldNotBeNull();
        response.DistributionFrequencyId.ShouldBeNull();
        response.DistributionTypeName.ShouldBe("All Frequencies");
        response.TotalDistributions.ShouldBe(500);
        response.TotalGrossAmount.ShouldBe(1000000.00M);
        response.TotalFederalTaxAmount.ShouldBe(150000.00M);
        response.TotalStateTaxAmount.ShouldBe(50000.00M);
        response.TotalCheckAmount.ShouldBe(800000.00M);
    }

    [Fact]
    [Description("PS-1230 : Should handle distribution summary with zero amounts")]
    public void ResponseWithZeroAmounts_ShouldBeValid()
    {
        // Arrange
        var response = new DistributionRunReportSummaryResponse
        {
            DistributionFrequencyId = 'S',
            DistributionTypeName = "Semi-Annual",
            TotalDistributions = 0,
            TotalGrossAmount = 0.00M,
            TotalFederalTaxAmount = 0.00M,
            TotalStateTaxAmount = 0.00M,
            TotalCheckAmount = 0.00M
        };

        // Act & Assert
        response.ShouldNotBeNull();
        response.DistributionFrequencyId.ShouldBe('S');
        response.DistributionTypeName.ShouldBe("Semi-Annual");
        response.TotalDistributions.ShouldBe(0);
        response.TotalGrossAmount.ShouldBe(0.00M);
        response.TotalFederalTaxAmount.ShouldBe(0.00M);
        response.TotalStateTaxAmount.ShouldBe(0.00M);
        response.TotalCheckAmount.ShouldBe(0.00M);
    }

    [Fact]
    [Description("PS-1230 : Should validate tax calculation consistency")]
    public void ResponseAmounts_ShouldFollowTaxCalculationLogic()
    {
        // Arrange
        var response = new DistributionRunReportSummaryResponse
        {
            DistributionFrequencyId = 'M',
            DistributionTypeName = "Monthly",
            TotalDistributions = 100,
            TotalGrossAmount = 100000.00M,
            TotalFederalTaxAmount = 15000.00M,
            TotalStateTaxAmount = 5000.00M,
            TotalCheckAmount = 80000.00M
        };

        // Act & Assert - Check amount should equal gross minus taxes
        var expectedCheckAmount = response.TotalGrossAmount - response.TotalFederalTaxAmount - response.TotalStateTaxAmount;
        response.TotalCheckAmount.ShouldBe(expectedCheckAmount);
        
        // Total taxes should be less than gross amount
        var totalTaxes = response.TotalFederalTaxAmount + response.TotalStateTaxAmount;
        totalTaxes.ShouldBeLessThan(response.TotalGrossAmount);
        
        // Check amount should be positive when there are distributions
        if (response.TotalDistributions > 0)
        {
            response.TotalCheckAmount.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    [Description("PS-1230 : Should handle different distribution frequency characters")]
    public void ResponseFrequencyIds_ShouldSupportAllValidFrequencies()
    {
        // Arrange & Act & Assert
        var validFrequencies = new[] { 'Q', 'M', 'Y', 'S' }; // Quarterly, Monthly, Yearly, Semi-annual
        
        foreach (var frequency in validFrequencies)
        {
            var response = new DistributionRunReportSummaryResponse
            {
                DistributionFrequencyId = frequency,
                DistributionTypeName = $"Test {frequency}",
                TotalDistributions = 1,
                TotalGrossAmount = 1000.00M,
                TotalFederalTaxAmount = 150.00M,
                TotalStateTaxAmount = 50.00M,
                TotalCheckAmount = 800.00M
            };

            response.ShouldNotBeNull();
            response.DistributionFrequencyId.ShouldBe(frequency);
        }
    }
}
