namespace Demoulas.ProfitSharing.Common;
public static class ReferenceData
{
    // Vesting Schedule Percent by year
    public static readonly List<int> OlderVestingSchedule = [0, 0, 20, 40, 60, 80, 100];
    public static readonly List<int> NewerVestingSchedule = [0, 20, 40, 60, 80, 100];

    // a sentinel value passed in externally which indicates the system should choose the correct profit sharing year based on the wall clock.
    // used by all the "Termination Reports" (aka the QPAY066* family of reports)
    public const decimal AutoSelectYear = 9999.9m;

}
