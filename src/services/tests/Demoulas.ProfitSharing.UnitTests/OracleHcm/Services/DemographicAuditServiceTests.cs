using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.ComponentModel;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Services;

public class DemographicAuditServiceTests
{
    private readonly Mock<ILogger<DemographicAuditService>> _loggerMock;

    public DemographicAuditServiceTests()
    {
        _loggerMock = new Mock<ILogger<DemographicAuditService>>();
        TestDataBuilder.Reset();
    }

    [Fact]
    [Description("PS-XXXX : Detect duplicate SSNs returns grouped demographics")]
    public void DetectDuplicateSsns_WithDuplicates_ReturnsGroupedBySSN()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        var demo1 = TestDataBuilder.CreateDemographic(ssn: 123456789);
        var demo2 = TestDataBuilder.CreateDemographic(ssn: 123456789); // Duplicate SSN
        var demo3 = TestDataBuilder.CreateDemographic(ssn: 123456790); // Unique SSN
        var demo4 = TestDataBuilder.CreateDemographic(ssn: 123456791);
        var demo5 = TestDataBuilder.CreateDemographic(ssn: 123456791); // Another duplicate

        var demographics = new List<Demographic> { demo1, demo2, demo3, demo4, demo5 };

        // Act
        var result = service.DetectDuplicateSsns(demographics);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // Two groups with duplicates

        var group1 = result.FirstOrDefault(g => g.Key == 123456789);
        group1.ShouldNotBeNull();
        group1.Count().ShouldBe(2);

        var group2 = result.FirstOrDefault(g => g.Key == 123456791);
        group2.ShouldNotBeNull();
        group2.Count().ShouldBe(2);
    }

    [Fact]
    [Description("PS-XXXX : Detect duplicate SSNs with no duplicates returns empty list")]
    public void DetectDuplicateSsns_WithNoDuplicates_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        var demo1 = TestDataBuilder.CreateDemographic(ssn: 123456789);
        var demo2 = TestDataBuilder.CreateDemographic(ssn: 123456790);
        var demo3 = TestDataBuilder.CreateDemographic(ssn: 123456791);

        var demographics = new List<Demographic> { demo1, demo2, demo3 };

        // Act
        var result = service.DetectDuplicateSsns(demographics);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Detect duplicate SSNs with empty list returns empty list")]
    public void DetectDuplicateSsns_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        // Act
        var result = service.DetectDuplicateSsns(new List<Demographic>());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Prepare audit for duplicate SSNs creates commands for each duplicate group")]
    public void PrepareAuditDuplicateSsns_WithDuplicates_CreatesAuditCommands()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        var demo1 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 100001, badgeNumber: 1001);
        var demo2 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 100002, badgeNumber: 1002);
        var demo3 = TestDataBuilder.CreateDemographic(ssn: 123456790, oracleHcmId: 100003, badgeNumber: 1003);
        var demo4 = TestDataBuilder.CreateDemographic(ssn: 123456790, oracleHcmId: 100004, badgeNumber: 1004);

        var demographics = new List<Demographic> { demo1, demo2, demo3, demo4 };
        var duplicateGroups = service.DetectDuplicateSsns(demographics);

        // Act
        var result = service.PrepareAuditDuplicateSsns(duplicateGroups);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // One command per duplicate group
    }

    [Fact]
    [Description("PS-XXXX : Prepare audit for duplicate SSNs with no duplicates returns empty list")]
    public void PrepareAuditDuplicateSsns_WithNoDuplicates_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);
        var emptyGroups = new List<IGrouping<int, Demographic>>();

        // Act
        var result = service.PrepareAuditDuplicateSsns(emptyGroups);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Check SSN conflicts detects mismatched OracleHcmId or Badge for same SSN")]
    public void PrepareCheckSsnConflicts_WithConflicts_CreatesAuditCommands()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        // Existing demographics
        var existing1 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 100001, badgeNumber: 1001);
        var existing2 = TestDataBuilder.CreateDemographic(ssn: 123456790, oracleHcmId: 100002, badgeNumber: 1002);

        // Incoming demographics with same SSN but different OracleHcmId/Badge
        var incoming1 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 999999, badgeNumber: 9999); // Conflict!
        var incoming2 = TestDataBuilder.CreateDemographic(ssn: 123456790, oracleHcmId: 100002, badgeNumber: 1002); // No conflict

        var existingList = new List<Demographic> { existing1, existing2 };
        var incomingList = new List<Demographic> { incoming1, incoming2 };

        // Act
        var result = service.PrepareCheckSsnConflicts(incomingList, existingList);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1); // Only incoming1 has a conflict
    }

    [Fact]
    [Description("PS-XXXX : Check SSN conflicts with terminated status allows overwrite")]
    public void PrepareCheckSsnConflicts_WithTerminatedStatus_AllowsOverwrite()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        // Existing terminated demographic
        var existing = TestDataBuilder.CreateTerminatedDemographic(ssn: 123456789);
        existing.OracleHcmId = 100001;
        existing.BadgeNumber = 1001;

        // Incoming demographic with same SSN but different OracleHcmId/Badge
        var incoming = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 999999, badgeNumber: 9999);

        var existingList = new List<Demographic> { existing };
        var incomingList = new List<Demographic> { incoming };

        // Act
        var result = service.PrepareCheckSsnConflicts(incomingList, existingList);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty(); // No audit command because existing is terminated
    }

    [Fact]
    [Description("PS-XXXX : Check SSN conflicts with no conflicts returns empty list")]
    public void PrepareCheckSsnConflicts_WithNoConflicts_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        var existing1 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 100001, badgeNumber: 1001);
        var existing2 = TestDataBuilder.CreateDemographic(ssn: 123456790, oracleHcmId: 100002, badgeNumber: 1002);

        var incoming1 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 100001, badgeNumber: 1001); // Match
        var incoming2 = TestDataBuilder.CreateDemographic(ssn: 123456790, oracleHcmId: 100002, badgeNumber: 1002); // Match

        var existingList = new List<Demographic> { existing1, existing2 };
        var incomingList = new List<Demographic> { incoming1, incoming2 };

        // Act
        var result = service.PrepareCheckSsnConflicts(incomingList, existingList);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Check SSN conflicts with different SSNs returns empty list")]
    public void PrepareCheckSsnConflicts_WithDifferentSSNs_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        var existing1 = TestDataBuilder.CreateDemographic(ssn: 123456789);
        var existing2 = TestDataBuilder.CreateDemographic(ssn: 123456790);

        var incoming1 = TestDataBuilder.CreateDemographic(ssn: 999999999); // Different SSN
        var incoming2 = TestDataBuilder.CreateDemographic(ssn: 999999998); // Different SSN

        var existingList = new List<Demographic> { existing1, existing2 };
        var incomingList = new List<Demographic> { incoming1, incoming2 };

        // Act
        var result = service.PrepareCheckSsnConflicts(incomingList, existingList);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Check SSN conflicts with empty lists returns empty list")]
    public void PrepareCheckSsnConflicts_WithEmptyLists_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        // Act
        var result = service.PrepareCheckSsnConflicts(new List<Demographic>(), new List<Demographic>());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Check SSN conflicts handles multiple conflicts for same SSN")]
    public void PrepareCheckSsnConflicts_WithMultipleConflictsPerSSN_CreatesMultipleAuditCommands()
    {
        // Arrange
        var service = new DemographicAuditService(_loggerMock.Object);

        // Multiple existing demographics with same SSN
        var existing1 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 100001, badgeNumber: 1001);
        var existing2 = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 100002, badgeNumber: 1002);

        // Incoming demographic with same SSN but different identifiers
        var incoming = TestDataBuilder.CreateDemographic(ssn: 123456789, oracleHcmId: 999999, badgeNumber: 9999);

        var existingList = new List<Demographic> { existing1, existing2 };
        var incomingList = new List<Demographic> { incoming };

        // Act
        var result = service.PrepareCheckSsnConflicts(incomingList, existingList);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // One audit command for each conflicting existing record
    }
}
