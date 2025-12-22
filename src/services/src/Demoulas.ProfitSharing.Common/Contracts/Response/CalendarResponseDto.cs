namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record CalendarResponseDto
{
    public DateOnly FiscalBeginDate { get; set; }
    public DateOnly FiscalEndDate { get; set; }

    public static CalendarResponseDto ResponseExample()
    {
        return new CalendarResponseDto
        {
            FiscalBeginDate = new DateOnly(2024, 1, 1),
            FiscalEndDate = new DateOnly(2024, 12, 31)
        };
    }
}
