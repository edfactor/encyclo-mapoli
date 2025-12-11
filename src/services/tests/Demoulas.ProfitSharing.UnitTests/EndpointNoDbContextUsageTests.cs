using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Infrastructure;

public class EndpointNoDbContextUsageTests
{
    [Fact(DisplayName = "Endpoints assembly must not reference DbContext/DbSet types")]
    public void EndpointsAssembly_ShouldNotReferenceEfCoreSets()
    {
        var endpointsAssembly = Assembly.Load("Demoulas.ProfitSharing.Endpoints");
        var forbidden = new[] { typeof(DbContext).FullName!, typeof(DbSet<>).FullName! };

        var violatingTypes = endpointsAssembly
            .GetTypes()
            .Where(t => t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                         .Any(f => f.FieldType.FullName != null && forbidden.Contains(f.FieldType.FullName))
                     || t.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                         .Any(p => p.PropertyType.FullName != null && forbidden.Contains(p.PropertyType.FullName)))
            .Select(t => t.FullName)
            .ToList();

        violatingTypes.ShouldBeEmpty("Endpoints must not directly depend on DbContext/DbSet; move logic into services.");
    }
}
