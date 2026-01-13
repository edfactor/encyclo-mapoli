using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

[NoMemberDataExposed]
public sealed record ProfitControlSheetResponse
{
    public decimal EmployeeContributionProfitSharingAmount { get; set; }
    public decimal NonEmployeeProfitSharingAmount { get; set; }
    public decimal EmployeeBeneficiaryAmount { get; set; }
    public decimal ProfitSharingAmount
    {
        get
        {
            return EmployeeContributionProfitSharingAmount + NonEmployeeProfitSharingAmount + EmployeeBeneficiaryAmount;
        }
    }

    public static ProfitControlSheetResponse ResponseExample()
    {
        return new ProfitControlSheetResponse()
        {
            EmployeeContributionProfitSharingAmount = 21991510m,
            NonEmployeeProfitSharingAmount = 150102m,
            EmployeeBeneficiaryAmount = 12502m
        };
    }
}
