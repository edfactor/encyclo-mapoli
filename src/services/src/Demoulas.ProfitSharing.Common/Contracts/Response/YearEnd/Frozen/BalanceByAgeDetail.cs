using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record BalanceByAgeDetail : BalanceByDetailBase
{
    [MaskSensitive] public required short Age { get; set; }

    public static BalanceByAgeDetail ResponseExample()
    {
        return new BalanceByAgeDetail { Age = 32, CurrentBalance = (decimal)159_451.46, EmployeeCount = 7, BeneficiaryCount = 3 };
    }
}
