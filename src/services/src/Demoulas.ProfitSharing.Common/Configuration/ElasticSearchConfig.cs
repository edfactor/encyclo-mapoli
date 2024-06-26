using System.Diagnostics;

namespace Demoulas.ProfitSharing.Common.Configuration;
public sealed record ElasticSearchConfig
{
    public bool EnableElasticSearchLogging { get; set; } = !Debugger.IsAttached;
    public ICollection<Uri>? ElasticSearchNodes { get; set; }
    public string? ProjectName { get; set; }
    public string? Namespace { get; set; }
}
