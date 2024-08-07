namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

public record WorkRelationshipAssignment(
    short LocationCode,
    byte JobCode, //PayClassification
    string PositionCode,
    string FullPartTime,
    char? Frequency
)
{
    public byte GetDepartmentId()
    {
        _ = byte.TryParse(PositionCode.Split('-').LastOrDefault(), out byte departmentId);
        return departmentId;
    }
};
