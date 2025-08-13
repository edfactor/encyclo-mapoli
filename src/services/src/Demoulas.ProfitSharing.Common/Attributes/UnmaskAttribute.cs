namespace Demoulas.ProfitSharing.Common.Attributes;

/// <summary>
/// Apply to a DTO class or property to opt-out of masking (including default decimal masking).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class UnmaskAttribute : Attribute
{
}
