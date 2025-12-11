namespace Demoulas.ProfitSharing.UnitTests.Common;

/// <summary>
/// Collection definition for tests that share global state via Constants.FakeParticipantTotals
/// This prevents parallel execution of tests that would interfere with each other
/// </summary>
[CollectionDefinition("SharedGlobalState", DisableParallelization = true)]
public class SharedGlobalStateCollection
{
}
