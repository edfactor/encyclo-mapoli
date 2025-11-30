using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Contracts;

public class DtoCommonExtensionsTests
{
    private sealed record NameCarrier(string FirstName, string LastName, string? MiddleName = null) : INameParts
    {
        // init implemented by record primary ctor
    }

    [Fact]
    public void ComputeFullName_LastNameFirst_WithMiddle()
    {
        var c = new NameCarrier("Jane", "Doe", "Q");
        c.ComputeFullName().ShouldBe("Doe, Jane Q");
    }

    [Fact]
    public void ComputeFullName_LastNameFirst_NoMiddle()
    {
        var c = new NameCarrier("Jane", "Doe");
        c.ComputeFullName().ShouldBe("Doe, Jane");
    }

    [Fact]
    public void ComputeFullName_FirstNameFirst()
    {
        var c = new NameCarrier("Jane", "Doe", "Q");
        c.ComputeFullName(lastNameFirst: false).ShouldBe("Jane Q Doe");
    }

    [Fact]
    public void HasBasicName_True()
    {
        var c = new NameCarrier("Jane", "Doe");
        c.HasBasicName().ShouldBeTrue();
    }

    [Fact]
    public void HasBasicName_False_WhenMissingLast()
    {
        var c = new NameCarrier("Jane", "");
        c.HasBasicName().ShouldBeFalse();
    }
}
