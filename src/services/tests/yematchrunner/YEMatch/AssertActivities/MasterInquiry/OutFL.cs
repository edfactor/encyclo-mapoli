// OutFL.cs

namespace YEMatch.AssertActivities.MasterInquiry;

/* Slavishly follows the Cobol output format */
public sealed record OutFL
{
    public required string OUT_SSN { get; init; } = "";
    public required decimal OUT_HRS { get; init; }
    public required int OUT_YEARS { get; init; }
    public required string OUT_ENROLLED { get; init; } = "";
    public required decimal OUT_BEGIN_BAL { get; init; }
    public required decimal OUT_BEGIN_VEST { get; init; }
    public required decimal OUT_CURRENT_BAL { get; init; }
    public required decimal OUT_VESTING_PCT { get; init; }
    public required decimal OUT_VESTING_AMT { get; init; }
    public required bool OUT_CONT_LAST_YEAR { get; init; }
    public required decimal OUT_ETVA { get; init; }
    public required string OUT_ERR_MESG { get; init; } = "";
}
