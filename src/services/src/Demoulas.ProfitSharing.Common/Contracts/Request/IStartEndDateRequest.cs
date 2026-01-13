namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public interface IStartEndDateRequest
{
    DateOnly? StartDate { get; set; }
    DateOnly? EndDate { get; set; }
}
