using System.Text.RegularExpressions;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.OracleHcm.Extensions;

internal static partial class WorkRelationshipAssignmentExtensions
{
    [GeneratedRegex(@"(?:.*?-){2}(\d+)", RegexOptions.Compiled)]
    private static partial Regex _departmentRegex();

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

    /// <summary>
    /// Retrieves the pay frequency for the specified work relationship assignment.
    /// </summary>
    /// <param name="work">The work relationship assignment instance.</param>
    /// <returns>
    /// A byte representing the pay frequency:
    /// 1 for weekly,
    /// 2 for monthly,
    /// or <see cref="byte.MinValue"/> if the frequency is not recognized.
    /// </returns>
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

    /// <summary>
    /// Retrieves the department ID associated with the specified work relationship assignment.
    /// </summary>
    /// <param name="work">The work relationship assignment from which to extract the department ID.</param>
    /// <returns>
    /// The department ID as a byte. Returns 0 if the department ID cannot be determined.
    /// </returns>
    public static byte GetDepartmentId(this WorkRelationshipAssignment work)
    {
        if (string.IsNullOrWhiteSpace(work.PositionCode) && string.IsNullOrWhiteSpace(work.DepartmentName))
        {
            return 0;
        }

        Department? department = _departmentArray.Find(d => string.Compare(d.Name, work.DepartmentName, StringComparison.InvariantCultureIgnoreCase) == 0);
        if (department != null)
        {
            return department.Id;
        }

        if (string.IsNullOrWhiteSpace(work.PositionCode))
        {
            return 0;
        }

        // Regular expression to find the first numeric sequence after the second dash
        Match match = _departmentRegex().Match(work.PositionCode);
        if (match.Success && byte.TryParse(match.Groups[1].Value, out byte departmentId))
        {
            return departmentId;
        }

        return 0; // Return default value if parsing fails
    }

    /// <summary>
    /// Determines the employment type of a given work relationship assignment.
    /// </summary>
    /// <param name="work">The work relationship assignment to evaluate.</param>
    /// <returns>
    /// A character representing the employment type:
    /// 'P' for part-time,
    /// 'H' for full-time straight salary,
    /// or <see cref="char.MinValue"/> if the employment type cannot be determined.
    /// </returns>
    public static char GetEmploymentType(this WorkRelationshipAssignment work)
    {
        const char partTime = 'P';
        const char fullTimeStraightSalary = 'H';

        char result = work.AssignmentCategory switch
        {
            "PT" => partTime,
            "PR" => partTime, // Part Time Regular
            "FT" => fullTimeStraightSalary,
            "FR" => fullTimeStraightSalary, // Full Time Regular
            _ => char.MinValue
        };

        if (result != char.MinValue)
        {
            return result;
        }

        result = work.AssignmentCategoryMeaning switch
        {
            "Part-time" => partTime,
            "Full-time" => fullTimeStraightSalary,
            _ => char.MinValue
        };

        if (result != char.MinValue)
        {
            return result;
        }

        return work.FullPartTime switch
        {
            "PART_TIME" => partTime,
            "FULL_TIME" => fullTimeStraightSalary,
            _ => char.MinValue
        };
    }
}
