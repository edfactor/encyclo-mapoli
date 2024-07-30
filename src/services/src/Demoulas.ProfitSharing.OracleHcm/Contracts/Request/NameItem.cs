namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
public record NameItem(
    string PersonNameId,
    DateTime EffectiveStartDate,
    DateTime EffectiveEndDate,
    string LegislationCode,
    string LastName,
    string FirstName,
    string Title,
    string PreNameAdjunct,
    string Suffix,
    string MiddleNames,
    string KnownAs,
    string PreviousLastName,
    string DisplayName,
    string FullName,
    string MilitaryRank,
    string NameLanguage,
    string CreatedBy,
    DateTime CreationDate,
    string LastUpdatedBy,
    DateTime LastUpdateDate,
    Context Context
);

