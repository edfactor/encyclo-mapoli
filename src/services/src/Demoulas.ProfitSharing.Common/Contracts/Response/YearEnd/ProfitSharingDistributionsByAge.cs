using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record ProfitSharingDistributionsByAge : PaginatedResponseDto<ProfitSharingDistributionsByAgeDetail>
{
    public short TotalEmployees { get; set; }
    public short HardshipTotalEmployees { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal HardshipTotalAmount { get; set; }
    public short DistributionTotalAmount { get; set; }

    
    public static ProfitSharingDistributionsByAge ResponseExample()
    {
        return new ProfitSharingDistributionsByAge
        {
            TotalEmployees = 93,
            HardshipTotalEmployees = 18,
            TotalAmount = (decimal)1_855_156.09,
            HardshipTotalAmount = (decimal)386_243.46,
            DistributionTotalAmount = 0
        };
    }
}
