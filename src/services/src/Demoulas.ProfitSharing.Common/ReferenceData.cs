namespace Demoulas.ProfitSharing.Common;
public static class ReferenceData
{
    // Vesting Schedule Percent by year
    public static readonly List<byte> OlderVestingSchedule = [0, 0, 20, 40, 60, 80, 100];
    public static readonly List<byte> NewerVestingSchedule = [0, 20, 40, 60, 80, 100];
}
