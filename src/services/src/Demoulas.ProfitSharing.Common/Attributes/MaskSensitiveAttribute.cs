namespace Demoulas.ProfitSharing.Common.Attributes;

/// <summary>
/// Apply to a DTO class or property (any data type) to indicate its values should be masked
/// unless explicitly opted out with UnmaskAttribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class MaskSensitiveAttribute : Attribute
{
}
