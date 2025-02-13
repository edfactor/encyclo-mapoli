namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed record ProfitShareUpdateMember
{
    internal bool IsEmployee { get; init; }
    internal int Ssn { get; set; }
    internal int BadgeNumber { get; set; }
    internal long Psn { get; set; }
    internal string? Name { get; set; }
    internal decimal BeginningAmount { get; set; }
    internal decimal Distributions { get; set; }
    internal decimal Military { get; set; }
    internal decimal Xfer { get; set; }
    internal decimal Pxfer { get; set; }
    internal long EmployeeTypeId { get; set; }
    internal decimal Contributions { get; set; }
    internal decimal IncomingForfeitures { get; set; }
    internal decimal AllEarnings { get; set; } // Earnings on both Non-ETVA and ETVA (PY-PROF-EARN)
    internal decimal AllSecondaryEarnings { get; set; } // // Earnings on both Non-ETVA and ETVA (PY-PROF-EARN2) 
    internal decimal EndingBalance { get; set; }
    internal byte? ZeroContributionReasonId { get; set; }
    internal decimal Etva { get; set; } // PAYPROFIT.PY_PS_ETVA
    internal decimal EtvaEarnings { get; set; } // PY-PROF-ETVA  (a portion of Earnings which applies to ETVA)
    internal decimal SecondaryEtvaEarnings { get; set; } // PY_PROF_ETVA2 (portion of Secondary Earnings which applies to ETVA)
}
