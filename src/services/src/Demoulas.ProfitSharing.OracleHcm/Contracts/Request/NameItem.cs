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
    string Honors,
    string KnownAs,
    string PreviousLastName,
    string DisplayName,
    string OrderName,
    string ListName,
    string FullName,
    string MilitaryRank,
    string NameLanguage,
    string CreatedBy,
    DateTime CreationDate,
    string LastUpdatedBy,
    DateTime LastUpdateDate,
    Context Context
);

