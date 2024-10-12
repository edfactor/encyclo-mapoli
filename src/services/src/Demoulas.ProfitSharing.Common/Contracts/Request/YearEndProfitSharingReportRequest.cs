namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record YearEndProfitSharingReportRequest:ProfitYearRequest
{
    public bool IsYearEnd { get; set; }
}
