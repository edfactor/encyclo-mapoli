namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class AssignmentContext : DeltaContextBase
{
    public long SalaryId { get; set; }
    public string? WorkEmail { get; set; }

    public long? AssignmentId { get; set; }
}
