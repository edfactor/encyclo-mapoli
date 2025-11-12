using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Validators;
using FluentValidation.TestHelper;

namespace Demoulas.ProfitSharing.UnitTests.Common.Validators;

/// <summary>
/// Unit tests for StartAndEndDateRequestValidator to ensure proper boundary validation
/// including dates and pagination.
/// </summary>
public sealed class StartAndEndDateRequestValidatorTests
{
    private readonly StartAndEndDateRequestValidator _validator = new();

    #region Date Validation Tests

    [Fact]
    [Description("PS-2009 : Validator should accept valid date range")]
    public void Should_Accept_Valid_Date_Range()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("PS-2009 : Validator should accept same beginning and ending date")]
    public void Should_Accept_Same_Beginning_And_Ending_Date()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 6, 15),
            EndingDate = new DateOnly(2024, 6, 15),
            Skip = 0,
            Take = 100
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("PS-2009 : Validator should reject ending date before beginning date")]
    public void Should_Reject_Ending_Date_Before_Beginning_Date()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 12, 31),
            EndingDate = new DateOnly(2024, 1, 1),
            Skip = 0,
            Take = 100
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.EndingDate)
            .WithErrorMessage("Ending date must be greater than or equal to beginning date.");
    }

    [Fact]
    [Description("PS-2009 : Validator should reject default beginning date")]
    public void Should_Reject_Default_Beginning_Date()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = default,
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.BeginningDate)
            .WithErrorMessage("Beginning date is required.");
    }

    [Fact]
    [Description("PS-2009 : Validator should reject default ending date")]
    public void Should_Reject_Default_Ending_Date()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = default,
            Skip = 0,
            Take = 100
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.EndingDate)
            .WithErrorMessage("Ending date is required.");
    }

    #endregion

    #region Pagination Boundary Tests

    [Fact]
    [Description("PS-2009 : Validator should accept maximum page size")]
    public void Should_Accept_Maximum_Page_Size()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 10000 // Maximum allowed
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("PS-2009 : Validator should reject negative skip value")]
    public void Should_Reject_Negative_Skip_Value()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = -1,
            Take = 100
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Skip);
    }

    [Fact]
    [Description("PS-2009 : Validator should reject take value less than 1")]
    public void Should_Reject_Take_Value_Less_Than_One()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 0
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Take);
    }

    [Fact]
    [Description("PS-2009 : Validator should reject take value greater than maximum")]
    public void Should_Reject_Take_Value_Greater_Than_Maximum()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 10001 // Over maximum
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Take);
    }

    #endregion

    #region Complex Scenarios

    [Fact]
    [Description("PS-2009 : Validator should handle request with all valid fields")]
    public void Should_Accept_Request_With_All_Valid_Fields()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 50,
            Take = 200,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("PS-2009 : Validator should reject multiple validation errors")]
    public void Should_Reject_Multiple_Validation_Errors()
    {
        var request = new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 12, 31),
            EndingDate = new DateOnly(2024, 1, 1), // Before beginning
            Skip = -10, // Negative
            Take = 0 // Zero
        };

        var result = _validator.TestValidate(request);

        // Should have errors for: EndingDate, Skip, Take
        result.ShouldHaveValidationErrorFor(x => x.EndingDate);
        result.ShouldHaveValidationErrorFor(x => x.Skip);
        result.ShouldHaveValidationErrorFor(x => x.Take);
    }

    #endregion
}
