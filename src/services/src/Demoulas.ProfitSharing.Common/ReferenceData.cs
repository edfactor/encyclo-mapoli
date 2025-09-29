namespace Demoulas.ProfitSharing.Common;
public static class ReferenceData
{
    // Vesting Schedule Percent by year
    public static readonly List<byte> OlderVestingSchedule = [0, 0, 20, 40, 60, 80, 100];
    public static readonly List<byte> NewerVestingSchedule = [0, 20, 40, 60, 80, 100];

    public static readonly short SmartTransitionYear = 2024;

#pragma warning disable S3400

    public static short MinimumHoursForContribution()
    {
        return 1000;
    }

    public static short MinimumAgeForContribution()
    {
        return 21;
    }

    public static short MinimumAgeForVesting()
    {
        return 18;
    }

    public static short RetirementAge()
    {
        return 65;
    }

    public static short VestingYears()
    {
        return 5;
    }

    public static readonly DateOnly DsmMinValue = new DateOnly(1971, 01, 01);

    public static readonly String CertificateSort = "CERTIFICATESORT";
#pragma warning restore S3400
}
