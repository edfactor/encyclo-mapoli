using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.ComponentModel;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Services;

public class DemographicHistoryServiceTests
{
    private readonly Mock<ILogger<DemographicHistoryService>> _loggerMock;

    public DemographicHistoryServiceTests()
    {
        _loggerMock = new Mock<ILogger<DemographicHistoryService>>();
        TestDataBuilder.Reset();
    }

    [Fact]
    [Description("PS-XXXX : Prepare update existing with history creates update and history commands when changes detected")]
    public void PrepareUpdateExistingWithHistory_WithChanges_CreatesUpdateAndHistoryCommands()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        var existing = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        existing.Id = 1;
        existing.ContactInfo.FirstName = "John";

        var incoming = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        incoming.ContactInfo.FirstName = "Jane"; // Changed name

        var existingList = new List<Demographic> { existing };
        var incomingList = new List<Demographic> { incoming };

        // Act
        var (updatedCount, commands) = service.PrepareUpdateExistingWithHistory(existingList, incomingList);

        // Assert
        commands.ShouldNotBeNull();
        commands.Count.ShouldBe(2); // One UpdateDemographicCommand + One AddHistoryCommand
        updatedCount.ShouldBe(1);
    }

    [Fact]
    [Description("PS-XXXX : Prepare update existing with history creates no commands when no changes")]
    public void PrepareUpdateExistingWithHistory_WithNoChanges_CreatesNoCommands()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        var existing = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        existing.Id = 1;

        // Create incoming with same values
        var incoming = TestDataBuilder.CreateDemographic(oracleHcmId: 100001);
        incoming.ContactInfo.FirstName = existing.ContactInfo.FirstName;
        incoming.ContactInfo.MiddleName = existing.ContactInfo.MiddleName;
        incoming.ContactInfo.LastName = existing.ContactInfo.LastName;
        incoming.ContactInfo.PhoneNumber = existing.ContactInfo.PhoneNumber;
        incoming.ContactInfo.MobileNumber = existing.ContactInfo.MobileNumber;
        incoming.ContactInfo.EmailAddress = existing.ContactInfo.EmailAddress;
        incoming.Address.Street = existing.Address.Street;
        incoming.Address.Street2 = existing.Address.Street2;
        incoming.Address.City = existing.Address.City;
        incoming.Address.State = existing.Address.State;
        incoming.Address.PostalCode = existing.Address.PostalCode;
        incoming.Ssn = existing.Ssn;
        incoming.BadgeNumber = existing.BadgeNumber;
        incoming.GenderId = existing.GenderId;
        incoming.DateOfBirth = existing.DateOfBirth;
        incoming.HireDate = existing.HireDate;
        incoming.TerminationDate = existing.TerminationDate;
        incoming.EmploymentStatusId = existing.EmploymentStatusId;

        var existingList = new List<Demographic> { existing };
        var incomingList = new List<Demographic> { incoming };

        // Act
        var (updatedCount, commands) = service.PrepareUpdateExistingWithHistory(existingList, incomingList);

        // Assert
        commands.ShouldNotBeNull();
        commands.ShouldBeEmpty(); // No changes, no commands
        updatedCount.ShouldBe(0);
    }

    [Fact]
    [Description("PS-XXXX : Prepare insert new with history creates add demographic and history commands")]
    public void PrepareInsertNewWithHistory_CreatesAddDemographicAndHistoryCommands()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        var newDemographic = TestDataBuilder.CreateDemographic();
        var newDemographicsList = new List<Demographic> { newDemographic };

        // Act
        var (insertedCount, commands) = service.PrepareInsertNewWithHistory(newDemographicsList);

        // Assert
        commands.ShouldNotBeNull();
        commands.Count.ShouldBe(2); // AddDemographicCommand + AddHistoryCommand
        insertedCount.ShouldBe(1);
    }

    [Fact]
    [Description("PS-XXXX : Detect SSN changes returns demographics with changed SSNs")]
    public void DetectSsnChanges_WithChangedSSNs_ReturnsDemographicsWithChanges()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        var existing1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789);
        existing1.Id = 1;
        var existing2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790);
        existing2.Id = 2;
        var existing3 = TestDataBuilder.CreateDemographic(oracleHcmId: 100003, ssn: 123456791);
        existing3.Id = 3;

        var incoming1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 999999999); // Changed SSN
        var incoming2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790); // Same SSN
        var incoming3 = TestDataBuilder.CreateDemographic(oracleHcmId: 100003, ssn: 888888888); // Changed SSN

        var existingList = new List<Demographic> { existing1, existing2, existing3 };
        var incomingByOracleId = new Dictionary<long, Demographic>
        {
            { 100001, incoming1 },
            { 100002, incoming2 },
            { 100003, incoming3 }
        };

        // Act
        var result = service.DetectSsnChanges(existingList, incomingByOracleId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // existing1 and existing3 have SSN changes
        result.ShouldContain(existing1);
        result.ShouldContain(existing3);
        result.ShouldNotContain(existing2);
    }

    [Fact]
    [Description("PS-XXXX : Detect SSN changes with no changes returns empty list")]
    public void DetectSsnChanges_WithNoChanges_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        var existing1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789);
        var existing2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790);

        var incoming1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789); // Same SSN
        var incoming2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790); // Same SSN

        var existingList = new List<Demographic> { existing1, existing2 };
        var incomingByOracleId = new Dictionary<long, Demographic>
        {
            { 100001, incoming1 },
            { 100002, incoming2 }
        };

        // Act
        var result = service.DetectSsnChanges(existingList, incomingByOracleId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Detect SSN changes with empty list returns empty list")]
    public void DetectSsnChanges_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        // Act
        var result = service.DetectSsnChanges(new List<Demographic>(), new Dictionary<long, Demographic>());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    [Description("PS-XXXX : Prepare SSN update commands creates commands for beneficiaries and profit details")]
    public void PrepareSsnUpdateCommands_WithSsnChanges_CreatesBeneficiaryAndProfitDetailCommands()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        var existing = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789);
        var incoming = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 999999999);

        var ssnChangedList = new List<Demographic> { existing };
        var incomingByOracleId = new Dictionary<long, Demographic>
        {
            { 100001, incoming }
        };

        // Act
        var result = service.PrepareSsnUpdateCommands(ssnChangedList, incomingByOracleId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // UpdateBeneficiaryContactsSsnCommand + UpdateProfitDetailsSsnCommand
    }

    [Fact]
    [Description("PS-XXXX : Prepare SSN update commands with multiple changes creates commands for each")]
    public void PrepareSsnUpdateCommands_WithMultipleChanges_CreatesCommandsForEach()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        var existing1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 123456789);
        var existing2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 123456790);

        var incoming1 = TestDataBuilder.CreateDemographic(oracleHcmId: 100001, ssn: 999999999);
        var incoming2 = TestDataBuilder.CreateDemographic(oracleHcmId: 100002, ssn: 888888888);

        var ssnChangedList = new List<Demographic> { existing1, existing2 };
        var incomingByOracleId = new Dictionary<long, Demographic>
        {
            { 100001, incoming1 },
            { 100002, incoming2 }
        };

        // Act
        var result = service.PrepareSsnUpdateCommands(ssnChangedList, incomingByOracleId);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(4); // 2 changes * 2 commands each = 4 total
    }

    [Fact]
    [Description("PS-XXXX : Prepare SSN update commands with empty list returns empty list")]
    public void PrepareSsnUpdateCommands_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var service = new DemographicHistoryService(_loggerMock.Object);

        // Act
        var result = service.PrepareSsnUpdateCommands(new List<Demographic>(), new Dictionary<long, Demographic>());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }
}
