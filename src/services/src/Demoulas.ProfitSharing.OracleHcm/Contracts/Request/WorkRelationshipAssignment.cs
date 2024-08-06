namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
public record WorkRelationshipAssignment(short LocationCode,
    byte JobCode,
    string PositionCode,
    string FullPartTime,
    char? Frequency
);
