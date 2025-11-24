namespace Demoulas.ProfitSharing.Common.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public sealed class YearEndArchivePropertyAttribute : Attribute
{
    public string? KeyName { get; }

    public YearEndArchivePropertyAttribute([System.Runtime.CompilerServices.CallerMemberName] string? keyName = null)
    {
        KeyName = keyName;
    }
}
