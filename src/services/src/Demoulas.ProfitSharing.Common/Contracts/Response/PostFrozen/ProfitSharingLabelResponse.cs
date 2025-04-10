using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
public record ProfitSharingLabelResponse
{
    public short StoreNumber { get; set; }
    public byte PayClassificationId { get; set; }
    public string? PayClassificationName { get; set; }
    public byte DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int BadgeNumber { get; set; }
    public required string EmployeeName { get; set; }
    public string? FirstName { get; set; }
    public string? Address1 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }

    public static ProfitSharingLabelResponse SampleResponse()
    {
        return new ProfitSharingLabelResponse()
        {
            StoreNumber = 22,
            PayClassificationId = 2,
            PayClassificationName = "Assistant Manager",
            DepartmentId = 2,
            DepartmentName = "Grocery",
            BadgeNumber = 710023,
            EmployeeName = "Fred Michaels",
            Address1 = "Main St.",
            City = "Anytown",
            State = "MA",
            PostalCode = "02112"
        };
    }
}
