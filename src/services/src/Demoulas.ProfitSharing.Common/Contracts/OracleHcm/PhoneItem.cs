namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record PhoneItem(
    string PhoneId,
    string PhoneType,
    string LegislationCode,
    string CountryCodeNumber,
    string AreaCode,
    string PhoneNumber,
    string Extension,
    DateTime FromDate,
    DateTime? ToDate,
    string Validity,
    string CreatedBy,
    DateTime CreationDate,
    string LastUpdatedBy,
    DateTimeOffset LastUpdateDate,
    bool? PrimaryFlag,
    Context Context
);
