namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record ProfitDetailDto(
    int Year,
    string Code,
    decimal Contributions,
    decimal Earnings,
    decimal Forfeitures,
    DateOnly Date,
    string? Comments
);