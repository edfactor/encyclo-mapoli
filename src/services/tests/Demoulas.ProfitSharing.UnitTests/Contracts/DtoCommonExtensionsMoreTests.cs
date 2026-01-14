using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Contracts;

public class DtoCommonExtensionsMoreTests
{
    private sealed record ContactCarrier(string? EmailAddress, string? PhoneNumber) : IEmailAddress, IPhoneNumber;

    private sealed record NameCarrier(string FirstName, string LastName, string? MiddleName = null) : INameParts;

    [Fact]
    public void HasContactChannel_True_WithEmailOnly()
    {
        var c = new ContactCarrier("test@example.com", null);
        c.HasContactChannel(c).ShouldBeTrue();
    }

    [Fact]
    public void HasContactChannel_False_BothMissing()
    {
        var c = new ContactCarrier(null, null);
        c.HasContactChannel(c).ShouldBeFalse();
    }

    [Fact]
    public void ComputeFullName_Fallback()
    {
        var n = new NameCarrier("Jane", "Doe", "Q");
        n.ComputeFullName().ShouldBe("Doe, Jane Q");
    }
}
