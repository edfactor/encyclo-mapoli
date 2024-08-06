namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
public record WorkRelationship(char WorkerType,
    DateOnly StartDate,
    bool OnMilitaryServiceFlag,
    DateOnly? TerminationDate,
    char? RevokeUserAccess,
    bool? PrimaryFlag
);
