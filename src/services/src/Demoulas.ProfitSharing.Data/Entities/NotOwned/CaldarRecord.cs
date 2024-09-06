namespace Demoulas.ProfitSharing.Data.Entities.NotOwned;
public sealed class CaldarRecord
{
    public DateOnly WeekEndingDate { get; set; }
    public DateOnly WeekDate { get; set; }
    public DateOnly AccApWkend { get; set; }
    public int? AccWeekN { get; set; }
    public int? AccPeriod { get; set; }
    public int? AccQuarter { get; set; }
    public int? AccCalPeriod { get; set; }
    public int? AccCln60Week { get; set; }
    public int? AccCln60Period { get; set; }
    public int? AccCln61Week { get; set; }
    public int? AccCln61Period { get; set; }
    public int? AccCln7XWeek { get; set; }
    public int? AccCln7XPeriod { get; set; }
    public int? AccCln6XWeek { get; set; }
    public int? AccCln6XPeriod { get; set; }
    public long? AccAltKeyNum { get; set; }
}
