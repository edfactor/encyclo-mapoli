using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Enums;
using Demoulas.ProfitSharing.Common.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Common.Validators;

/// <summary>
/// Unit tests for FilterableStartAndEndDateRequestValidator to ensure proper boundary validation
/// including dates, pagination, and vested balance filter parameters.
/// </summary>
public sealed class FilterableStartAndEndDateRequestValidatorTests
{
    private readonly FilterableStartAndEndDateRequestValidator _validator = new();

    #region Vested Balance Value Tests

    [Fact]
    [Description("PS-2009 : Validator should accept positive vested balance value with operator")]
    public void Should_Accept_Positive_Vested_Balance_Value_With_Operator()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = 1000.00m,
            VestedBalanceOperator = ComparisonOperator.GreaterThan
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("PS-2009 : Validator should accept zero vested balance value with operator")]
    public void Should_Accept_Zero_Vested_Balance_Value_With_Operator()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = 0m,
            VestedBalanceOperator = ComparisonOperator.Equals
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("PS-2009 : Validator should reject negative vested balance value")]
    public void Should_Reject_Negative_Vested_Balance_Value()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = -100.00m,
            VestedBalanceOperator = ComparisonOperator.GreaterThan
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.VestedBalanceValue)
            .WithErrorMessage("Vested balance value must be greater than or equal to zero.");
    }

    [Fact]
    [Description("PS-2009 : Validator should reject vested balance value without operator")]
    public void Should_Reject_Vested_Balance_Value_Without_Operator()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = 1000.00m,
            VestedBalanceOperator = null
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.VestedBalanceOperator)
            .WithErrorMessage("Vested balance operator is required when vested balance value is provided.");
    }

    #endregion

    #region Vested Balance Operator Tests

    [Fact]
    [Description("PS-2009 : Validator should accept all valid ComparisonOperator enum values")]
    public void Should_Accept_All_Valid_Comparison_Operators()
    {
        var operators = new[]
        {
            ComparisonOperator.Equals,
            ComparisonOperator.LessThan,
            ComparisonOperator.LessThanOrEqual,
            ComparisonOperator.GreaterThan,
            ComparisonOperator.GreaterThanOrEqual
        };

        foreach (var op in operators)
        {
            var request = new FilterableStartAndEndDateRequest
            {
                BeginningDate = new DateOnly(2024, 1, 1),
                EndingDate = new DateOnly(2024, 12, 31),
                Skip = 0,
                Take = 100,
                VestedBalanceValue = 500.00m,
                VestedBalanceOperator = op
            };

            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Fact]
    [Description("PS-2009 : Validator should reject invalid enum value for operator")]
    public void Should_Reject_Invalid_Enum_Value_For_Operator()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = 1000.00m,
            VestedBalanceOperator = (ComparisonOperator)999 // Invalid enum value
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.VestedBalanceOperator);
    }

    [Fact]
    [Description("PS-2009 : Validator should reject operator without value")]
    public void Should_Reject_Operator_Without_Value()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = null,
            VestedBalanceOperator = ComparisonOperator.GreaterThan
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.VestedBalanceValue)
            .WithErrorMessage("Vested balance value is required when vested balance operator is provided.");
    }

    #endregion

    #region No Vested Balance Filter Tests

    [Fact]
    [Description("PS-2009 : Validator should accept request without vested balance filter")]
    public void Should_Accept_Request_Without_Vested_Balance_Filter()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = null,
            VestedBalanceOperator = null
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Complex Scenarios

    [Fact]
    [Description("PS-2009 : Validator should handle request with all valid fields including vested balance filter")]
    public void Should_Accept_Request_With_All_Valid_Fields()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 50,
            Take = 200,
            VestedBalanceValue = 5000.00m,
            VestedBalanceOperator = ComparisonOperator.GreaterThanOrEqual,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();

        // Verify specific field values
        request.VestedBalanceValue.HasValue.ShouldBeTrue();
        request.VestedBalanceValue.Value.ShouldBe(5000.00m);
        request.VestedBalanceOperator.HasValue.ShouldBeTrue();
        request.VestedBalanceOperator.Value.ShouldBe(ComparisonOperator.GreaterThanOrEqual);
    }

    [Fact]
    [Description("PS-2009 : Validator should reject multiple validation errors")]
    public void Should_Reject_Multiple_Validation_Errors()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 12, 31),
            EndingDate = new DateOnly(2024, 1, 1), // Before beginning
            Skip = -10, // Negative
            Take = 0, // Zero
            VestedBalanceValue = -100.00m, // Negative
            VestedBalanceOperator = ComparisonOperator.GreaterThan
        };

        var result = _validator.TestValidate(request);

        // Should have errors for: EndingDate, Skip, Take, VestedBalanceValue
        result.ShouldHaveValidationErrorFor(x => x.EndingDate);
        result.ShouldHaveValidationErrorFor(x => x.Skip);
        result.ShouldHaveValidationErrorFor(x => x.Take);
        result.ShouldHaveValidationErrorFor(x => x.VestedBalanceValue);
    }

    [Fact]
    [Description("PS-2009 : Validator should accept large vested balance values")]
    public void Should_Accept_Large_Vested_Balance_Values()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = 999999999.99m, // Large value
            VestedBalanceOperator = ComparisonOperator.LessThanOrEqual
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Description("PS-2009 : Validator should accept decimal vested balance values")]
    public void Should_Accept_Decimal_Vested_Balance_Values()
    {
        var request = new FilterableStartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2024, 1, 1),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 100,
            VestedBalanceValue = 1234.56m,
            VestedBalanceOperator = ComparisonOperator.Equals
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Inherited Date and Pagination Validation

    [Fact]
    [Description("PS-2009 : Validator should reject ending date before beginning date")]
    public void Should_Reject_Ending_Date_Before_Beginning_Date()
    {
        var request = new FilterableStartAndEndDateRequest
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
    [Description("PS-2009 : Validator should reject take value greater than maximum")]
    public void Should_Reject_Take_Value_Greater_Than_Maximum()
    {
        var request = new FilterableStartAndEndDateRequest
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
}
