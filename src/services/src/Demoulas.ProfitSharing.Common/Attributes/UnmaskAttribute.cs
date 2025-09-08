namespace Demoulas.ProfitSharing.Common.Attributes;

/// <summary>
/// Apply to a DTO class or property to opt-out of masking (including default decimal masking).
/// Optionally restrict the unmasking to specific roles; if no roles provided, applies to all roles.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class UnmaskAttribute : Attribute
{
	/// <summary>
	/// The roles for which unmasking applies; empty means all roles.
	/// </summary>
	public string[] Roles { get; }

	/// <summary>
	/// Create an Unmask attribute optionally limited to one or more roles.
	/// </summary>
	/// <param name="roles">Optional list of role names; omit to unmask for all roles.</param>
	/// <remarks>
	/// Examples:
	/// - <c>[Unmask]</c> — unmask for all roles.
	/// - <c>[Unmask(Role.FINANCEMANAGER, Role.ADMINISTRATOR)]</c> — only Finance and Admin see clear values.
	/// - <c>[Unmask(Role.EXECUTIVEADMIN)]</c> — only Executive-Administrators see executive values in clear.
	/// </remarks>
	public UnmaskAttribute(params string[] roles)
	{
		Roles = roles ?? [];
	}
}
