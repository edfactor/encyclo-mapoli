namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

/// <summary>
/// Response DTO for store list lookup operations.
/// Contains store identification and associated department information.
/// </summary>
public sealed record StoreListResponse
{
    /// <summary>
    /// The unique identifier for the store.
    /// </summary>
    public required int StoreId { get; init; }

    /// <summary>
    /// The display name of the store (e.g., "1 - FLETCHER").
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// The city where the store is located.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// The state where the store is located.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// The ZIP/postal code for the store location.
    /// </summary>
    public string? ZipCode { get; init; }

    /// <summary>
    /// Indicates whether this is a spirits store.
    /// </summary>
    public bool HasBakery { get; init; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the store has spirits.
    /// </summary>
    public bool HasSpirits { get; init; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the store has a kitchen.
    /// </summary>
    public bool HasKitchen { get; init; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the store has a cafe.
    /// </summary>
    public bool HasCafe { get; init; } = false;
}

/// <summary>
/// Response DTO for single store lookup by ID.
/// Contains detailed store information including departments and store type.
/// </summary>
public sealed record StoreDetailResponse
{
    /// <summary>
    /// The unique identifier for the store.
    /// </summary>
    public required int StoreId { get; init; }

    /// <summary>
    /// The display name of the store (e.g., "1 - FLETCHER").
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// The city where the store is located.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// The state where the store is located.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// The ZIP/postal code for the store location.
    /// </summary>
    public string? ZipCode { get; init; }

    /// <summary>
    /// Indicates whether this is a spirits store.
    /// </summary>
    public bool HasBakery { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the store has spirits.
    /// </summary>
    public bool? HasSpirits { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the store has a kitchen.
    /// </summary>
    public bool? HasKitchen { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the store has a cafe.
    /// </summary>
    public bool? HasCafe { get; init; }
}
