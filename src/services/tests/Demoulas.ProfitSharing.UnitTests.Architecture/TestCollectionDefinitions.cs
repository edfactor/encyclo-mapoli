namespace Demoulas.ProfitSharing.UnitTests.Architecture;

/// <summary>
/// Collection definitions for tests that must be run sequentially.
/// </summary>
[CollectionDefinition("Infrastructure Tests", DisableParallelization = true)]
public sealed class InfrastructureTestCollection { }

[CollectionDefinition("Architecture Tests", DisableParallelization = true)]
public sealed class ArchitectureTestCollection { }

[CollectionDefinition("Analyzer Tests", DisableParallelization = true)]
public sealed class AnalyzerTestCollection { }
