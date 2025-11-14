using System.ComponentModel;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Repositories;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Repositories;

/// <summary>
/// Unit tests for DemographicsRepository using in-memory database.
/// Tests all query and command operations with realistic test data.
///
/// NOTE: These tests use ScenarioDataContextFactory.Create() which is experimental.
/// See TESTING_LIMITATIONS.md for known issues and workarounds.
/// </summary>
[Description("PS-1721: Unit tests for DemographicsRepository data access layer")]
#pragma warning disable CS0618 // ScenarioDataContextFactory.Create() is obsolete but intentionally used here
public sealed class DemographicsRepositoryTests
{
    #region Query Operations Tests

    [Fact]
    [Description("PS-1721: GetByOracleIdsAsync returns matching demographics with includes")]
    public async Task GetByOracleIdsAsync_WithValidIds_ReturnsMatchingDemographics()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(10);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        var targetIds = demographics.Take(3).Select(d => d.OracleHcmId).ToList();

        // Act
        var results = await repository.GetByOracleIdsAsync(targetIds, default);

        // Assert
        results.Count.ShouldBe(3);
        results.All(r => targetIds.Contains(r.OracleHcmId)).ShouldBeTrue();
        results.All(r => r.ContactInfo != null).ShouldBeTrue();
        results.All(r => r.Address != null).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1721: GetByOracleIdsAsync with empty list returns empty result")]
    public async Task GetByOracleIdsAsync_WithEmptyList_ReturnsEmpty()
    {
        // Arrange
        var factory = ScenarioDataContextFactory.Create();
        var repository = new DemographicsRepository(factory);

        // Act
        var results = await repository.GetByOracleIdsAsync(Array.Empty<long>(), default);

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-1721: GetByOracleIdsAsync with non-existent IDs returns empty result")]
    public async Task GetByOracleIdsAsync_WithNonExistentIds_ReturnsEmpty()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(5);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        // Act
        var results = await repository.GetByOracleIdsAsync(new[] { 999999999L }, default);

        // Assert
        results.ShouldBeEmpty();
    }


    [Fact]
    [Description("PS-1721: GetBySsnAndBadgePairsAsync with all zero badges returns empty")]
    public async Task GetBySsnAndBadgePairsAsync_WithAllZeroBadges_ReturnsEmpty()
    {
        // Arrange
        var factory = ScenarioDataContextFactory.Create();
        var repository = new DemographicsRepository(factory);

        var allZeroPairs = new List<(int Ssn, int BadgeNumber)>
        {
            (111111111, 0),
            (222222222, 0),
            (333333333, 0)
        };

        // Act
        var results = await repository.GetBySsnAndBadgePairsAsync(allZeroPairs, default);

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-1721: GetBySsnAsync returns demographic when SSN exists")]
    public async Task GetBySsnAsync_WithExistingSsn_ReturnsDemographic()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(5);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);
        var targetSsn = demographics[2].Ssn;

        // Act
        var result = await repository.GetBySsnAsync(targetSsn, default);

        // Assert
        result.ShouldNotBeNull();
        result.Ssn.ShouldBe(targetSsn);
    }

    [Fact]
    [Description("PS-1721: GetBySsnAsync returns null when SSN does not exist")]
    public async Task GetBySsnAsync_WithNonExistentSsn_ReturnsNull()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(5);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        // Act
        var result = await repository.GetBySsnAsync(999999999, default);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    [Description("PS-1721: GetDuplicateSsnsAsync returns only demographics with duplicate SSNs")]
    public async Task GetDuplicateSsnsAsync_WithDuplicates_ReturnsOnlyDuplicates()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(10);

        // Create intentional duplicates
        demographics[1].Ssn = demographics[0].Ssn; // Duplicate
        demographics[2].Ssn = demographics[0].Ssn; // Another duplicate
        demographics[4].Ssn = demographics[3].Ssn; // Different duplicate pair

        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        var ssnsToCheck = demographics.Select(d => d.Ssn).Distinct().ToList();

        // Act
        var results = await repository.GetDuplicateSsnsAsync(ssnsToCheck, default);

        // Assert
        results.Count.ShouldBe(5); // 3 with first SSN + 2 with second SSN

        var groupedResults = results.GroupBy(d => d.Ssn).ToList();
        groupedResults.Count.ShouldBe(2); // Two different duplicate SSNs
        groupedResults.All(g => g.Count() > 1).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1721: GetDuplicateSsnsAsync with no duplicates returns empty")]
    public async Task GetDuplicateSsnsAsync_WithNoDuplicates_ReturnsEmpty()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(5);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        var uniqueSsns = demographics.Select(d => d.Ssn).ToList();

        // Act
        var results = await repository.GetDuplicateSsnsAsync(uniqueSsns, default);

        // Assert
        results.ShouldBeEmpty();
    }

    #endregion

}
