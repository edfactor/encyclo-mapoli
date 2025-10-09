namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class EmployeeUpdateContext : DeltaContextBase
{
    public long? NationalIdentifierId { get; set; }

    public DateOnly? EffectiveStartDate { get; set; }
}
