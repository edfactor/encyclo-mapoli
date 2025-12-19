namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

public record ProfitSharingUnder21TotalForStatus(int TotalVested, int PartiallyVested, int PartiallyVestedButLessThanThreeYears)
{
    public static ProfitSharingUnder21TotalForStatus ResponseExample()
    {
        return new ProfitSharingUnder21TotalForStatus(10, 5, 3);
    }
}
