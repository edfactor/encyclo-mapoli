namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
internal sealed record HttpRequestFields
{
    public HashSet<string> Root = ["PersonNumber", "PersonId", "DateOfBirth", "LastUpdateDate"];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> addresses =
    [
        "AddressId",
        "AddressLine1",
        "AddressLine2",
        "AddressLine3",
        "AddressLine4",
        "TownOrCity",
        "Region1",
        "Region2",
        "Country",
        "CountryName",
        "PostalCode",
        "LongPostalCode",
        "Building",
        "FloorNumber",
        "CreatedBy",
        "CreationDate",
        "LastUpdatedBy",
        "PersonAddrUsageId",
        "AddressType",
        "AddressTypeMeaning",
        "PrimaryFlag"
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> workRelationships =
    [
        "WorkerType",
        "StartDate",
        "OnMilitaryServiceFlag",
        "TerminationDate",
        "NotificationDate",
        "RevokeUserAccess",
        "PrimaryFlag"
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> workRelationships_assignments =
    [
        "LocationCode", // Store Number
        "JobCode", // Pay Classification
        "PositionCode", // Split on dash "-", Department will be the last value Example: "14-CASHIERS - PM-1" Department = 1
        "FullPartTime", // Might need to be mapped for EmploymentType
        "Frequency" // W = 1(Weekly) M=2(Monthly)
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> emails =
    [
        "EmailAddressId",
        "EmailType",
        "EmailAddress",
        "PrimaryFlag"
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> names =
    [
        "PersonNameId",
        "EffectiveStartDate",
        "EffectiveEndDate",
        "LegislationCode",
        "Title",
        "LastName",
        "MiddleNames",
        "FirstName",
        "FullName",
        "Suffix",
        "KnownAs",
        "PreviousLastName",
        "DisplayName",
        "PreNameAdjunct",
        "MilitaryRank",
        "NameLanguage",
        "LastUpdateDate"
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> nationalIdentifiers =
    [
        "NationalIdentifierNumber",
        "LastUpdateDate",
        "PrimaryFlag",
    ];

    public static string ToFormattedString()
    {
        var request = new HttpRequestFields();

        var collections = new Dictionary<string, HashSet<string>>
        {
            { "addresses", request.addresses },
            { "workRelationships", request.workRelationships },
            { "workRelationships.assignments", request.workRelationships_assignments },
            { "emails", request.emails },
            { "names", request.names },
            { "nationalIdentifiers", request.nationalIdentifiers }
        };

        string formattedString = string.Join(",", request.Root) + ";";
        foreach (var kvp in collections)
        {
            formattedString += $"{kvp.Key}:{string.Join(",", kvp.Value)};";
        }

        return formattedString.TrimEnd(';');
    }
}
