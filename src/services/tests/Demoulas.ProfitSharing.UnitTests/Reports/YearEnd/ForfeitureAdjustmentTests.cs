using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Api;
using FastEndpoints;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ForfeitureAdjustmentTests : ApiTestBase<Program>
{
    public ForfeitureAdjustmentTests()
    {
    }

    private readonly int testBadge = 700310;
    private readonly short testYear = 2024;
    private readonly string testSSN = "700000351";

    [Fact(DisplayName = "Get Forfeiture Adjustments - Success")]
    public async Task GetForfeitureAdjustmentsSuccessTest()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new ForfeitureAdjustmentRequest
        {
            Badge = testBadge.ToString(),
            ProfitYear = testYear
        };

        // Act
        var response = await ApiClient.GETAsync<GetForfeitureAdjustmentsEndpoint,
            ForfeitureAdjustmentRequest, ForfeitureAdjustmentReportResponse>(request);

        // Assert
        response.Should().NotBeNull();
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Result.ReportName.Should().Be("FORFEITURE ADJUSTMENT REPORT");
        response.Result.Response.Should().NotBeNull();
    }

    [Fact(DisplayName = "Get Forfeiture Adjustments - Filter By SSN")]
    public async Task GetForfeitureAdjustmentsBySSNTest()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new ForfeitureAdjustmentRequest
        {
            SSN = testSSN,
            ProfitYear = testYear
        };

        // Act
        var response = await ApiClient.GETAsync<GetForfeitureAdjustmentsEndpoint,
            ForfeitureAdjustmentRequest, ForfeitureAdjustmentReportResponse>(request);

        // Assert
        response.Should().NotBeNull();
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Result.Should().NotBeNull();
    }

    [Fact(DisplayName = "Get Forfeiture Adjustments - Missing Required Role")]
    public async Task GetForfeitureAdjustmentsWithoutPermission()
    {
        // Arrange
        var request = new ForfeitureAdjustmentRequest
        {
            Badge = testBadge.ToString(),
            ProfitYear = testYear
        };

        // Act
        var response = await ApiClient.GETAsync<GetForfeitureAdjustmentsEndpoint,
            ForfeitureAdjustmentRequest, ForfeitureAdjustmentReportResponse>(request);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}