namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record ProfitDetailDto(
    int Year,
    string Code,
    decimal Contributions,
    decimal Earnings,
    decimal Forfeitures,
    DateOnly Date,
    string? Comments
)
{
    public static ProfitDetailDto ResponseExample() => new(
        Year: 2024,
        Code: "PS",
        Contributions: 5000.00m,
        Earnings: 2500.00m,
        Forfeitures: 0m,
        Date: new DateOnly(2024, 12, 31),
        Comments: "Profit sharing contribution"
    );
}
