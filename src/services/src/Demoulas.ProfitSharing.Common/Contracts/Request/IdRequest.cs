using System.Numerics;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record IdRequest<T> where T : INumber<T>
{
    public required T Id { get; set; }

    public static IdRequest<T> RequestExample() => new() { Id = T.One };
}
