namespace Demoulas.ProfitSharing.Common;

public static class ReferenceData
{
    public static readonly short SmartTransitionYear = 2025;

    public const short MinimumHoursForContribution = 1000;
    public const short MinimumAgeForContribution = 21;
    public const short MinimumAgeForVesting = 18;
    public const short RetirementAge = 65;
    public const short VestingYears = 5;

    public static readonly DateOnly DsmMinValue = new DateOnly(1971, 01, 01);
    public static readonly String CertificateSort = "CERTIFICATESORT";
}
