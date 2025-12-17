using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public record ProfitShareUpdateMemberResponse : IIsExecutive
{
    public bool IsEmployee { get; init; }
    public long Badge { get; set; }
    public string? Psn { get; set; }
    [MaskSensitive] public string? Name { get; set; }
    [MaskSensitive] public string? FullName => Name;
    public decimal BeginningAmount { get; set; }
    public decimal Distributions { get; set; }
    public decimal Military { get; set; }
    public decimal Xfer { get; set; }
    public decimal Pxfer { get; set; }
    public long EmployeeTypeId { get; set; }
    public decimal Contributions { get; set; }
    public decimal IncomingForfeitures { get; set; }
    public decimal AllEarnings { get; set; } // Earnings on both Non-ETVA and ETVA (PY-PROF-EARN)
    public decimal AllSecondaryEarnings { get; set; } // // Earnings on both Non-ETVA and ETVA (PY-PROF-EARN2) 
    public decimal EndingBalance { get; set; }
    public byte? ZeroContributionReasonId { get; set; }
    public decimal Etva { get; set; } // PAYPROFIT.PY_PS_ETVA
    public decimal EtvaEarnings { get; set; } // PY-PROF-ETVA  (a portion of Earnings which applies to ETVA)
    public decimal SecondaryEtvaEarnings { get; set; } // PY_PROF_ETVA2 (portion of Secondary Earnings which applies to ETVA)
    public bool TreatAsBeneficiary { get; set; }
    public bool IsExecutive { get; set; }
}
