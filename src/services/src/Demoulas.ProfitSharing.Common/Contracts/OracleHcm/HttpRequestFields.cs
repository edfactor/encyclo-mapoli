using System.Text;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public static class HttpRequestFields
{
    private static readonly HashSet<string> _root = ["PersonNumber", "PersonId", "DateOfBirth", "LastUpdateDate"];

    // ReSharper disable once InconsistentNaming
    private static HashSet<string> addresses { get; } =
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
    private static HashSet<string> workRelationships { get; } =
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
    private static HashSet<string> workRelationships_assignments { get; } =
    [
        "LocationCode", // Store Number
        "JobCode", // Pay Classification
        "PositionCode", // Split on dash "-", Department will be the last value Example: "14-CASHIERS - PM-1" Department = 1
        "DepartmentName",
        "DepartmentId",
        nameof(WorkRelationshipAssignment.AssignmentId),
        nameof(WorkRelationshipAssignment.AssignmentName),
        nameof(WorkRelationshipAssignment.AssignmentNumber),
        "AssignmentCategory",
        "AssignmentCategoryMeaning",
        "FullPartTime", // Might need to be mapped for EmploymentType
        "Frequency" // W = 1(Weekly) M=2(Monthly)
    ];

    // ReSharper disable once InconsistentNaming
    private static HashSet<string> emails { get; } =
    [
        "EmailAddressId",
        "EmailType",
        "EmailAddress",
        "PrimaryFlag"
    ];

    private static HashSet<string> phones { get; } =
    [
        nameof(PhoneItem.AreaCode),
        nameof(PhoneItem.CountryCodeNumber),
        nameof(PhoneItem.LegislationCode),
        nameof(PhoneItem.PhoneNumber),
        nameof(PhoneItem.PhoneId),
        nameof(PhoneItem.PhoneType),
        nameof(PhoneItem.PrimaryFlag),
    ];

    // ReSharper disable once InconsistentNaming
    private static HashSet<string> names { get; } =
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
    private static HashSet<string> nationalIdentifiers { get; } =
    [
        "NationalIdentifierNumber",
        "LastUpdateDate",
        "PrimaryFlag",
    ];

    // ReSharper disable once InconsistentNaming
    private static HashSet<string> legislativeInfo { get; } =
    [
        "Gender",
        "MaritalStatus",
        "LastUpdateDate",
    ];


    private static string _finalFormatted = string.Empty;
    public static string ToFormattedString()
    {
        if (!string.IsNullOrWhiteSpace(_finalFormatted))
        {
            return _finalFormatted;
        }

        var collections = new Dictionary<string, HashSet<string>>
        {
            { "addresses", addresses },
            { "workRelationships", workRelationships },
            { "workRelationships.assignments", workRelationships_assignments },
            { "emails", emails },
            { "phones", phones },
            { "names", names },
            { "nationalIdentifiers", nationalIdentifiers },
            { "legislativeInfo", legislativeInfo }
        };

        StringBuilder sb = new StringBuilder();

        // Append the root elements
        sb.Append(string.Join(",", _root));
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
        _finalFormatted = sb.ToString().TrimEnd(';');

        return _finalFormatted;
    }
}
