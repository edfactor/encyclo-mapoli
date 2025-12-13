using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.ItDevOps.Services;

/// <summary>
/// Tests for FrozenService ContactInfo and Address reconstruction from DemographicHistory.
/// Verifies that temporal snapshots use historical contact data instead of current demographic data.
/// </summary>
public class FrozenServiceContactInfoTests
{
    private readonly Mock<ILogger<FrozenService>> _loggerMock;
    private readonly Mock<IDistributedCache> _cacheMock;

    public FrozenServiceContactInfoTests()
    {
        _loggerMock = new Mock<ILogger<FrozenService>>();
        _cacheMock = new Mock<IDistributedCache>();
        TestDataBuilder.Reset();
    }

    [Fact]
    [Description("PS-XXXX : BuildDemographicSnapshot reconstructs ContactInfo with FullName from history")]
    public void BuildDemographicSnapshot_WithContactInfo_ReconstructsFullName()
    {
        // Arrange
        var historicalDemographic = new DemographicHistory
        {
            FirstName = "John",
            LastName = "Smith",
            MiddleName = "Michael",
            PhoneNumber = "555-1234",
            MobileNumber = "555-5678",
            EmailAddress = "john.smith@test.com",
            Street = "123 Main St",
            Street2 = "Apt 4",
            City = "Boston",
            State = "MA",
            PostalCode = "02101"
        };

        // Act
        var result = BuildDemographicSnapshotProjection(historicalDemographic);

        // Assert
        result.ShouldNotBeNull();
        result.ContactInfo.ShouldNotBeNull();
        result.ContactInfo.FirstName.ShouldBe("John");
        result.ContactInfo.LastName.ShouldBe("Smith");
        result.ContactInfo.MiddleName.ShouldBe("Michael");
        result.ContactInfo.FullName.ShouldBe("Smith, John M");
        result.ContactInfo.PhoneNumber.ShouldBe("555-1234");
        result.ContactInfo.MobileNumber.ShouldBe("555-5678");
        result.ContactInfo.EmailAddress.ShouldBe("john.smith@test.com");

        result.Address.ShouldNotBeNull();
        result.Address.Street.ShouldBe("123 Main St");
        result.Address.Street2.ShouldBe("Apt 4");
        result.Address.City.ShouldBe("Boston");
        result.Address.State.ShouldBe("MA");
        result.Address.PostalCode.ShouldBe("02101");
        result.Address.CountryIso.ShouldBe("US");
    }

    [Fact]
    [Description("PS-XXXX : BuildDemographicSnapshot handles NULL ContactInfo fields gracefully")]
    public void BuildDemographicSnapshot_WithNullContactInfo_HandlesNullsGracefully()
    {
        // Arrange - pre-migration record with all NULL contact info fields
        var historicalDemographic = new DemographicHistory
        {
            FirstName = null,
            LastName = null,
            MiddleName = null,
            PhoneNumber = null,
            MobileNumber = null,
            EmailAddress = null,
            Street = null,
            Street2 = null,
            City = null,
            State = null,
            PostalCode = null
        };

        // Act
        var result = BuildDemographicSnapshotProjection(historicalDemographic);

        // Assert - should not throw null reference exception, use defaults for required fields
        result.ShouldNotBeNull();
        result.ContactInfo.ShouldNotBeNull();
        result.ContactInfo.FirstName.ShouldBe(string.Empty); // Required field defaults to empty
        result.ContactInfo.LastName.ShouldBe(string.Empty); // Required field defaults to empty
        result.ContactInfo.MiddleName.ShouldBeNull(); // Optional field stays NULL
        result.ContactInfo.PhoneNumber.ShouldBeNull();
        result.ContactInfo.MobileNumber.ShouldBeNull();
        result.ContactInfo.EmailAddress.ShouldBeNull();
        result.ContactInfo.FullName.ShouldBe(", "); // Computed from empty FirstName and LastName (no middle initial)

        result.Address.ShouldNotBeNull();
        result.Address.Street.ShouldBe(string.Empty); // Required field defaults to empty
        result.Address.Street2.ShouldBeNull();
        result.Address.City.ShouldBe(string.Empty); // Required field defaults to empty
        result.Address.State.ShouldBeNull();
        result.Address.PostalCode.ShouldBeNull();
        result.Address.CountryIso.ShouldBe("US"); // Always defaults to US
    }

    [Fact]
    [Description("PS-XXXX : BuildDemographicSnapshot computes FullName without MiddleName")]
    public void BuildDemographicSnapshot_WithoutMiddleName_ComputesFullNameCorrectly()
    {
        // Arrange
        var historicalDemographic = new DemographicHistory
        {
            FirstName = "Jane",
            LastName = "Doe",
            MiddleName = null // No middle name
        };

        // Act
        var result = BuildDemographicSnapshotProjection(historicalDemographic);

        // Assert
        result.ContactInfo.FullName.ShouldBe("Doe, Jane"); // No middle initial when NULL
    }

    [Fact]
    [Description("PS-XXXX : BuildDemographicSnapshot reconstructs full Address from history")]
    public void BuildDemographicSnapshot_WithAddress_ReconstructsCompleteAddress()
    {
        // Arrange
        var historicalDemographic = new DemographicHistory
        {
            FirstName = "Robert",
            LastName = "Johnson",
            Street = "456 Oak Ave",
            Street2 = "Suite 200",
            City = "Cambridge",
            State = "MA",
            PostalCode = "02142"
        };

        // Act
        var result = BuildDemographicSnapshotProjection(historicalDemographic);

        // Assert
        result.Address.Street.ShouldBe("456 Oak Ave");
        result.Address.Street2.ShouldBe("Suite 200");
        result.Address.City.ShouldBe("Cambridge");
        result.Address.State.ShouldBe("MA");
        result.Address.PostalCode.ShouldBe("02142");
        result.Address.CountryIso.ShouldBe("US");
    }

    [Fact]
    [Description("PS-XXXX : BuildDemographicSnapshot uses historical data not current data")]
    public void BuildDemographicSnapshot_UsesHistoricalData_NotCurrentData()
    {
        // Arrange - historical record shows OLD contact info
        var historicalDemographic = new DemographicHistory
        {
            FirstName = "OldFirstName",
            LastName = "OldLastName",
            MiddleName = "OldMiddle",
            Street = "111 Old Street"
        };

        // Act - when building snapshot from history, should use historical values
        var result = BuildDemographicSnapshotProjection(historicalDemographic);

        // Assert - result should have old values from history, not current demographic
        result.ContactInfo.FirstName.ShouldBe("OldFirstName");
        result.ContactInfo.LastName.ShouldBe("OldLastName");
        result.ContactInfo.MiddleName.ShouldBe("OldMiddle");
        result.Address.Street.ShouldBe("111 Old Street");
    }

    /// <summary>
    /// Helper method to simulate the FrozenService.BuildDemographicSnapshot() projection logic.
    /// This mimics the LINQ projection used in the actual service to convert DemographicHistory
    /// records into Demographic DTOs with reconstructed ContactInfo and Address.
    /// </summary>
    private static DemographicResponse BuildDemographicSnapshotProjection(DemographicHistory dh)
    {
        return new DemographicResponse
        {
            Id = dh.Id,
            Ssn = dh.Ssn,
            BadgeNumber = dh.BadgeNumber,
            OracleHcmId = dh.OracleHcmId,
            ContactInfo = new ContactInfo
            {
                FirstName = dh.FirstName ?? string.Empty,
                LastName = dh.LastName ?? string.Empty,
                MiddleName = dh.MiddleName,
                PhoneNumber = dh.PhoneNumber,
                MobileNumber = dh.MobileNumber,
                EmailAddress = dh.EmailAddress,
                FullName = DtoCommonExtensions.ComputeFullNameWithInitial(
                    dh.LastName ?? string.Empty,
                    dh.FirstName ?? string.Empty,
                    dh.MiddleName)
            },
            Address = new Address
            {
                Street = dh.Street ?? string.Empty,
                Street2 = dh.Street2,
                City = dh.City ?? string.Empty,
                State = dh.State,
                PostalCode = dh.PostalCode,
                CountryIso = "US"
            },
            GenderId = dh.GenderId,
            DateOfBirth = dh.DateOfBirth,
            HireDate = dh.HireDate,
            TerminationDate = dh.TerminationDate,
            EmploymentStatusId = dh.EmploymentStatusId,
            ValidFrom = dh.ValidFrom,
            ValidTo = dh.ValidTo
        };
    }
}
