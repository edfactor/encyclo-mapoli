namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record SimpleRequest<TType> where TType : struct
{
    public TType Id { get; init; }
}
