using System.ComponentModel;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Repositories;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Demoulas.ProfitSharing.UnitTests.Common.Helpers;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Data.Repositories;

/// <summary>
/// Unit tests for DemographicsRepository using in-memory database.
/// Tests all query and command operations with realistic test data.
/// </summary>
[Description("PS-1721: Unit tests for DemographicsRepository data access layer")]
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
    [Description("PS-1721: GetBySsnAndBadgePairsAsync returns matching demographics")]
    public async Task GetBySsnAndBadgePairsAsync_WithValidPairs_ReturnsMatches()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(10);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        var targetPairs = demographics.Take(3)
            .Select(d => (d.Ssn, d.BadgeNumber))
            .ToList();

        // Act
        var results = await repository.GetBySsnAndBadgePairsAsync(targetPairs, default);

        // Assert
        results.Count.ShouldBe(3);
        results.All(r => r.ContactInfo != null).ShouldBeTrue();
        results.All(r => r.Address != null).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-1721: GetBySsnAndBadgePairsAsync filters out zero badge numbers")]
    public async Task GetBySsnAndBadgePairsAsync_WithZeroBadges_FiltersThemOut()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(5);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        var pairsWithZeros = new List<(int Ssn, int BadgeNumber)>
        {
            (111111111, 0),
            (222222222, 0),
            (demographics[0].Ssn, demographics[0].BadgeNumber) // Valid pair
        };

        // Act
        var results = await repository.GetBySsnAndBadgePairsAsync(pairsWithZeros, default);

        // Assert
        // Should only return the one valid pair (zero badges filtered out)
        results.Count.ShouldBe(1);
        results[0].Ssn.ShouldBe(demographics[0].Ssn);
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

    #region Command Operations Tests

    [Fact]
    [Description("PS-1721: AddAsync adds new demographic to context")]
    public async Task AddAsync_WithValidDemographic_AddsToContext()
    {
        // Arrange
        var factory = ScenarioDataContextFactory.Create();
        var repository = new DemographicsRepository(factory);
        var newDemographic = new DemographicFaker().Generate();

        // Act
        await repository.AddAsync(newDemographic, default);

        // Assert
        await factory.UseReadOnlyContext(async ctx =>
        {
            var saved = await ctx.Demographics.FindAsync(newDemographic.Id);
            saved.ShouldNotBeNull();
            saved.Ssn.ShouldBe(newDemographic.Ssn);
        }, default);
    }

    [Fact]
    [Description("PS-1721: AddRangeAsync adds multiple demographics to context")]
    public async Task AddRangeAsync_WithMultipleDemographics_AddsAll()
    {
        // Arrange
        var factory = ScenarioDataContextFactory.Create();
        var repository = new DemographicsRepository(factory);
        var newDemographics = new DemographicFaker().Generate(5);

        // Act
        await repository.AddRangeAsync(newDemographics, default);

        // Assert
        await factory.UseReadOnlyContext(async ctx =>
        {
            var count = ctx.Demographics.Count();
            count.ShouldBe(5);
        }, default);
    }

    [Fact]
    [Description("PS-1721: Update modifies tracked demographic")]
    public async Task Update_WithTrackedDemographic_ModifiesEntity()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(1);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);
        var original = demographics[0];

        // Act - Use ExecuteWithChangesAsync for update operations
        await repository.ExecuteWithChangesAsync(async ctx =>
        {
            var tracked = await ctx.Demographics.FindAsync(original.Id);
            tracked!.StoreNumber = 999;
            return await ctx.SaveChangesAsync(default);
        }, default);

        // Assert
        await factory.UseReadOnlyContext(async ctx =>
        {
            var updated = await ctx.Demographics.FindAsync(original.Id);
            updated!.StoreNumber.ShouldBe((short)999);
        }, default);
    }

    #endregion

    #region Related Entity Operations Tests

    [Fact]
    [Description("PS-1721: UpdateRelatedSsnAsync updates BeneficiaryContacts SSN references")]
    public async Task UpdateRelatedSsnAsync_UpdatesBeneficiaryContactsSsn()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(1);
        var beneficiaryContacts = new List<BeneficiaryContact>
        {
            new() { Id = 1, Ssn = demographics[0].Ssn, BadgeNumber = 12345, PsnSuffix = 1 }
        };

        var factory = ScenarioDataContextFactory.Create(
            demographics,
            beneficiaryContacts: beneficiaryContacts);

        var repository = new DemographicsRepository(factory);
        const int newSsn = 999999999;

        // Act - UpdateRelatedSsnAsync now handles its own transaction
        await repository.UpdateRelatedSsnAsync(demographics[0].Ssn, newSsn, default);

        // Assert
        await factory.UseReadOnlyContext(async ctx =>
        {
            var updated = await ctx.BeneficiaryContacts.FindAsync(1);
            updated!.Ssn.ShouldBe(newSsn);
        }, default);
    }

    [Fact]
    [Description("PS-1721: UpdateRelatedSsnAsync updates ProfitDetails SSN references")]
    public async Task UpdateRelatedSsnAsync_UpdatesProfitDetailsSsn()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(1);
        var profitDetails = new List<ProfitDetail>
        {
            new()
            {
                Id = 1,
                Ssn = demographics[0].Ssn,
                ProfitYear = 2025,
                ProfitCodeId = "REG",
                PayPeriodEndDate = DateOnly.FromDateTime(DateTime.Today)
            }
        };

        var factory = ScenarioDataContextFactory.Create(
            demographics,
            profitDetails: profitDetails);

        var repository = new DemographicsRepository(factory);
        const int newSsn = 888888888;

        // Act - UpdateRelatedSsnAsync now handles its own transaction
        await repository.UpdateRelatedSsnAsync(demographics[0].Ssn, newSsn, default);

        // Assert
        await factory.UseReadOnlyContext(async ctx =>
        {
            var updated = await ctx.ProfitDetails.FindAsync(1L);
            updated!.Ssn.ShouldBe(newSsn);
        }, default);
    }

    #endregion

    #region Transaction Management Tests

    [Fact]
    [Description("PS-1721: ExecuteWithChangesAsync commits pending changes")]
    public async Task ExecuteWithChangesAsync_CommitsPendingChanges()
    {
        // Arrange
        var factory = ScenarioDataContextFactory.Create();
        var repository = new DemographicsRepository(factory);
        var newDemographic = new DemographicFaker().Generate();

        // Act
        var savedCount = await repository.ExecuteWithChangesAsync(async ctx =>
        {
            await ctx.Demographics.AddAsync(newDemographic, default);
            return await ctx.SaveChangesAsync(default);
        }, default);

        // Assert
        savedCount.ShouldBeGreaterThan(0);

        await factory.UseReadOnlyContext(async ctx =>
        {
            var exists = await ctx.Demographics.FindAsync(newDemographic.Id);
            exists.ShouldNotBeNull();
        }, default);
    }

    [Fact]
    [Description("PS-1721: ExecuteWithChangesAsync without changes returns zero")]
    public async Task ExecuteWithChangesAsync_WithoutChanges_ReturnsZero()
    {
        // Arrange
        var demographics = new DemographicFaker().Generate(5);
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);

        // Act
        var savedCount = await repository.ExecuteWithChangesAsync(async ctx =>
        {
            // No changes made
            return await ctx.SaveChangesAsync(default);
        }, default);

        // Assert
        savedCount.ShouldBe(0);
    }

    #endregion
}
