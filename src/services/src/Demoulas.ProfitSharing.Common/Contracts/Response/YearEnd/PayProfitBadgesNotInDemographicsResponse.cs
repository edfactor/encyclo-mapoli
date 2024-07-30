namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record PayProfitBadgesNotInDemographicsResponse
{
    public required long EmployeeBadge { get; set; }
    public required long EmployeeSSN { get; set; }
}
