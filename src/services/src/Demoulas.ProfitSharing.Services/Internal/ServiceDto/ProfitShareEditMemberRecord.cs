using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

/// <summary>
/// Represents a year end Transaction (aka row in PROFIT_DETAIL) as shown to the user
/// </summary>
public record ProfitShareEditMemberRecord
{
    internal ProfitShareEditMemberRecord(ProfitShareUpdateMember mr, byte profitCode)
    {
        IsEmployee = mr.IsEmployee;
        Ssn = mr.Ssn;
        BadgeNumber = mr.BadgeNumber;
        Psn = long.Parse(mr.Psn!);
        Name = mr.Name;
        ProfitCode = profitCode;
        IsExecutive = mr.PayFrequencyId == PayFrequency.Constants.Monthly;
        if (IsEmployee)
        {
            DisplayedZeroContStatus = (byte)mr.ZeroContributionReasonId!; // NOTE overridden to 0 when ZercontributionReasonId == 7
        }
        else
        {
            DisplayedZeroContStatus = null;
        }
    }

    internal bool IsEmployee { get; init; }
    internal int Ssn { get; set; }
    internal int BadgeNumber { get; set; }
    internal long Psn { get; set; }
    internal string? Name { get; set; }
    internal byte ProfitCode { get; set; }

    internal decimal ContributionAmount { get; set; }
    internal decimal EarningAmount { get; set; }
    internal decimal ForfeitureAmount { get; set; }

    internal string? Remark { get; set; }
    internal byte? CommentTypeId { get; set; }
    internal string? RecordChangeSummary { get; set; }

    internal byte? ZeroContStatus { get; set; } // MAPS to CF_STATUS in PAY447.cbl

    // This is a displayed version of the ZeroContribution status, which oddly for value 7 (64 AND 1ST CONTRIBUTION >=5 YEARS AGO GETS 100% VESTED ON THEIR BIRTHDAY)
    // is displayed as 0 (NORMAL aka "0") in the report.
    internal byte? DisplayedZeroContStatus { get; set; } // MAPS to P-ZEROCONT in PAY447.cbl
    internal byte YearExtension { get; set; }
    internal bool IsExecutive { get; set; }
}
