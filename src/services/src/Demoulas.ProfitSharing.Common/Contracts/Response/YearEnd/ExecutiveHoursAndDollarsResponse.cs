
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Shared;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[YearEndArchiveProperty]
public sealed record  ExecutiveHoursAndDollarsResponse : IFullNameProperty
{
    public required int BadgeNumber { get; set; }
    public required string? FullName { get; set; }
    public required short StoreNumber { get; set; }
    public required string Ssn { get; set; }
    public required decimal HoursExecutive { get; set; }
    public required decimal IncomeExecutive { get; set; }
    public required decimal CurrentHoursYear { get; set; }
    public required decimal CurrentIncomeYear { get; set; }
    public required byte PayFrequencyId { get; set; }
    public required char EmploymentStatusId { get; set; }
    public required string PayFrequencyName { get; set; }
    public required string EmploymentStatusName { get; set; }


    public static  ExecutiveHoursAndDollarsResponse ResponseExample()
    {
        return new  ExecutiveHoursAndDollarsResponse
        {
            BadgeNumber = 1,
            FullName = "John, Null E",
            StoreNumber = 2,
            Ssn = "XXX-XX-8825",
            HoursExecutive = 3,
            IncomeExecutive = 4,
            CurrentHoursYear = 5,
            CurrentIncomeYear = 6,
            PayFrequencyId = 2,
            EmploymentStatusId = 'a',
            PayFrequencyName = "Monthly",
            EmploymentStatusName = "Terminated"
        };
    }
}
