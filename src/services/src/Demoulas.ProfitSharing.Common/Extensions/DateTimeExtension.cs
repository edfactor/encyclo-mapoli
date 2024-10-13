namespace Demoulas.ProfitSharing.Common.Extensions;

public static class DateTimeExtension
{
    public static int Age(this DateOnly birthDate)
    {
        return Age(birthDate, DateTime.Today);
    }

    public static int Age(this DateOnly birthDate, DateTime fromDateTime)
    {
        return Age(birthDate.ToDateTime(TimeOnly.MinValue), fromDateTime);
    }

    public static int Age(this DateTime birthDate)
    {
        return Age(birthDate, DateTime.Today);
    }

    public static int Age(this DateTime birthDate, DateTime fromDateTime)
    {
        int age = fromDateTime.Year - birthDate.Year;
        if (birthDate.Date > fromDateTime.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
