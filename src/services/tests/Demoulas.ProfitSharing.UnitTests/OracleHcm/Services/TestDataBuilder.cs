using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Services;

/// <summary>
/// Fluent test data builder for creating Demographics with realistic test data.
/// Uses Bogus for data generation.
/// </summary>
public static class TestDataBuilder
{
    private static int _nextOracleHcmId = 100000;
    private static int _nextSsn = 123450000;
    private static int _nextBadgeNumber = 10000;

    /// <summary>
    /// Creates a single Demographic with randomized data.
    /// </summary>
    public static Demographic CreateDemographic(
        long? oracleHcmId = null,
        int? ssn = null,
        int? badgeNumber = null,
        short? storeNumber = null,
        char? employmentStatusId = null)
    {
        var faker = new Faker();

        var demographic = new Demographic
        {
            Id = 0, // Will be set by EF
            OracleHcmId = oracleHcmId ?? Interlocked.Increment(ref _nextOracleHcmId),
            Ssn = ssn ?? Interlocked.Increment(ref _nextSsn),
            BadgeNumber = badgeNumber ?? Interlocked.Increment(ref _nextBadgeNumber),
            StoreNumber = storeNumber ?? (short)faker.Random.Int(1, 99),
            PayClassificationId = faker.PickRandom("HOUR", "SAL"),
            ContactInfo = new ContactInfo
            {
                FirstName = faker.Name.FirstName(),
                MiddleName = faker.Name.FirstName().Substring(0, 1),
                LastName = faker.Name.LastName(),
                PhoneNumber = faker.Phone.PhoneNumber("###-###-####"),
                MobileNumber = faker.Phone.PhoneNumber("###-###-####"),
                EmailAddress = faker.Internet.Email()
            },
            Address = new Address
            {
                Street = faker.Address.StreetAddress(),
                Street2 = faker.Random.Bool(0.3f) ? faker.Address.SecondaryAddress() : string.Empty,
                Street3 = string.Empty,
                Street4 = string.Empty,
                City = faker.Address.City(),
                State = faker.Address.StateAbbr(),
                PostalCode = faker.Address.ZipCode(),
                CountryIso = "USA"
            },
            DateOfBirth = DateOnly.FromDateTime(faker.Date.Past(50, DateTime.Now.AddYears(-18))),
            HireDate = DateOnly.FromDateTime(faker.Date.Past(10)),
            ReHireDate = null,
            TerminationDate = null,
            DepartmentId = (byte)faker.Random.Int(1, 50),
            EmploymentTypeId = faker.PickRandom('F', 'P'), // Full-time, Part-time
            GenderId = faker.PickRandom('M', 'F', 'U'),
            PayFrequencyId = (byte)faker.Random.Int(1, 4),
            TerminationCodeId = null,
            EmploymentStatusId = employmentStatusId ?? 'a', // 'a' for active
            FullTimeDate = faker.Random.Bool(0.5f) ? DateOnly.FromDateTime(faker.Date.Past(5)) : null,
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-30),
            ModifiedAtUtc = DateTimeOffset.UtcNow.AddDays(-1)
        };

        return demographic;
    }

    /// <summary>
    /// Creates a list of Demographics with sequential IDs for easy testing.
    /// </summary>
    public static List<Demographic> CreateDemographics(int count)
    {
        var demographics = new List<Demographic>();
        for (int i = 0; i < count; i++)
        {
            demographics.Add(CreateDemographic());
        }
        return demographics;
    }

    /// <summary>
    /// Creates a Demographic with terminated employment status.
    /// </summary>
    public static Demographic CreateTerminatedDemographic(int? ssn = null)
    {
        var demographic = CreateDemographic(ssn: ssn, employmentStatusId: 't');
        demographic.TerminationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        demographic.TerminationCodeId = 'V'; // Voluntary
        return demographic;
    }

    /// <summary>
    /// Creates two Demographics with the same SSN (duplicate scenario).
    /// </summary>
    public static (Demographic first, Demographic second) CreateDuplicateSsn()
    {
        var sharedSsn = Interlocked.Increment(ref _nextSsn);
        var first = CreateDemographic(ssn: sharedSsn);
        var second = CreateDemographic(ssn: sharedSsn);
        return (first, second);
    }

    /// <summary>
    /// Creates a Demographic with zero badge number (edge case).
    /// </summary>
    public static Demographic CreateZeroBadgeDemographic()
    {
        return CreateDemographic(badgeNumber: 0);
    }

    /// <summary>
    /// Creates a DemographicHistory record from a Demographic.
    /// </summary>
    public static DemographicHistory CreateHistoryFromDemographic(Demographic demographic)
    {
        return DemographicHistory.FromDemographic(demographic);
    }

    /// <summary>
    /// Creates a DemographicSyncAudit record.
    /// </summary>
    public static DemographicSyncAudit CreateAudit(
        int? badgeNumber = null,
        long? oracleHcmId = null,
        string? propertyName = null,
        string? message = null)
    {
        return new DemographicSyncAudit
        {
            BadgeNumber = badgeNumber ?? Interlocked.Increment(ref _nextBadgeNumber),
            OracleHcmId = oracleHcmId ?? Interlocked.Increment(ref _nextOracleHcmId),
            InvalidValue = "test-value",
            PropertyName = propertyName ?? "SSN",
            Message = message ?? "Test audit message",
            UserName = "test-user",
            Created = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Resets static counters for test isolation.
    /// </summary>
    public static void Reset()
    {
        _nextOracleHcmId = 100000;
        _nextSsn = 123450000;
        _nextBadgeNumber = 10000;
    }
}
