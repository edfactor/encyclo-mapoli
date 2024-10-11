namespace Demoulas.ProfitSharing.Common.Extensions;

public static class DateTimeExtension
{
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
