namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public record GrandTotalsByStoreRowDto
{
    public string Category { get; init; } = string.Empty;

    public decimal Store700 { get; init; }
    public decimal Store701 { get; init; }
    public decimal Store800 { get; init; }
    public decimal Store801 { get; init; }
    public decimal Store802 { get; init; }
    public decimal Store900 { get; init; }
    public decimal StoreOther { get; init; }

    public decimal RowTotal { get; init; }
}
