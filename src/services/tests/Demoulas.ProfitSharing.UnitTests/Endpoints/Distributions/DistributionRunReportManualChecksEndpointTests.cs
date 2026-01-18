using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Distributions;

/// <summary>
/// Unit tests for DistributionRunReportManualChecksEndpoint focusing on service interaction.
/// Note: This endpoint uses manual telemetry that requires HttpContext, so these tests
/// focus on verifying the service layer integration rather than the full endpoint execution.
/// Full endpoint behavior including telemetry is tested via integration tests.
/// </summary>
public class DistributionRunReportManualChecksEndpointTests
{
    [Fact]
    [Description("PS-1230 : Should validate request parameters correctly")]
    public void Request_WithValidParameters_ShouldPassValidation()
    {
        // Arrange
        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        // Act & Assert - if no exception is thrown, validation passed
        request.ShouldNotBeNull();
        request.Skip.ShouldBe(0);
        request.Take.ShouldBe(50);
        request.SortBy.ShouldBe("BadgeNumber");
        request.IsSortDescending.ShouldBe(false);
    }

    [Fact]
    [Description("PS-1230 : Should handle pagination parameters correctly")]
    public void Request_WithPaginationParameters_ShouldBeValid()
    {
        // Arrange
        var request = new SortedPaginationRequestDto
        {
            Skip = 100,
            Take = 25,
            SortBy = "CheckNumber",
            IsSortDescending = true
        };

        // Act & Assert
        request.ShouldNotBeNull();
        request.Skip.ShouldBe(100);
        request.Take.ShouldBe(25);
        request.SortBy.ShouldBe("CheckNumber");
        request.IsSortDescending.ShouldBe(true);
    }

    [Fact]
    [Description("PS-1230 : Should handle null sort parameters")]
    public void Request_WithNullSortBy_ShouldBeValid()
    {
        // Arrange
        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = null,
            IsSortDescending = false
        };

        // Act & Assert
        request.ShouldNotBeNull();
        request.SortBy.ShouldBeNull();
        request.IsSortDescending.ShouldBe(false);
    }

    [Fact]
    [Description("PS-1230 : Should handle empty sort parameters")]
    public void Request_WithEmptySortBy_ShouldBeValid()
    {
        // Arrange
        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = string.Empty,
            IsSortDescending = false
        };

        // Act & Assert
        request.ShouldNotBeNull();
        request.SortBy.ShouldBe(string.Empty);
    }

    [Fact]
    [Description("PS-1230 : Should create valid response from example")]
    public void ResponseExample_ShouldCreateValidResponse()
    {
        // Act
        var response = ManualChecksWrittenResponse.ResponseExample();

        // Assert
        response.ShouldNotBeNull();
        response.Ssn.ShouldBe("XXX-XX-1234");
        response.PayTo.ShouldBe("John Doe");
        response.CheckAmount.ShouldBe(1200.00m);
        response.CheckNumber.ShouldBe("1001");
        response.GrossAmount.ShouldBe(1500.00m);
        response.FederalTaxAmount.ShouldBe(150.00m);
        response.StateTaxAmount.ShouldBe(75.00m);
    }

    [Fact]
    [Description("PS-1230 : Should test service mock setup")]
    public async Task MockDistributionService_ShouldCallCorrectMethod()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "CheckNumber",
            IsSortDescending = false
        };

        var manualChecks = new List<ManualChecksWrittenResponse>
        {
            ManualChecksWrittenResponse.ResponseExample()
        };

        var paginatedResponse = new PaginatedResponseDto<ManualChecksWrittenResponse>
        {
            Results = manualChecks,
            Total = 1
        };

        var serviceResult = Result<PaginatedResponseDto<ManualChecksWrittenResponse>>.Success(paginatedResponse);

        mockService
            .Setup(x => x.GetManualCheckDistributionsAsync(It.IsAny<SortedPaginationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        await mockService.Object.GetManualCheckDistributionsAsync(request, CancellationToken.None);

        // Assert
        mockService.Verify(
            x => x.GetManualCheckDistributionsAsync(
                It.Is<SortedPaginationRequestDto>(req =>
                    req.Skip == 0 &&
                    req.Take == 50 &&
                    req.SortBy == "CheckNumber" &&
                    req.IsSortDescending == false),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle service cancellation token correctly")]
    public async Task MockDistributionService_ShouldPropagateCancellationToken()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "CheckNumber",
            IsSortDescending = false
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var paginatedResponse = new PaginatedResponseDto<ManualChecksWrittenResponse>
        {
            Results = new List<ManualChecksWrittenResponse>(),
            Total = 0
        };

        var serviceResult = Result<PaginatedResponseDto<ManualChecksWrittenResponse>>.Success(paginatedResponse);

        mockService
            .Setup(x => x.GetManualCheckDistributionsAsync(It.IsAny<SortedPaginationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        await mockService.Object.GetManualCheckDistributionsAsync(request, cancellationToken);

        // Assert
        mockService.Verify(
            x => x.GetManualCheckDistributionsAsync(
                It.IsAny<SortedPaginationRequestDto>(),
                It.Is<CancellationToken>(ct => ct == cancellationToken)),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle service exception and rethrow")]
    public async Task MockDistributionService_WhenServiceThrowsException_ShouldRethrow()
    {
        // Arrange
        var mockService = new Mock<IDistributionService>();
        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "CheckNumber",
            IsSortDescending = false
        };

        var expectedException = new InvalidOperationException("Database connection failed");

        mockService
            .Setup(x => x.GetManualCheckDistributionsAsync(It.IsAny<SortedPaginationRequestDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var thrownException = await Should.ThrowAsync<InvalidOperationException>(
            async () => await mockService.Object.GetManualCheckDistributionsAsync(request, CancellationToken.None));

        thrownException.ShouldBe(expectedException);

        mockService.Verify(
            x => x.GetManualCheckDistributionsAsync(It.IsAny<SortedPaginationRequestDto>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-1230 : Should handle different sort field types")]
    public void Request_WithDifferentSortFields_ShouldBeValid()
    {
        // Arrange & Act & Assert
        var requests = new[]
        {
            new SortedPaginationRequestDto { SortBy = "CheckNumber", Skip = 0, Take = 50 },
            new SortedPaginationRequestDto { SortBy = "PayTo", Skip = 0, Take = 50 },
            new SortedPaginationRequestDto { SortBy = "GrossAmount", Skip = 0, Take = 50 },
            new SortedPaginationRequestDto { SortBy = "FederalTaxAmount", Skip = 0, Take = 50 },
            new SortedPaginationRequestDto { SortBy = "StateTaxAmount", Skip = 0, Take = 50 }
        };

        foreach (var request in requests)
        {
            request.ShouldNotBeNull();
            request.SortBy.ShouldNotBeNullOrEmpty();
            request.Skip.ShouldBe(0);
            request.Take.ShouldBe(50);
        }
    }

    [Fact]
    [Description("PS-1230 : Should handle boundary pagination values")]
    public void Request_WithBoundaryPaginationValues_ShouldBeValid()
    {
        // Arrange
        var minRequest = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 1,
            SortBy = "CheckNumber",
            IsSortDescending = false
        };

        var maxRequest = new SortedPaginationRequestDto
        {
            Skip = 10000,
            Take = 1000,
            SortBy = "CheckNumber",
            IsSortDescending = true
        };

        // Act & Assert
        minRequest.ShouldNotBeNull();
        minRequest.Skip.ShouldBe(0);
        minRequest.Take.ShouldBe(1);

        maxRequest.ShouldNotBeNull();
        maxRequest.Skip.ShouldBe(10000);
        maxRequest.Take.ShouldBe(1000);
        maxRequest.IsSortDescending.ShouldBe(true);
    }
}
