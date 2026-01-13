namespace Demoulas.ProfitSharing.Data.Entities;

public class Missive
{
    public static class Constants
    {
        public const int VestingIncreasedOnCurrentBalance = 1;
        public const int VestingIsNow100Percent = 2;
        public const int EmployeeIsAlsoABeneficiary = 3;
        public const int BeneficiaryIsNotOnFile = 4;
        public const int EmployeeBadgeIsNotOnFile = 5;
        public const int EmployeeSsnIsNotOnFile = 6;
        public const int EmployeeMayBe100Percent = 7;
        public const int EmployeeUnder21WithBalance = 8;
    }

    public int Id { get; set; }
    public required string Message { get; set; }
    public required string Description { get; set; }
    public required string Severity { get; set; }
}
