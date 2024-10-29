namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public record CalendarResponseDto
{
    public DateOnly BeginDate { get; set; }
    public DateOnly YearEndDate { get; set; }
}
