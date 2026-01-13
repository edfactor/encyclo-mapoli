using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

public sealed record MemberProfitPlanDetails : MemberDetails
{
    public int YearsInPlan { get; init; }
    [UnmaskSensitive] public decimal PercentageVested { get; init; }
    public decimal? BeginPSAmount { get; set; }
    public decimal? CurrentPSAmount { get; set; }
    public decimal? BeginVestedAmount { get; set; }
    public decimal? CurrentVestedAmount { get; set; }
    public decimal AllocationToAmount { get; set; }
    public decimal AllocationFromAmount { get; set; }

    public static new MemberProfitPlanDetails ResponseExample()
    {
        return new MemberProfitPlanDetails
        {
            Id = 1,
            IsEmployee = true,
            BadgeNumber = 1001,
            PsnSuffix = 0,
            PayFrequencyId = 1,
            IsExecutive = false,
            Ssn = "123456789",
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            Address = "123 Main St",
            AddressCity = "Boston",
            AddressState = "MA",
            AddressZipCode = "02101",
            Age = 35,
            DateOfBirth = new DateOnly(1989, 1, 15),
            HireDate = new DateOnly(2015, 3, 1),
            TerminationDate = null,
            ReHireDate = null,
            EmploymentStatus = "Active",
            YearToDateProfitSharingHours = 1040.00m,
            EnrollmentId = 1,
            Enrollment = "Eligible",
            StoreNumber = 5,
            CurrentEtva = 50000.00m,
            PreviousEtva = 48000.00m,
            Department = "Grocery",
            PayClassification = "Full Time",
            Gender = "M",
            PhoneNumber = "555-1234",
            WorkLocation = "Store 5",
            ReceivedContributionsLastYear = true,
            FullTimeDate = new DateOnly(2015, 3, 1),
            TerminationReason = null,
            Missives = [],
            BadgesOfDuplicateSsns = [],
            YearsInPlan = 9,
            PercentageVested = 100.00m,
            BeginPSAmount = 25000.00m,
            CurrentPSAmount = 35000.00m,
            BeginVestedAmount = 25000.00m,
            CurrentVestedAmount = 35000.00m,
            AllocationToAmount = 500.00m,
            AllocationFromAmount = 0.00m
        };
    }
}
