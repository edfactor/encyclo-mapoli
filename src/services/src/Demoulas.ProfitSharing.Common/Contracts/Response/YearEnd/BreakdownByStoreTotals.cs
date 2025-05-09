namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public record BreakdownByStoreTotals
{
    public short TotalNumberEmployees { get; set; }
    public decimal TotalBeginningBalances { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalContributions { get; set; }

    public decimal TotalForfeitures { get; set; }

    public decimal TotalDisbursements { get; set; }

    public decimal TotalEndBalances { get; set; }

    public decimal TotalVestedBalance { get; set; }
}

public record GrandTotalsByStoreResponseDto
{
    public List<GrandTotalsByStoreRowDto> Rows { get; init; } = new();
}

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


