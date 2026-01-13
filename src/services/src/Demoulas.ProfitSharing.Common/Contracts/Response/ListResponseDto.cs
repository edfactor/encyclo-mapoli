namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// Simple wrapper for non-paginated list responses to provide a consistent envelope
/// and allow future metadata (e.g., last-updated, etag) without breaking clients.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class ListResponseDto<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public int Count => Items.Count;

    public static ListResponseDto<T> From(IEnumerable<T> items) => new()
    {
        Items = items as IReadOnlyList<T> ?? items.ToList()
    };

    public static ListResponseDto<T> ResponseExample() => new()
    {
        Items = new List<T>()
    };
}
