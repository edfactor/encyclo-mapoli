namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IHasDateRange
{
    DateOnly StartDate { get; set; }
    DateOnly EndDate { get; set; }
}
