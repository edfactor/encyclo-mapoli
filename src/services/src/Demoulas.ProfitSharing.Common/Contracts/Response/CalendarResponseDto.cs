namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record CalendarResponseDto
{
    public DateOnly FiscalBeginDate { get; set; }
    public DateOnly FiscalEndDate { get; set; }
}
