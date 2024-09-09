using System.Text;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
public sealed record HttpRequestFields
{
    private readonly HashSet<string> _root = ["PersonNumber", "PersonId", "DateOfBirth", "LastUpdateDate"];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> addresses { get; }=
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
    public HashSet<string> workRelationships { get; } =
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
    public HashSet<string> workRelationships_assignments { get; } =
    [
        "LocationCode", // Store Number
        "JobCode", // Pay Classification
        "PositionCode", // Split on dash "-", Department will be the last value Example: "14-CASHIERS - PM-1" Department = 1
        "FullPartTime", // Might need to be mapped for EmploymentType
        "Frequency" // W = 1(Weekly) M=2(Monthly)
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> emails { get; } =
    [
        "EmailAddressId",
        "EmailType",
        "EmailAddress",
        "PrimaryFlag"
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> names { get; } =
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
    public HashSet<string> nationalIdentifiers { get; } =
    [
        "NationalIdentifierNumber",
        "LastUpdateDate",
        "PrimaryFlag",
    ];

    // ReSharper disable once InconsistentNaming
    public HashSet<string> legislativeInfo { get; } =
    [
        "Gender",
        "MaritalStatus",
        "LastUpdateDate",
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
            { "nationalIdentifiers", request.nationalIdentifiers },
            { "legislativeInfo", request.legislativeInfo }
        };

        StringBuilder sb = new StringBuilder();

        // Append the root elements
        sb.Append(string.Join(",", request._root));
        sb.Append(';');

        // Append the key-value pairs
        foreach (var kvp in collections)
        {
            sb.Append(kvp.Key);
            sb.Append(':');
            sb.Append(string.Join(",", kvp.Value));
            sb.Append(';');
        }

        // Return the formatted string, trimmed of the trailing semicolon
        return sb.ToString().TrimEnd(';');
    }
}
