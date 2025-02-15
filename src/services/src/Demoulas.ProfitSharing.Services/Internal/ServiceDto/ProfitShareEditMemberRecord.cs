namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

/// <summary>
/// Represents a year end Transaction (aka row in PROFIT_DETAIL) as shown to the user
/// </summary>
public record ProfitShareEditMemberRecord
{
    internal ProfitShareEditMemberRecord(ProfitShareUpdateMember mr, byte code)
    {
        IsEmployee = mr.IsEmployee;
        Ssn = mr.Ssn;
        BadgeNumber = mr.BadgeNumber;
        Psn = mr.Psn;
        Name = mr.Name;
        Code = code;
    }
    internal bool IsEmployee { get; init; }
    internal int Ssn { get; set; }
    internal int BadgeNumber { get; set; }
    internal long Psn { get; set; }
    internal string? Name { get; set; }
    internal byte Code { get; set; }
    internal decimal ContributionAmount { get; set; }
    internal decimal EarningAmount { get; set; }
    internal decimal ForfeitureAmount { get; set; }
    internal string? Remark { get; set; }
    internal byte? CommentTypeId { get; set; }
    internal string? RecordChangeSummary { get; set; }

    internal byte ZeroContStatus { get; set; }
    internal byte YearExtension { get; set; }
}
