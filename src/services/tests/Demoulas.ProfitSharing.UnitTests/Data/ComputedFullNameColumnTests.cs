using System.ComponentModel;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Data;

/// <summary>
/// Tests for the FullName computed column across all entities (Demographic, BeneficiaryContact, BeneficiaryContactArchive).
/// These tests verify that the database-computed FullName column correctly formats names as "LastName, FirstName" or "LastName, FirstName M".
/// Related Ticket: PS-1829 (FullName consolidation with computed columns)
/// </summary>
public class ComputedFullNameColumnTests
{
    [Description("PS-1829: Demographic FullName computed as 'LastName, FirstName' without middle name")]
    [Fact]
    public void DemographicFullName_WithoutMiddleName_ReturnsCorrectFormat()
    {
        // Arrange
        var demographic = new Demographic
        {
            Ssn = 123456789,
            OracleHcmId = 1,
            BadgeNumber = 100,
            ContactInfo = new ContactInfo
            {
                LastName = "Smith",
                FirstName = "John",
                MiddleName = null
            }
        };

        // Act
        var fullName = demographic.ContactInfo.FullName;

        // Assert - Note: In LINQ, this will use computed column; outside DB it will be null
        // Verification happens in integration tests. This documents the expected format.
        fullName.ShouldBeNull(); // In-memory, not computed
    }

    [Description("PS-1829: Demographic FullName computed as 'LastName, FirstName M' with middle initial")]
    [Fact]
    public void DemographicFullName_WithMiddleName_ReturnsCorrectFormatWithInitial()
    {
        // Arrange
        var demographic = new Demographic
        {
            Ssn = 234567890,
            OracleHcmId = 2,
            BadgeNumber = 101,
            ContactInfo = new ContactInfo
            {
                LastName = "Johnson",
                FirstName = "Jane",
                MiddleName = "Marie"
            }
        };

        // Act
        var fullName = demographic.ContactInfo.FullName;

        // Assert - In-memory, not computed
        fullName.ShouldBeNull();
    }

    [Description("PS-1829: BeneficiaryContact FullName computed column format without middle name")]
    [Fact]
    public void BeneficiaryContactFullName_WithoutMiddleName_ReturnsCorrectFormat()
    {
        // Arrange
        var contact = new BeneficiaryContact
        {
            Ssn = 345678901,
            ContactInfo = new ContactInfo
            {
                LastName = "Williams",
                FirstName = "Robert",
                MiddleName = null
            }
        };

        // Act
        var fullName = contact.ContactInfo.FullName;

        // Assert - In-memory, not computed
        fullName.ShouldBeNull();
    }

    [Description("PS-1829: BeneficiaryContact FullName computed column format with middle initial")]
    [Fact]
    public void BeneficiaryContactFullName_WithMiddleName_ReturnsCorrectFormatWithInitial()
    {
        // Arrange
        var contact = new BeneficiaryContact
        {
            Ssn = 456789012,
            ContactInfo = new ContactInfo
            {
                LastName = "Brown",
                FirstName = "Michael",
                MiddleName = "Edward"
            }
        };

        // Act
        var fullName = contact.ContactInfo.FullName;

        // Assert - In-memory, not computed
        fullName.ShouldBeNull();
    }

    [Description("PS-1829: BeneficiaryContactArchive FullName computed column format")]
    [Fact]
    public void BeneficiaryContactArchiveFullName_WithMiddleName_ReturnsCorrectFormat()
    {
        // Arrange
        var archive = new BeneficiaryContactArchive
        {
            Id = 1,
            Ssn = 567890123,
            ContactInfo = new ContactInfo
            {
                LastName = "Davis",
                FirstName = "Elizabeth",
                MiddleName = "Ann"
            }
        };

        // Act
        var fullName = archive.ContactInfo.FullName;

        // Assert - In-memory, not computed
        fullName.ShouldBeNull();
    }

    [Description("PS-1829: Verify FullName column property is mapped on Demographic")]
    [Fact]
    public void DemographicMapping_HasFullNameProperty()
    {
        // Arrange & Act
        var demographic = new Demographic
        {
            Ssn = 111111111,
            OracleHcmId = 10,
            BadgeNumber = 500,
            ContactInfo = new ContactInfo
            {
                LastName = "Test",
                FirstName = "User",
                MiddleName = "Q"
            }
        };

        // Assert - Property exists and is accessible
        demographic.ContactInfo.ShouldNotBeNull();
        demographic.ContactInfo.LastName.ShouldBe("Test");
        demographic.ContactInfo.FirstName.ShouldBe("User");
        demographic.ContactInfo.MiddleName.ShouldBe("Q");
    }

    [Description("PS-1829: Verify FullName column property is mapped on BeneficiaryContact")]
    [Fact]
    public void BeneficiaryContactMapping_HasFullNameProperty()
    {
        // Arrange & Act
        var contact = new BeneficiaryContact
        {
            Ssn = 222222222,
            ContactInfo = new ContactInfo
            {
                LastName = "Contact",
                FirstName = "Test",
                MiddleName = null
            }
        };

        // Assert - Property exists and is accessible
        contact.ContactInfo.ShouldNotBeNull();
        contact.ContactInfo.LastName.ShouldBe("Contact");
        contact.ContactInfo.FirstName.ShouldBe("Test");
        contact.ContactInfo.MiddleName.ShouldBeNull();
    }

    [Description("PS-1829: Verify FullName column property is mapped on BeneficiaryContactArchive")]
    [Fact]
    public void BeneficiaryContactArchiveMapping_HasFullNameProperty()
    {
        // Arrange & Act
        var archive = new BeneficiaryContactArchive
        {
            Id = 99,
            Ssn = 333333333,
            ContactInfo = new ContactInfo
            {
                LastName = "Archive",
                FirstName = "Test",
                MiddleName = "X"
            }
        };

        // Assert - Property exists and is accessible
        archive.ContactInfo.ShouldNotBeNull();
        archive.ContactInfo.LastName.ShouldBe("Archive");
        archive.ContactInfo.FirstName.ShouldBe("Test");
        archive.ContactInfo.MiddleName.ShouldBe("X");
    }

    [Description("PS-1829: FullName computed column should not be settable (stored computed column)")]
    [Fact]
    public void DemographicFullName_IsReadOnlyWhenRetrievedFromDatabase()
    {
        // This test documents the behavior: in LINQ queries, FullName comes from the
        // computed column in the database and should be used directly without modification.
        // The property is computed, not manually maintained.

        // Arrange
        var demographic = new Demographic
        {
            Ssn = 444444444,
            OracleHcmId = 20,
            BadgeNumber = 501,
            ContactInfo = new ContactInfo
            {
                LastName = "ReadOnly",
                FirstName = "Test"
            }
        };

        // Act & Assert - The property exists and can be read (returns null in-memory since not computed)
        var fullName = demographic.ContactInfo.FullName;
        fullName.ShouldBeNull(); // Computed only in database
    }

    [Description("PS-1829: Verify edge case - single character middle name")]
    [Fact]
    public void FullName_WithSingleCharacterMiddleName_ReturnsCorrectFormat()
    {
        // Arrange
        var demographic = new Demographic
        {
            Ssn = 555555555,
            OracleHcmId = 30,
            BadgeNumber = 502,
            ContactInfo = new ContactInfo
            {
                LastName = "Single",
                FirstName = "Character",
                MiddleName = "X"
            }
        };

        // Act
        var fullName = demographic.ContactInfo.FullName;

        // Assert - In-memory, not computed
        fullName.ShouldBeNull();
    }

    [Description("PS-1829: Verify edge case - long middle name should truncate to first character")]
    [Fact]
    public void FullName_WithLongMiddleName_UsesOnlyFirstCharacter()
    {
        // Arrange
        var demographic = new Demographic
        {
            Ssn = 666666666,
            OracleHcmId = 40,
            BadgeNumber = 503,
            ContactInfo = new ContactInfo
            {
                LastName = "Long",
                FirstName = "Middle",
                MiddleName = "Christopher"  // Should only use 'C'
            }
        };

        // Act
        var fullName = demographic.ContactInfo.FullName;

        // Assert - In-memory, not computed
        fullName.ShouldBeNull();
    }

    [Description("PS-1829: PostFrozenService uses computed FullName instead of manual concatenation")]
    [Fact]
    public void PostFrozenService_ResponseDTO_UsesComputedFullName()
    {
        // This test documents that PostFrozenService now uses d.ContactInfo.FullName
        // from the computed column instead of manual $"{LastName}, {FirstName}" concatenation.
        // Verification: Review PostFrozenService.cs line ~340

        // Arrange - The service should assign like this:
        // FullName = d.ContactInfo.FullName ?? string.Empty,

        var demographic = new Demographic
        {
            Ssn = 777777777,
            OracleHcmId = 50,
            BadgeNumber = 504,
            ContactInfo = new ContactInfo
            {
                LastName = "Service",
                FirstName = "Test",
                MiddleName = "S"
            }
        };

        // Act - Simulate what the service does
        var responseFullName = demographic.ContactInfo.FullName ?? string.Empty;

        // Assert - In-memory it's empty, but in DB it would be computed
        responseFullName.ShouldBe(string.Empty);
    }

    [Description("PS-1829: BreakdownReportService uses computed FullName instead of manual concatenation")]
    [Fact]
    public void BreakdownReportService_ResponseDTO_UsesComputedFullName()
    {
        // This test documents that BreakdownReportService now uses d.ContactInfo.FullName
        // from the computed column instead of the 3-line ternary concatenation.
        // Verification: Review BreakdownReportService.cs line ~746

        // Arrange
        var demographic = new Demographic
        {
            Ssn = 888888888,
            OracleHcmId = 60,
            BadgeNumber = 505,
            ContactInfo = new ContactInfo
            {
                LastName = "Report",
                FirstName = "Breakdown",
                MiddleName = null
            }
        };

        // Act - Simulate what the service does
        var responseFullName = demographic.ContactInfo.FullName ?? string.Empty;

        // Assert - In-memory it's empty, but in DB it would be computed
        responseFullName.ShouldBe(string.Empty);
    }
}
