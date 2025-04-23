using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.UnitTests;

public class ForfeitureAdjustmentServiceTests : ApiTestBase<Program>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ForfeitureAdjustmentService _forfeitureAdjustmentService;
    private readonly Mock<ITotalService> _mockTotalService;
    private const short testYear = 2024;

    public ForfeitureAdjustmentServiceTests()
    {
        _dataContextFactory = MockDbContextFactory;
        _mockTotalService = new Mock<ITotalService>();
        _forfeitureAdjustmentService = new ForfeitureAdjustmentService(_dataContextFactory, _mockTotalService.Object);
    }

    [Fact(DisplayName = "GetForfeitureAdjustmentReportAsync returns empty response when employee not found")]
    public async Task GetForfeitureAdjustmentReportAsync_WhenEmployeeNotFound_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new ForfeitureAdjustmentRequest
        {
            SSN = "999999999",
            ProfitYear = testYear
        };

        // Act
        var result = await _forfeitureAdjustmentService.GetForfeitureAdjustmentReportAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Response.Results.Should().BeEmpty();
        result.Response.Total.Should().Be(0);
        result.TotatNetBalance.Should().Be(0);
        result.TotatNetVested.Should().Be(0);
    }

    [Fact(DisplayName = "GetForfeitureAdjustmentReportAsync returns expected report format")]
    public async Task GetForfeitureAdjustmentReportAsync_WhenEmployeeFound_ReturnsCorrectReport()
    {
        const int testSsn = 700000351;

        var request = new ForfeitureAdjustmentRequest
        {
            SSN = testSsn.ToString(),
            ProfitYear = testYear
        };

        // Act
        var result = await _forfeitureAdjustmentService.GetForfeitureAdjustmentReportAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.ReportName.Should().Be("FORFEITURE ADJUSTMENT REPORT");
        result.Response.Should().NotBeNull();
    }

}