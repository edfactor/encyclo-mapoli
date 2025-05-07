
namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record ForfeitureAdjustmentReportDetail
{
    public required int ClientNumber { get; set; }
    public required int BadgeNumber { get; set; }
    public required decimal StartingBalance { get; set; }
    public required decimal ForfeitureAmount { get; set; }
    public required decimal NetBalance { get; set; }
    public required decimal NetVested { get; set; }

    public static ForfeitureAdjustmentReportDetail ResponseExample()
    {
        return new ForfeitureAdjustmentReportDetail
        {
            ClientNumber = 123,
            BadgeNumber = 456,
            StartingBalance = 1000,
            ForfeitureAmount = 100,
            NetBalance = 900,
            NetVested = 800
        };
    }
}