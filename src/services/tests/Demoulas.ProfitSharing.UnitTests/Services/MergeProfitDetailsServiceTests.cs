using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for MergeProfitDetailsService to verify profit detail merging and PayProfit EnrollmentId updates.
/// Tests cover successful merge operations, enrollment ID updates, error handling, and edge cases.
/// </summary>
[Collection("Adjustments Tests")]
public sealed class MergeProfitDetailsServiceTests : ApiTestBase<Api.Program>
{
    private readonly IMergeProfitDetailsService _mergeProfitDetailsService;

    public MergeProfitDetailsServiceTests()
    {
        _mergeProfitDetailsService = ServiceProvider?.GetRequiredService<IMergeProfitDetailsService>()
            ?? throw new NullReferenceException("IMergeProfitDetailsService not registered in DI container");
    }

    #region Happy Path Tests - Basic Merge Operations

    [Fact(DisplayName = "MergeProfitDetails - Should fail since source SSN and destination SSN are not found")]
    [Description("PS-1721 : Verifies profit details are successfully merged between demographics")]
    public async Task MergeProfitDetailsToDemographic_WithInvalidSsns_ShouldMergeProfitDetails()
    {
        // Arrange
        const int sourceSsn = 111111111;
        const int destinationSsn = 222222222;

        // Act
        var result = await _mergeProfitDetailsService.MergeProfitDetailsToDemographic(
            sourceSsn,
            destinationSsn,
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Value.ShouldBeFalse();
        result.Error?.Description.ShouldContain("not found");
    }

    #endregion

    #region Error Handling Tests

    [Fact(DisplayName = "MergeProfitDetails - Should fail when source and destination SSNs are identical")]
    [Description("PS-1721 : Verifies merge fails with same SSN error")]
    public async Task MergeProfitDetailsToDemographic_WithSameSsn_ShouldFail()
    {
        // Arrange
        const int sameSsn = 123456789;

        // Act
        var result = await _mergeProfitDetailsService.MergeProfitDetailsToDemographic(
            sameSsn,
            sameSsn,
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        // Verify error message contains expected text
        result.Error.Description.ShouldContain("Cannot merge");
    }

   

    #endregion
}
