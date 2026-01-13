using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

namespace Demoulas.ProfitSharing.UnitTests.Common.Extensions;

public static class OracleEmployeeExtensions
{
    public static OracleEmployee ToOracleFromDemographic(this Data.Entities.Demographic d)
    {
        // Build minimal-yet-valid OracleEmployee from Demographic faker fields
        var addressItem = new AddressItem
        {
            AddressLine1 = d.Address.Street,
            AddressLine2 = d.Address.Street2,
            AddressLine3 = d.Address.Street3,
            AddressLine4 = d.Address.Street4,
            TownOrCity = d.Address.City ?? "City",
            State = d.Address.State ?? "MA",
            Country = d.Address.CountryIso ?? "USA",
            PostalCode = d.Address.PostalCode ?? "01810",
            PrimaryFlag = true
        };
        var addresses = new Addresses(
            Items: new List<AddressItem> { addressItem },
            TotalResults: null,
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0,
            Links: new List<Link>()
        );

        var emailItem = new EmailItem(
            EmailAddressId: Guid.NewGuid().ToString("N"),
            EmailType: "WORK",
            EmailAddress: d.ContactInfo.EmailAddress ?? "noreply@example.com",
            PrimaryFlag: true
        );
        var emails = new Emails(
            Items: new List<EmailItem> { emailItem },
            TotalResults: null,
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0,
            Links: new List<Link>()
        );

        var nameItem = new NameItem(
            PersonNameId: Guid.NewGuid().ToString("N"),
            EffectiveStartDate: DateTime.UtcNow.AddYears(-10),
            EffectiveEndDate: DateTime.UtcNow.AddYears(10),
            LegislationCode: "US",
            LastName: d.ContactInfo.LastName,
            FirstName: d.ContactInfo.FirstName,
            Title: string.Empty,
            PreNameAdjunct: string.Empty,
            Suffix: d.ContactInfo.MiddleName ?? string.Empty,
            MiddleNames: d.ContactInfo.MiddleName ?? string.Empty,
            KnownAs: string.Empty,
            PreviousLastName: string.Empty,
            DisplayName: d.ContactInfo.FullName ?? $"{d.ContactInfo.LastName}, {d.ContactInfo.FirstName}",
            FullName: d.ContactInfo.FullName ?? $"{d.ContactInfo.FirstName} {d.ContactInfo.LastName}",
            MilitaryRank: string.Empty,
            NameLanguage: "en",
            CreatedBy: "faker",
            CreationDate: DateTimeOffset.UtcNow,
            LastUpdatedBy: "faker",
            LastUpdateDate: DateTimeOffset.UtcNow,
            Context: new Context(
                Key: "/workers",
                Headers: new Headers(ETag: "tag"),
                Links: new List<Link>()
            )
        );
        var names = new Names(
            Items: new List<NameItem> { nameItem },
            TotalResults: null,
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0,
            Links: new List<Link>()
        );

        var phoneItem = new PhoneItem(
            PhoneId: Guid.NewGuid().ToString("N"),
            PhoneType: "MOBILE",
            LegislationCode: "US",
            CountryCodeNumber: "1",
            AreaCode: "978",
            PhoneNumber: (d.ContactInfo.MobileNumber ?? d.ContactInfo.PhoneNumber ?? "555-555-5555").Replace("-", string.Empty),
            Extension: string.Empty,
            FromDate: DateTime.UtcNow.AddYears(-5),
            ToDate: null,
            Validity: "VALID",
            CreatedBy: "faker",
            CreationDate: DateTime.UtcNow.AddYears(-5),
            LastUpdatedBy: "faker",
            LastUpdateDate: DateTimeOffset.UtcNow,
            PrimaryFlag: true,
            Context: new Context(
                Key: "/workers",
                Headers: new Headers(ETag: "tag"),
                Links: new List<Link>()
            )
        );
        var phones = new Phones(
            Items: new List<PhoneItem> { phoneItem },
            TotalResults: null,
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0,
            Links: new List<Link>()
        );

        var assignment = new WorkRelationshipAssignment(
            LocationCode: d.StoreNumber,
            JobCode: d.PayClassificationId,
            PositionCode: d.DepartmentId.ToString(),
            DepartmentName: d.Department?.Name,
            DepartmentId: d.DepartmentId,
            FullPartTime: d.EmploymentTypeId.ToString(),
            AssignmentId: null,
            AssignmentName: null,
            AssignmentNumber: null,
            AssignmentCategory: null,
            AssignmentCategoryMeaning: null,
            Frequency: null
        );
        var assignments = new WorkRelationshipAssignments(
            Items: new List<WorkRelationshipAssignment> { assignment },
            TotalResults: null,
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0
        );
        var wr = new WorkRelationships(
            Items: new List<WorkRelationship>
            {
                new WorkRelationship(
                    WorkerType: 'E',
                    StartDate: d.HireDate,
                    OnMilitaryServiceFlag: false,
                    TerminationDate: d.TerminationDate,
                    RevokeUserAccess: null,
                    PrimaryFlag: true,
                    Assignments: assignments
                )
            },
            TotalResults: null,
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0
        );

        var nationalIds = new NationalIdentifiers(
            Items: new List<NationalIdentifier>
            {
                new NationalIdentifier(
                    NationalIdentifierId: null,
                    NationalIdentifierNumber: d.Ssn.ToString().PadLeft(9, '0'),
                    LastUpdateDate: DateTimeOffset.UtcNow,
                    PrimaryFlag: true)
            },
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0
        );

        var legislativeInfo = new LegislativeInfo(
            Items: new List<LegislativeInfoItem>
            {
                new LegislativeInfoItem(
                    Gender: d.GenderId == 'M' ? "M" : (d.GenderId == 'F' ? "F" : null),
                    MaritalStatus: null,
                    LastUpdateDate: DateTimeOffset.UtcNow)
            },
            Count: 1,
            HasMore: false,
            Limit: 1,
            Offset: 0,
            Links: new List<Link>()
        );

        var context = new Context(
            Key: "/workers",
            Headers: new Headers(ETag: "etag"),
            Links: new List<Link>()
        );

        return new OracleEmployee(
            PersonId: d.OracleHcmId,
            BadgeNumber: d.BadgeNumber,
            DateOfBirth: d.DateOfBirth,
            DateOfDeath: d.DateOfDeath,
            CreatedBy: "faker",
            CreationDate: DateTimeOffset.UtcNow,
            LastUpdatedBy: "faker",
            LastUpdateDate: DateTimeOffset.UtcNow,
            Addresses: addresses,
            Emails: emails,
            Names: names,
            Phones: phones,
            WorkRelationships: wr,
            NationalIdentifiers: nationalIds,
            LegislativeInfo: legislativeInfo,
            Context: context
        );
    }
}
