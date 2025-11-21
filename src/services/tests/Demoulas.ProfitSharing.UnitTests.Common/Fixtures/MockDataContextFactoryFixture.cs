using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fixtures;

/// <summary>
/// xUnit Fixture that provides each test class with its own isolated factory instance.
/// This prevents state pollution between test classes by ensuring each class gets
/// 6,500+ fresh fake records.
/// 
/// Usage in test class:
/// <code>
/// public class MyTestClass : IClassFixture&lt;MockDataContextFactoryFixture&gt;
/// {
///     private readonly IProfitSharingDataContextFactory _factory;
///     
///     public MyTestClass(MockDataContextFactoryFixture fixture)
///     {
///         _factory = fixture.Factory;
///     }
/// }
/// </code>
/// </summary>
public class MockDataContextFactoryFixture : IDisposable
{
    public IProfitSharingDataContextFactory Factory { get; }

    public MockDataContextFactoryFixture()
    {
        // Each test class gets its own fresh factory instance with 6,500+ fake records
        Factory = MockDataContextFactory.InitializeForTesting();
    }

    public void Dispose()
    {
        // Cleanup if needed (mock objects don't require cleanup, but adding for completeness)
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// xUnit Collection Fixture that can be used to group related tests that share state.
/// Tests in the same collection will reuse the same factory instance.
/// </summary>
[CollectionDefinition("MockDataContext Collection")]
public class MockDataContextCollection : ICollectionFixture<MockDataContextFactoryFixture>
{
    // This class has no code; it just declares the collection behavior
}
