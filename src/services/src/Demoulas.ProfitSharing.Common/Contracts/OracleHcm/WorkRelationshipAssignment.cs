namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

/// <summary>
/// Represents an assignment within a work relationship in the Oracle HCM system.
/// </summary>
/// <param name="LocationCode">The code representing the location of the assignment.</param>
/// <param name="JobCode">The code representing the job associated with the assignment.</param>
/// <param name="PositionCode">The code representing the position of the assignment.</param>
/// <param name="FullPartTime">Indicates whether the assignment is full-time or part-time.</param>
/// <param name="Frequency">The frequency of the assignment, such as weekly or monthly.</param>
public record WorkRelationshipAssignment(
    short? LocationCode,
    byte? JobCode,
    string? PositionCode,
    string? FullPartTime,
    char? Frequency
)
{
    public byte GetPayFrequency()
    {
        const byte Weekly = 1;
        const byte Monthly = 2;
        switch (Frequency)
        {
            case 'W':
                return Weekly;
            case 'M':
                return Monthly;
            default:
                return byte.MinValue;
        }
    }

    public byte GetDepartmentId()
    {
        _ = byte.TryParse(PositionCode?.Split('-').LastOrDefault(), out byte departmentId);
        return departmentId;
    }

    public char GetEmploymentType()
    {
        const char PartTime = 'P';
        const char FullTimeStraightSalary = 'H';

        return FullPartTime switch
        {
            "PART_TIME" => PartTime,
            "FULL_TIME" => FullTimeStraightSalary,
            _ => char.MinValue
        };
    }
}
