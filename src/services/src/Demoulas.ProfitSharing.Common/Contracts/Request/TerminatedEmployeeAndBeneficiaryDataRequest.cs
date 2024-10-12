using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record TerminatedEmployeeAndBeneficiaryDataRequest : ProfitYearRequest
{
    [DefaultValue("01/07/2023")]
    public required DateOnly StartDate { get; set; }
    [DefaultValue("01/02/2024")]
    public required DateOnly EndDate { get; set; }
}
