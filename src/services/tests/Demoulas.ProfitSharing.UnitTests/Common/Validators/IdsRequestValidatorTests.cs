using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Endpoints.Validation;
using FluentValidation.TestHelper;

namespace Demoulas.ProfitSharing.UnitTests.Common.Validators;

/// <summary>
/// Unit tests for IdsRequestValidator to ensure proper boundary validation
/// according to backend-expert guidelines.
/// </summary>
public sealed class IdsRequestValidatorTests
{
    private readonly IdsRequestValidator _validator = new();

    [Fact]
    [Description("Validator should reject null Ids array")]
    public void Should_Reject_Null_Ids()
    {
        var request = new IdsRequest { Ids = null! };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage("Ids cannot be null.");
    }

    [Fact]
    [Description("Validator should reject empty Ids array")]
    public void Should_Reject_Empty_Ids()
    {
        var request = new IdsRequest { Ids = Array.Empty<int>() };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage("At least one ID must be provided.");
    }

    [Fact]
    [Description("Validator should reject batch sizes exceeding 1000")]
    public void Should_Reject_Oversized_Batch()
    {
        var ids = Enumerable.Range(1, 1001).ToArray();
        var request = new IdsRequest { Ids = ids };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage("Cannot process more than 1000 IDs in a single request.");
    }

    [Fact]
    [Description("Validator should reject non-positive IDs")]
    public void Should_Reject_NonPositive_Ids()
    {
        var request = new IdsRequest { Ids = new[] { 1, 0, -1 } };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage("All IDs must be positive integers.");
    }

    [Fact]
    [Description("Validator should reject duplicate IDs")]
    public void Should_Reject_Duplicate_Ids()
    {
        var request = new IdsRequest { Ids = new[] { 1, 2, 2, 3 } };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage("Duplicate IDs are not allowed.");
    }

    [Fact]
    [Description("Validator should accept valid ID arrays")]
    public void Should_Accept_Valid_Ids()
    {
        var request = new IdsRequest { Ids = new[] { 1, 2, 3, 100, 999 } };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("Validator should accept maximum batch size of 1000")]
    public void Should_Accept_Maximum_Batch_Size()
    {
        var ids = Enumerable.Range(1, 1000).ToArray();
        var request = new IdsRequest { Ids = ids };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
