namespace Demoulas.ProfitSharing.Data.Entities;

public class ProfitShareCheck
{
    public int Id { get; set; }
    public int CheckNumber { get; set; }


    public int Ssn { get; set; }
    public required int DemographicId { get; set; }
    public string? PayableName { get; set; }
    public decimal CheckAmount { get; set; }
    public TaxCode? TaxCode { get; set; }
    public char TaxCodeId { get; set; }

    public DateOnly? CheckDate { get; set; }
    public bool? IsVoided { get; set; }
    public DateOnly? VoidDate { get; set; }
    public DateOnly? VoidReconDate { get; set; }
    public DateOnly? ClearDate { get; set; }
    public DateOnly? ClearDateLoaded { get; set; }
    public int? RefNumber { get; set; }

    public short? FloatDays { get; set; }
    public DateOnly? CheckRunDate { get; set; }
    public DateOnly? DateLoaded { get; set; }
    public bool? OtherBeneficiary { get; set; }

    public bool? IsManualCheck { get; set; }
    public string? ReplaceCheck { get; set; }
    public int PscCheckId { get; set; }
}
