using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Mappers;

/// <summary>
/// Unit tests for OracleHcm Mapperly mappers (DemographicMapper, AddressMapper, ContactInfoMapper).
/// </summary>
public class DemographicMapperTests
{
    private readonly DemographicMapper _mapper;

    public DemographicMapperTests()
    {
        var addressMapper = new AddressMapper();
        var contactInfoMapper = new ContactInfoMapper();
        _mapper = new DemographicMapper(addressMapper, contactInfoMapper);
    }

    /// <summary>
    /// Creates a valid DemographicsRequest with all required properties for testing.
    /// </summary>
    private static DemographicsRequest CreateValidRequest(
        long oracleHcmId = 100001,
        int ssn = 123456789,
        int badgeNumber = 9001,
        short storeNumber = 42)
    {
        return new DemographicsRequest
        {
            OracleHcmId = oracleHcmId,
            Ssn = ssn,
            BadgeNumber = badgeNumber,
            StoreNumber = storeNumber,
            DepartmentId = 10,
            PayClassificationId = "FT",
            DateOfBirth = new DateOnly(1980, 1, 1),
            HireDate = new DateOnly(2020, 1, 1),
            EmploymentTypeCode = 'E',
            PayFrequencyId = 1,
            GenderCode = 'M',
            EmploymentStatusId = 'A',
            ContactInfo = new ContactInfoRequestDto
            {
                FirstName = "John",
                LastName = "Doe"
            },
            Address = new AddressRequestDto
            {
                Street = "123 Main St"
            }
        };
    }

    [Fact]
    [Description("PS-XXXX : Maps DemographicsRequest to Demographic entity correctly")]
    public void Map_ValidRequest_ReturnsDemographicEntity()
    {
        // Arrange
        var request = CreateValidRequest();
        request = request with
        {
            StoreNumber = 42,
            DepartmentId = 10,
            PayClassificationId = "MGMT",
            FullTimeDate = new DateOnly(2011, 1, 1),
            EmploymentTypeCode = 'F',
            PayFrequencyId = 2,
            GenderCode = 'M',
            EmploymentStatusId = 'A'
        };
        request.ContactInfo = request.ContactInfo with
        {
            MiddleName = "Q",
            PhoneNumber = "555-1234",
            EmailAddress = "john.doe@example.com"
        };
        request.Address = request.Address with
        {
            City = "Boston",
            State = "MA",
            PostalCode = "02101"
        };

        // Act
        var result = _mapper.Map(request);

        // Assert
        result.ShouldNotBeNull();
        result.Ssn.ShouldBe(123456789);
        result.BadgeNumber.ShouldBe(9001);
        result.OracleHcmId.ShouldBe(100001);
        result.StoreNumber.ShouldBe((short)42);
        result.DepartmentId.ShouldBe((byte)10);
        result.PayClassificationId.ShouldBe("MGMT");

        // Contact info
        result.ContactInfo.ShouldNotBeNull();
        result.ContactInfo.FirstName.ShouldBe("John");
        result.ContactInfo.LastName.ShouldBe("Doe");
        result.ContactInfo.MiddleName.ShouldBe("Q");
        result.ContactInfo.PhoneNumber.ShouldBe("555-1234");
        result.ContactInfo.EmailAddress.ShouldBe("john.doe@example.com");

        // Address
        result.Address.ShouldNotBeNull();
        result.Address.Street.ShouldBe("123 Main St");
        result.Address.City.ShouldBe("Boston");
        result.Address.State.ShouldBe("MA");
        result.Address.PostalCode.ShouldBe("02101");

        // Date fields
        result.DateOfBirth.ShouldBe(new DateOnly(1980, 1, 1));
        result.HireDate.ShouldBe(new DateOnly(2020, 1, 1));
        result.FullTimeDate.ShouldBe(new DateOnly(2011, 1, 1));
        result.ReHireDate.ShouldBeNull();
        result.TerminationDate.ShouldBeNull();

        // Classification fields
        result.EmploymentTypeId.ShouldBe('F');
        result.PayFrequencyId.ShouldBe((byte)2);
        result.GenderId.ShouldBe('M');
        result.EmploymentStatusId.ShouldBe('A');
    }

    [Fact]
    [Description("PS-XXXX : Maps collection of DemographicsRequests correctly")]
    public void Map_MultipleRequests_ReturnsCorrectCount()
    {
        // Arrange
        var request1 = CreateValidRequest(oracleHcmId: 100001, ssn: 111223333, badgeNumber: 9001, storeNumber: 1);
        request1.ContactInfo = request1.ContactInfo with { FirstName = "Alice", LastName = "Smith" };
        request1.Address = request1.Address with { Street = "10 Alice St", City = "Boston" };

        var request2 = CreateValidRequest(oracleHcmId: 100002, ssn: 222334444, badgeNumber: 9002, storeNumber: 2);
        request2.ContactInfo = request2.ContactInfo with { FirstName = "Bob", LastName = "Jones" };
        request2.Address = request2.Address with { Street = "20 Bob St", City = "Cambridge" };

        var requests = new[] { request1, request2 };

        // Act
        var results = _mapper.Map(requests).ToList();

        // Assert
        results.Count.ShouldBe(2);
        results[0].ContactInfo.FirstName.ShouldBe("Alice");
        results[1].ContactInfo.FirstName.ShouldBe("Bob");
    }

    [Fact]
    [Description("PS-XXXX : Handles null optional fields correctly")]
    public void Map_NullOptionalFields_HandlesGracefully()
    {
        // Arrange
        var request = CreateValidRequest();
        request = request with
        {
            ReHireDate = null,
            TerminationDate = null,
            TerminationCodeId = null
        };

        // Act
        var result = _mapper.Map(request);

        // Assert
        result.ShouldNotBeNull();
        result.ReHireDate.ShouldBeNull();
        result.TerminationDate.ShouldBeNull();
        result.TerminationCodeId.ShouldBeNull();
    }
}

/// <summary>
/// Unit tests for AddressMapper.
/// </summary>
public class AddressMapperTests
{
    private readonly AddressMapper _mapper = new AddressMapper();

    [Fact]
    [Description("PS-XXXX : Maps Address request to Address entity correctly")]
    public void Map_ValidAddressRequest_ReturnsAddressEntity()
    {
        // Arrange
        var request = new AddressRequestDto
        {
            Street = "456 Elm Street",
            City = "Springfield",
            State = "IL",
            PostalCode = "62701"
        };

        // Act
        var result = _mapper.Map(request);

        // Assert
        result.ShouldNotBeNull();
        result.Street.ShouldBe("456 Elm Street");
        result.City.ShouldBe("Springfield");
        result.State.ShouldBe("IL");
        result.PostalCode.ShouldBe("62701");
    }
}

/// <summary>
/// Unit tests for ContactInfoMapper.
/// </summary>
public class ContactInfoMapperTests
{
    private readonly ContactInfoMapper _mapper = new ContactInfoMapper();

    [Fact]
    [Description("PS-XXXX : Maps ContactInfo request to ContactInfo entity correctly")]
    public void Map_ValidContactInfoRequest_ReturnsContactInfoEntity()
    {
        // Arrange
        var request = new ContactInfoRequestDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            MiddleName = "M",
            PhoneNumber = "555-9876",
            EmailAddress = "jane.doe@example.com"
        };

        // Act
        var result = _mapper.Map(request);

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("Jane");
        result.LastName.ShouldBe("Doe");
        result.MiddleName.ShouldBe("M");
        result.PhoneNumber.ShouldBe("555-9876");
        result.EmailAddress.ShouldBe("jane.doe@example.com");
    }
}
