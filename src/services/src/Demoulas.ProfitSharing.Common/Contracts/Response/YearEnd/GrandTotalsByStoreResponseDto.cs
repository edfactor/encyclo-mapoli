namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public record GrandTotalsByStoreResponseDto
{
    public List<GrandTotalsByStoreRowDto> Rows { get; init; } = new();
}