namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record SetFrozenStateRequest : YearRequest
{
    public DateTime AsOfDateTime { get; set; }

    public static new SetFrozenStateRequest RequestExample() => new()
    {
        ProfitYear = 2024,
        AsOfDateTime = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Unspecified)
    };
}
