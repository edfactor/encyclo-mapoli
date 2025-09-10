namespace Demoulas.ProfitSharing.Common.Attributes;

/// <summary>
/// Apply to a DTO class or property (any data type) to indicate its values should be masked
/// unless explicitly opted out with UnmaskSensitiveAttribute.
/// Optionally restrict masking to specific roles; if none provided, mask for all roles.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class MaskSensitiveAttribute : Attribute
{
	/// <summary>
	/// The roles for which masking applies; empty means all roles.
	/// </summary>
	public string[] Roles { get; }

	/// <summary>
	/// Create a MaskSensitive attribute optionally limited to one or more roles.
	/// </summary>
	/// <param name="roles">Optional list of role names; omit to mask for all roles.</param>
	/// <remarks>
	/// Examples:
	/// - <c>[MaskSensitive]</c> — mask for all roles.
	/// - <c>[MaskSensitive(Role.ITDEVOPS, Role.ITOPERATIONS)]</c> — mask only for IT support roles.
	/// - Combine with <c>[Unmask]</c> at class/property level to create nuanced visibility rules.
	/// </remarks>
	public MaskSensitiveAttribute(params string[] roles)
	{
		Roles = roles ?? [];
	}
}
