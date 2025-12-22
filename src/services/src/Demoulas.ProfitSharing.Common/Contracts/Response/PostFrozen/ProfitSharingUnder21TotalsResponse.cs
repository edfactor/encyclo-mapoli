using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

[NoMemberDataExposed]
public sealed record ProfitSharingUnder21TotalsResponse
{
    public int NumberOfEmployees { get; set; }
    public int NumberOfActiveUnder21With1to2Years { get; set; }
    public int NumberOfActiveUnder21With20to80PctVested { get; set; }
    public int NumberOfActiveUnder21With100PctVested { get; set; }
    public int NumberOfInActiveUnder21With1to2Years { get; set; }
    public int NumberOfInActiveUnder21With20to80PctVested { get; set; }
    public int NumberOfInActiveUnder21With100PctVested { get; set; }
    public int NumberOfTerminatedUnder21With1to2Years { get; set; }
    public int NumberOfTerminatedUnder21With20to80PctVested { get; set; }
    public int NumberOfTerminatedUnder21With100PctVested { get; set; }
    public decimal? TotalBeginningBalance { get; set; }
    public decimal? TotalEarnings { get; set; }
    public decimal? TotalContributions { get; set; }
    public decimal? TotalForfeitures { get; set; }
    public decimal? TotalDisbursements { get; set; }
    public decimal? TotalEndingBalance { get; set; }
    public decimal? TotalVestingBalance { get; set; }

    public static ProfitSharingUnder21TotalsResponse ResponseExample()
    {
        return new ProfitSharingUnder21TotalsResponse
        {
            NumberOfEmployees = 86,
            NumberOfActiveUnder21With1to2Years = 20,
            NumberOfActiveUnder21With20to80PctVested = 12,
            NumberOfActiveUnder21With100PctVested = 4,
            NumberOfInActiveUnder21With1to2Years = 5,
            NumberOfInActiveUnder21With20to80PctVested = 6,
            NumberOfInActiveUnder21With100PctVested = 9,
            NumberOfTerminatedUnder21With1to2Years = 22,
            NumberOfTerminatedUnder21With20to80PctVested = 3,
            NumberOfTerminatedUnder21With100PctVested = 5,
            TotalBeginningBalance = 1599342.55m,
            TotalEarnings = 200123.10m,
            TotalContributions = 10241m,
            TotalForfeitures = 50012m,
            TotalDisbursements = 12031m,
            TotalEndingBalance = 18203184m,
            TotalVestingBalance = 1002312m
        };
    }
}
