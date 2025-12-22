namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen
{
    public sealed record NewProfitSharingLabelResponse : ProfitSharingLabelResponse
    {
        public required string Ssn { get; set; }
        public char EmployeeTypeId { get; set; }
        public required string EmployeeTypeName { get; set; }
        public decimal Hours { get; set; }
        public decimal? Balance { get; set; }
        public byte? Years { get; set; }

        public static new NewProfitSharingLabelResponse ResponseExample()
        {
            return new NewProfitSharingLabelResponse()
            {
                StoreNumber = 22,
                PayClassificationId = "2",
                PayClassificationName = "Assistant Manager",
                DepartmentId = 2,
                DepartmentName = "Grocery",
                BadgeNumber = 710023,
                Ssn = "xxx-xx-0231",
                EmployeeName = "Fred Michaels",
                EmployeeTypeId = 'H',
                EmployeeTypeName = "FullTimeStraightSalary",
                Hours = 1223,
                Balance = 49912.44m,
                Years = 6,
                Address1 = "Main St.",
                City = "Anytown",
                State = "MA",
                PostalCode = "02112",
                IsExecutive = false,
            };
        }

    }
}
