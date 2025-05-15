namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IHasDateRange
{
    DateOnly StartDate { get; init; }
    DateOnly EndDate { get; init; }
}
