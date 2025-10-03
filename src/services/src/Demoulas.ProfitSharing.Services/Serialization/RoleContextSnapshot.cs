namespace Demoulas.ProfitSharing.Services.Serialization;

/// <summary>
/// Snapshot of caller roles used by masking converter.
/// </summary>
public sealed record RoleContextSnapshot(IReadOnlyList<string> Roles, bool IsItDevOps, bool IsExecutiveAdmin);
