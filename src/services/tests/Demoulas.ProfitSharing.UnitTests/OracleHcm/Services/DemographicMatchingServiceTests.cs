using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Repositories;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.ComponentModel;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Services;

public class DemographicMatchingServiceTests
{
    private readonly Mock<ILogger<DemographicMatchingService>> _loggerMock;

    public DemographicMatchingServiceTests()
    {
        _loggerMock = new Mock<ILogger<DemographicMatchingService>>();
        TestDataBuilder.Reset();
    }

    [Fact]
    [Description("PS-XXXX : Primary match by OracleHcmId returns expected demographics")]
    public async Task MatchByOracleIdAsync_WithValidOracleHcmIds_ReturnsMatchedDemographics()
    {
        // Arrange
        var existing1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        var existing2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002);
        var existing3 = TestDataBuilder.CreateDemographic(oracleHcmId: 100003);

        var factory = ScenarioDataContextFactory.Create(
            demographics: new List<Demographic> { existing1, existing2, existing3 }
        );

        var repository = new DemographicsRepository(factory);
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        var incoming1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        var incoming2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002);
        var incoming4 = TestDataBuilder.CreateDemographic(oracleHcmId: 100004); // No match

        var incomingByOracleId = new Dictionary<long, Demographic>
        {
            { 100001, incoming1 },
            { 100002, incoming2 },
            { 100004, incoming4 }
        };

        // Act
        var result = await service.MatchByOracleIdAsync(incomingByOracleId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldContain(d => d.OracleHcmId == 100001);
        result.ShouldContain(d => d.OracleHcmId == 100002);
    }

    [Fact]
    [Description("PS-XXXX : Primary match with empty incoming dictionary returns empty list")]
    public async Task MatchByOracleIdAsync_WithEmptyIncomingDictionary_ReturnsEmptyList()
    {
        // Arrange
        var existing = TestDataBuilder.CreateDemographic();
        var factory = ScenarioDataContextFactory.Create(
            demographics: new List<Demographic> { existing }
        );

        var repository = new DemographicsRepository(factory);
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        // Act
        var result = await service.MatchByOracleIdAsync(new Dictionary<long, Demographic>(), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Primary match with no matches returns empty list")]
    public async Task MatchByOracleIdAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var existing = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        var factory = ScenarioDataContextFactory.Create(
            demographics: new List<Demographic> { existing }
        );

        var repository = new DemographicsRepository(factory);
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        var incoming = TestDataBuilder.CreateDemographic(oracleHcmId: 999999); // Different ID

        var incomingByOracleId = new Dictionary<long, Demographic>
        {
            { 999999, incoming }
        };

        // Act
        var result = await service.MatchByOracleIdAsync(incomingByOracleId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Fallback match by SSN and Badge returns matching demographics")]
    public async Task MatchByFallbackAsync_WithValidSsnBadgePairs_ReturnsMatchedDemographics()
    {
        // Arrange
        var existing1 = TestDataBuilder.CreateDemographic(ssn: 123456789, badgeNumber: 1001);
        var existing2 = TestDataBuilder.CreateDemographic(ssn: 123456790, badgeNumber: 1002);

        var factory = ScenarioDataContextFactory.Create(
            demographics: new List<Demographic> { existing1, existing2 }
        );

        var repository = new DemographicsRepository(factory);
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        var pairs = new List<(int Ssn, int Badge)>
        {
            (123456789, 1001),
            (123456790, 1002),
            (999999999, 9999) // No match
        };

        // Act
        var (matched, skippedAllZero) = await service.MatchByFallbackAsync(pairs, CancellationToken.None);

        // Assert
        matched.ShouldNotBeNull();
        matched.Count.ShouldBe(2);
        matched.ShouldContain(d => d.Ssn == 123456789 && d.BadgeNumber == 1001);
        matched.ShouldContain(d => d.Ssn == 123456790 && d.BadgeNumber == 1002);
        skippedAllZero.ShouldBeFalse();
    }

    [Fact]
    [Description("PS-XXXX : Fallback match with all zero badge numbers skips query")]
    public async Task MatchByFallbackAsync_WithAllZeroBadgeNumbers_SkipsQueryAndReturnsFlag()
    {
        // Arrange
        var existing = TestDataBuilder.CreateDemographic(ssn: 123456789, badgeNumber: 1001);

        var factory = ScenarioDataContextFactory.Create(
            demographics: new List<Demographic> { existing }
        );

        var repository = new DemographicsRepository(factory);
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        var pairs = new List<(int Ssn, int Badge)>
        {
            (123456789, 0),     // All zero badges
            (123456790, 0),
            (123456791, 0)
        };

        // Act
        var (matched, skippedAllZero) = await service.MatchByFallbackAsync(pairs, CancellationToken.None);

        // Assert
        matched.ShouldNotBeNull();
        matched.ShouldBeEmpty();
        skippedAllZero.ShouldBeTrue(); // Flag indicates all-zero condition
    }

    [Fact]
    [Description("PS-XXXX : Fallback match with empty pairs returns empty list")]
    public async Task MatchByFallbackAsync_WithEmptyPairs_ReturnsEmptyList()
    {
        // Arrange
        var existing = TestDataBuilder.CreateDemographic();
        var factory = ScenarioDataContextFactory.Create(
            demographics: new List<Demographic> { existing }
        );

        var repository = new DemographicsRepository(factory);
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        // Act
        var (matched, skippedAllZero) = await service.MatchByFallbackAsync(new List<(int Ssn, int Badge)>(), CancellationToken.None);

        // Assert
        matched.ShouldNotBeNull();
        matched.ShouldBeEmpty();
        skippedAllZero.ShouldBeFalse();
    }

    [Fact]
    [Description("PS-XXXX : Identify new demographics returns items not in existing list")]
    public void IdentifyNewDemographics_WithMixedMatches_ReturnsOnlyNewItems()
    {
        // Arrange
        var repository = new DemographicsRepository(ScenarioDataContextFactory.Create());
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        var incoming1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789, badgeNumber: 1001);
        var incoming2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790, badgeNumber: 1002);
        var incoming3 = TestDataBuilder.CreateDemographic(oracleHcmId: 100003, ssn: 123456791, badgeNumber: 1003);
        var incoming4 = TestDataBuilder.CreateDemographic(oracleHcmId: 100004, ssn: 123456792, badgeNumber: 1004);

        var incomingList = new List<Demographic> { incoming1, incoming2, incoming3, incoming4 };

        var existing1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789, badgeNumber: 1001);
        var existing2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790, badgeNumber: 1002);

        var existingList = new List<Demographic> { existing1, existing2 };

        // Act
        var result = service.IdentifyNewDemographics(incomingList, existingList);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // incoming3 and incoming4 should be new
        result.ShouldContain(d => d.OracleHcmId == 100003);
        result.ShouldContain(d => d.OracleHcmId == 100004);
    }

    [Fact]
    [Description("PS-XXXX : Identify new demographics with all matched returns empty list")]
    public void IdentifyNewDemographics_WithAllMatched_ReturnsEmptyList()
    {
        // Arrange
        var repository = new DemographicsRepository(ScenarioDataContextFactory.Create());
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        var incoming1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789, badgeNumber: 1001);
        var incoming2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790, badgeNumber: 1002);

        var incomingList = new List<Demographic> { incoming1, incoming2 };

        var existing1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789, badgeNumber: 1001);
        var existing2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790, badgeNumber: 1002);

        var existingList = new List<Demographic> { existing1, existing2 };

        // Act
        var result = service.IdentifyNewDemographics(incomingList, existingList);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Identify new demographics with no matches returns all items")]
    public void IdentifyNewDemographics_WithNoMatches_ReturnsAllItems()
    {
        // Arrange
        var repository = new DemographicsRepository(ScenarioDataContextFactory.Create());
        var service = new DemographicMatchingService(repository, _loggerMock.Object);

        var incoming1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        var incoming2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002);
        var incoming3 = TestDataBuilder.CreateDemographic(oracleHcmId: 100003);

        var incomingList = new List<Demographic> { incoming1, incoming2, incoming3 };

        // Act
        var result = service.IdentifyNewDemographics(incomingList, new List<Demographic>());

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.ShouldContain(incoming1);
        result.ShouldContain(incoming2);
        result.ShouldContain(incoming3);
    }
}
