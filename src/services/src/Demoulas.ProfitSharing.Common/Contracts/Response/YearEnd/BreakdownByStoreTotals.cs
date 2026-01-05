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

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static BreakdownByStoreTotals ResponseExample()
    {
        return new BreakdownByStoreTotals
        {
            TotalNumberEmployees = 150,
            TotalBeginningBalances = 500000.00m,
            TotalEarnings = 75000.00m,
            TotalContributions = 45000.00m,
            TotalForfeitures = 5000.00m,
            TotalDisbursements = 30000.00m,
            TotalEndBalances = 585000.00m,
            TotalVestedBalance = 520000.00m
        };
    }
}
