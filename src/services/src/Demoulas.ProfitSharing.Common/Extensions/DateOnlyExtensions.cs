namespace Demoulas.ProfitSharing.Common.Extensions;
public static class DateOnlyExtensions
{
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        var ldt = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        return DateOnly.FromDateTime(ldt);
    }

    public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)
    {
        return ToDateOnly(dateTimeOffset.DateTime);
    }
}
