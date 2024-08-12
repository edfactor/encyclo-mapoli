namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

/// <param name="LocationCode">Store Number</param>
/// <param name="JobCode">Pay Classification</param>
/// <param name="PositionCode">Split on dash "-", Department will be the last value Example: "14-CASHIERS - PM-1" Department = 1</param>
/// <param name="FullPartTime">EmploymentType</param>
/// <param name="Frequency">Pay Frequency W = 1(Weekly) M=2(Monthly)</param>
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
        //const char FullTimeAccruedPaidHolidays = 'G';
        //const char FullTimeEightPaidHolidays = 'F';

        return FullPartTime switch
        {
            "PART_TIME" => PartTime,
            "FULL_TIME" => FullTimeStraightSalary,
            _ => char.MinValue
        };
    }
}
