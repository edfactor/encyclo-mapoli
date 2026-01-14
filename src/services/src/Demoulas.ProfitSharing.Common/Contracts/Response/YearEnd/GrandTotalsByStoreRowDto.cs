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

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static GrandTotalsByStoreRowDto ResponseExample()
    {
        return new GrandTotalsByStoreRowDto
        {
            Category = "Distributions",
            Store700 = 85000.00m,
            Store701 = 72000.00m,
            Store800 = 91000.00m,
            Store801 = 68000.00m,
            Store802 = 79000.00m,
            Store900 = 82000.00m,
            StoreOther = 43000.00m,
            RowTotal = 520000.00m
        };
    }
}
