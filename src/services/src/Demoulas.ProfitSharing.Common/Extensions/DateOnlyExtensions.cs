namespace Demoulas.ProfitSharing.Common.Extensions;

public static class DateOnlyExtensions
{
    public static DateTimeOffset ToDateTimeOffset(this DateOnly date, TimeOnly time = default, TimeZoneInfo? timeZone = null)
    {
        var dateTime = date.ToDateTime(time);
        timeZone ??= TimeZoneInfo.Local;
        return new DateTimeOffset(dateTime, timeZone.GetUtcOffset(dateTime));
    }
}
