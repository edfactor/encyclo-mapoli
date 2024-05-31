namespace Demoulas.ProfitSharing.Common.Extensions;
public static class DateOnlyExtensions
{
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

    public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)
    {
        return ToDateOnly(dateTimeOffset.DateTime);
    }
}
