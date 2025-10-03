using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Common.Base;

public static class TestServiceOverrides
{
    /// <summary>
    /// When set by a test prior to creating ApiTestBase, this action will be invoked with the
    /// test host's IServiceCollection so tests may register or override services for the host.
    /// </summary>
    public static Action<IServiceCollection>? Hook { get; set; }
}
