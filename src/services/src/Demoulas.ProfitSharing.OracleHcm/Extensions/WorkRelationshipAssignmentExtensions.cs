using System.Text.RegularExpressions;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.OracleHcm.Extensions;
internal static class WorkRelationshipAssignmentExtensions
{
    private static readonly List<Department> _departmentArray =
    [
        new Department { Id = Department.Constants.Grocery, Name = "Grocery" },
        new Department { Id = Department.Constants.Meat, Name = "Meat" },
        new Department { Id = Department.Constants.Produce, Name = "Produce" },
        new Department { Id = Department.Constants.Deli, Name = "Deli" },
        new Department { Id = Department.Constants.Dairy, Name = "Dairy" },
        new Department { Id = Department.Constants.BeerAndWine, Name = "Beer/Wine" },
        new Department { Id = Department.Constants.Bakery, Name = "Bakery" }
    ];

public static byte GetPayFrequency(this WorkRelationshipAssignment work)
    {
        const byte weekly = 1;
        const byte monthly = 2;
        switch (work.Frequency)
        {
            case 'W':
                return weekly;
            case 'M':
                return monthly;
            default:
                return byte.MinValue;
        }
    }

    public static byte GetDepartmentId(this WorkRelationshipAssignment work)
    {
        if (string.IsNullOrWhiteSpace(work.PositionCode) && string.IsNullOrWhiteSpace(work.DepartmentName))
        {
            return 0;
        }

        var department = _departmentArray.Find(d => string.Compare(d.Name , work.DepartmentName, StringComparison.InvariantCultureIgnoreCase) == 0 );
        if (department != null)
        {
            return department.Id;
        }

        if (string.IsNullOrWhiteSpace(work.PositionCode))
        {
            return 0;
        }

        // Regular expression to find the first numeric sequence after the second dash
        var match = Regex.Match(work.PositionCode, @"(?:.*?-){2}(\d+)", RegexOptions.Compiled);
        if (match.Success && byte.TryParse(match.Groups[1].Value, out byte departmentId))
        {
            return departmentId;
        }

        return 0; // Return default value if parsing fails
    }

    public static char GetEmploymentType(this WorkRelationshipAssignment work)
    {
        const char partTime = 'P';
        const char fullTimeStraightSalary = 'H';

        return work.FullPartTime switch
        {
            "PART_TIME" => partTime,
            "FULL_TIME" => fullTimeStraightSalary,
            _ => char.MinValue
        };
    }
}
