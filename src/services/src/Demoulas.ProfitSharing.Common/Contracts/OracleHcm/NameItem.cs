namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

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
    DateTimeOffset CreationDate,
    string LastUpdatedBy,
    DateTimeOffset LastUpdateDate,
    Context Context
);

