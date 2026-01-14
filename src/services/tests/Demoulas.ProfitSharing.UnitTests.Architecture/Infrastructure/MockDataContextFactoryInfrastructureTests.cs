using System.ComponentModel;
using System.Reflection;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Infrastructure;

[Collection("Infrastructure Tests")]
public sealed class MockDataContextFactoryInfrastructureTests
{
    [Fact]
    [Description("PS-2337 : MockDataContextFactory non-generic UseReadOnlyContext invokes delegate")]
    public async Task UseReadOnlyContext_NonGeneric_ShouldInvokeDelegate()
    {
        // Arrange
        var factory = MockDataContextFactory.InitializeForTesting();
        var wasInvoked = false;

        // Act
        await factory.UseReadOnlyContext(_ =>
        {
            wasInvoked = true;
            return Task.CompletedTask;
        });

        // Assert
        wasInvoked.ShouldBeTrue();
    }

    [Fact]
    [Description("PS-2337 : MockDataContextFactory non-generic UseReadOnlyContext honors cancellation")]
    public async Task UseReadOnlyContext_NonGeneric_WhenCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var factory = MockDataContextFactory.InitializeForTesting();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var wasInvoked = false;

        // Act / Assert
        await Should.ThrowAsync<OperationCanceledException>(() => factory.UseReadOnlyContext(_ =>
        {
            wasInvoked = true;
            return Task.CompletedTask;
        }, cts.Token));

        wasInvoked.ShouldBeFalse();
    }

    [Fact]
    [Description("PS-2337 : MockDataContextFactory non-generic UseReadOnlyContext unwraps TargetInvocationException")]
    public async Task UseReadOnlyContext_NonGeneric_WhenTargetInvocationExceptionThrown_ShouldRethrowInnerException()
    {
        // Arrange
        var factory = MockDataContextFactory.InitializeForTesting();

        // Act / Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(() => factory.UseReadOnlyContext(_ =>
        {
            throw new TargetInvocationException(new InvalidOperationException("boom"));
        }));

        ex.Message.ShouldBe("boom");
    }
}
