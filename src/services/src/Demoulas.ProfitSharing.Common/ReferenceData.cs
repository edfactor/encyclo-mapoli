namespace Demoulas.ProfitSharing.Common;

public static class ReferenceData
{
    public static readonly short SmartTransitionYear = 2025;

    public const short MinimumHoursForContribution = 1000;
    public const short MinimumAgeForContribution = 21;
    public const short MinimumAgeForVesting = 18;
    public const short RetirementAge = 65;

    /// <summary>
    /// Years since first contribution required for pending 100% vesting (ZEROCONT=7).
    /// Per >64 & >5 Rule: "Since 1st Contribution Year = 5" → ZEROCONT 7
    /// </summary>
    public const short PendingVestingYears = 5;

    /// <summary>
    /// Years since first contribution required for full 100% vesting (ZEROCONT=6).
    /// Per >64 & >5 Rule: "Since 1st Contribution Year > 5" → ZEROCONT 6
    /// This means 6+ years (the fifth plan year FOLLOWING the plan year of participation).
    /// </summary>
    public const short FullVestingYears = 6;

    public static readonly DateOnly DsmMinValue = new DateOnly(1971, 01, 01);
    public static readonly string CertificateSort = "CERTIFICATESORT";
}
