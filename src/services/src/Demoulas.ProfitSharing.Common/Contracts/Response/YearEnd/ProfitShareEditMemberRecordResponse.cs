namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// Represents a year end Transaction (aka row in PROFIT_DETAIL) as shown to the user
/// </summary>
public record ProfitShareEditMemberRecordResponse
{
    public ProfitShareEditMemberRecordResponse(ProfitShareUpdateMemberResponse mr, byte code)
    {
        Ssn = mr.Ssn;
        Badge = mr.Badge;
        Psn = mr.Psn;
        Name = mr.Name;
        Code = code;
    }

    public int Ssn { get; set; }
    public long Badge { get; set; }
    public long Psn { get; set; }
    public string? Name { get; set; }
    public byte Code { get; set; }
    public decimal ContributionAmount { get; set; }
    public decimal EarningsAmount { get; set; }
    public decimal IncomingForfeitures { get; set; }
    public string? Reason { get; set; }
    public string? ReasonSummary { get; set; }

    public int ZeroContStatus { get; set; }
    public short YearExtension { get; set; }

    public static ProfitShareEditMemberRecordResponse ResponseExample()
    {
        return new ProfitShareEditMemberRecordResponse(new ProfitShareUpdateMemberResponse(), 0)
        {
            Ssn = 777,
            Badge = 123,
            Psn = 0,
            Name = "VILLANUEVA, ISAAC",
            ContributionAmount = 4350m,
            EarningsAmount = 50,
            IncomingForfeitures = 290,
            Reason = "V-ONLY",
            ReasonSummary = "18,19,20 > 1000"
        };
    }
}
