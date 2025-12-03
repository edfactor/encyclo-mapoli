using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[NoMemberDataExposed]
public record BreakdownByStoreTotals
{
    public ushort TotalNumberEmployees { get; set; }
    public decimal TotalBeginningBalances { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalContributions { get; set; }

    public decimal TotalForfeitures { get; set; }

    public decimal TotalDisbursements { get; set; }

    public decimal TotalEndBalances { get; set; }

    public decimal TotalVestedBalance { get; set; }
}
