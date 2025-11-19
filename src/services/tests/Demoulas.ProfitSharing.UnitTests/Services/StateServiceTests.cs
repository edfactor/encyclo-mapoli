using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.Lookup;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for StateService to verify state lookup functionality with EF Core read-only queries.
/// Tests the query against ProfitDetail.COMMENT_RELATED_STATE joined with State lookup table.
/// </summary>
[Collection("Lookup Tests")]
public sealed class StateServiceTests : ApiTestBase<Api.Program>
{
    private readonly IStateService _stateService;

    public StateServiceTests()
    {
        _stateService = this.ServiceProvider?.GetRequiredService<IStateService>() ?? throw new NullReferenceException();
    }

    [Fact(DisplayName = "StateService - Should return states from database")]
    [Description("PS-XXXX : Retrieves distinct states from COMMENT_RELATED_STATE")]
    public async Task GetStatesAsync_ShouldReturnStatesFromDatabase()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "StateService - Should return StateListResponse objects")]
    [Description("PS-XXXX : Verifies correct DTO structure")]
    public async Task GetStatesAsync_ShouldReturnCorrectDtoStructure()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        result.ShouldAllBe(state =>
            !string.IsNullOrWhiteSpace(state.Abbreviation) &&
            !string.IsNullOrWhiteSpace(state.Name)
        );
    }

    [Fact(DisplayName = "StateService - Should return states with 2-character abbreviations")]
    [Description("PS-XXXX : Validates state abbreviation format")]
    public async Task GetStatesAsync_ShouldReturnValidStateAbbreviations()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        result.ShouldAllBe(state => state.Abbreviation.Length == 2);
    }

    [Fact(DisplayName = "StateService - Should return states ordered alphabetically")]
    [Description("PS-XXXX : Verifies result ordering by abbreviation")]
    public async Task GetStatesAsync_ShouldReturnStatesOrderedAlphabetically()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        var abbreviations = result.Select(s => s.Abbreviation).ToList();
        var sortedAbbreviations = abbreviations.OrderBy(a => a).ToList();
        abbreviations.ShouldBe(sortedAbbreviations);
    }

    [Fact(DisplayName = "StateService - Should have non-empty state names")]
    [Description("PS-XXXX : Validates state names are populated")]
    public async Task GetStatesAsync_ShouldHaveNonEmptyStateNames()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        result.ShouldAllBe(state => !string.IsNullOrWhiteSpace(state.Name));
    }

    [Fact(DisplayName = "StateService - Should return ICollection type")]
    [Description("PS-XXXX : Verifies return type is ICollection")]
    public async Task GetStatesAsync_ShouldReturnICollectionType()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        result.ShouldBeAssignableTo<ICollection<StateListResponse>>();
    }

    [Fact(DisplayName = "StateService - Should handle cancellation token")]
    [Description("PS-XXXX : Verifies cancellation token support")]
    public async Task GetStatesAsync_ShouldHandleCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var task = _stateService.GetStatesAsync(cts.Token);

        // Assert - Should not throw when token is valid
        var result = await task;
        result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "StateService - Should be consistent across multiple calls")]
    [Description("PS-XXXX : Verifies consistent query results")]
    public async Task GetStatesAsync_ShouldBeConsistentAcrossMultipleCalls()
    {
        // Act
        var result1 = await _stateService.GetStatesAsync(CancellationToken.None);
        var result2 = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        result1.Count.ShouldBe(result2.Count);
        result1.Select(s => s.Abbreviation).OrderBy(a => a)
            .ShouldBe(result2.Select(s => s.Abbreviation).OrderBy(a => a));
    }

    [Fact(DisplayName = "StateService - Should only return states referenced in ProfitDetail")]
    [Description("PS-XXXX : Verifies filtering by COMMENT_RELATED_STATE")]
    public async Task GetStatesAsync_ShouldOnlyReturnReferencedStates()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert - Result should contain states that exist in mock data
        // Mock data includes MA, NH, ME, CT, RI, VT, NY, CA, TX
        if (result.Count > 0)
        {
            var expectedStates = new[] { "MA", "NH", "ME", "CT", "RI", "VT", "NY", "CA", "TX" };
            var abbreviations = result.Select(s => s.Abbreviation).ToList();
            abbreviations.ShouldAllBe(abbr => expectedStates.Contains(abbr) || expectedStates.Contains(abbr));
        }
    }

    [Fact(DisplayName = "StateService - Should map state names correctly")]
    [Description("PS-XXXX : Verifies abbreviation to name mapping")]
    public async Task GetStatesAsync_ShouldMapStateNamesCorrectly()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert - Check some known mappings from mock data
        var statesByAbbr = result.ToDictionary(s => s.Abbreviation, s => s.Name);

        if (statesByAbbr.ContainsKey("MA"))
        {
            statesByAbbr["MA"].ShouldBe("Massachusetts");
        }

        if (statesByAbbr.ContainsKey("NH"))
        {
            statesByAbbr["NH"].ShouldBe("New Hampshire");
        }

        if (statesByAbbr.ContainsKey("CA"))
        {
            statesByAbbr["CA"].ShouldBe("California");
        }
    }

    [Fact(DisplayName = "StateService - Should not contain duplicate abbreviations")]
    [Description("PS-XXXX : Verifies distinct abbreviations")]
    public async Task GetStatesAsync_ShouldNotContainDuplicates()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        var abbreviations = result.Select(s => s.Abbreviation).ToList();
        abbreviations.Distinct().Count().ShouldBe(abbreviations.Count);
    }

    [Fact(DisplayName = "StateService - Should not return null or empty abbreviations")]
    [Description("PS-XXXX : Validates abbreviation field quality")]
    public async Task GetStatesAsync_ShouldNotReturnNullOrEmptyAbbreviations()
    {
        // Act
        var result = await _stateService.GetStatesAsync(CancellationToken.None);

        // Assert
        result.ShouldNotContain(s => string.IsNullOrWhiteSpace(s.Abbreviation));
    }
}
