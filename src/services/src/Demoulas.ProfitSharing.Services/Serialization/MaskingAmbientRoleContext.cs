namespace Demoulas.ProfitSharing.Services.Serialization;

/// <summary>
/// Ambient role context for masking decisions (AsyncLocal per-request scope).
/// </summary>
public static class MaskingAmbientRoleContext
{
    private static readonly AsyncLocal<RoleContextSnapshot?> _current = new();
    public static RoleContextSnapshot? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }
    public static void Clear() => _current.Value = null;
}
