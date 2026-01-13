namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record ChangedAttribute
{
    public string? SalaryBasisId { get; set; }
    public long? NationalIdentifierId { get; set; }
    public string? LegislationCode { get; set; }
    public string? NationalIdentifierType { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}
