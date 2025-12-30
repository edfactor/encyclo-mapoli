namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

public sealed record RowCountResult
{
    public required string TableName { get; set; }
    public int RowCount { get; set; }

    public static RowCountResult ResponseExample()
    {
        return new RowCountResult
        {
            TableName = "Demographic",
            RowCount = 15432
        };
    }
}
