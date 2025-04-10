using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen
{
    public sealed record NewProfitSharingLabelResponse
    {
        public short StoreNumber { get; set; }
        public byte PayClassificationId { get; set; }
        public string? PayClassificationName { get; set; }
        public byte DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int BadgeNumber { get; set; }
        public required string Ssn { get; set; }
        public required string EmployeeName { get; set; }
        public char EmployeeTypeId { get; set; }
        public required string EmployeeTypeName { get; set; }
        public decimal Hours { get; set; }
        public decimal? Balance { get; set; }
        public byte? Years { get; set; }
        public string? Address1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public static NewProfitSharingLabelResponse SampleResponse()
        {
            return new NewProfitSharingLabelResponse()
            {
                StoreNumber = 22,
                PayClassificationId = 2,
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
                PostalCode = "02112"
            };
        }

    }
}
