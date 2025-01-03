namespace Demoulas.ProfitSharing.Common;
public static class ReferenceData
{
    // Vesting Schedule Percent by year
    public static readonly List<byte> OlderVestingSchedule = [0, 0, 20, 40, 60, 80, 100];
    public static readonly List<byte> NewerVestingSchedule = [0, 20, 40, 60, 80, 100];

#pragma warning disable S3400

    public static short MinimumHoursForContribution()
    {
        return 1000;
    }
#pragma warning restore S3400
}
