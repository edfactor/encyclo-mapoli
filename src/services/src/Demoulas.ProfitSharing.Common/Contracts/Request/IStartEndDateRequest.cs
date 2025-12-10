namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public interface IStartEndDateRequest
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
