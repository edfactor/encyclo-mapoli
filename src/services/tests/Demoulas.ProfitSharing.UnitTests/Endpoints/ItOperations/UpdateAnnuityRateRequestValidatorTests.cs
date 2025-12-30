using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Validators;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;

/// <summary>
/// Unit tests for UpdateAnnuityRateRequestValidator to verify decimal place validation.
/// Tests ensure that rates with up to 4 decimal places are accepted, and more than 4 are rejected.
/// PS-2382: Fix decimal validation to correctly accept values like 1.0111 (exactly 4 decimal places).
/// </summary>
[Collection("Validation Tests")]
public sealed class UpdateAnnuityRateRequestValidatorTests
{
    private readonly UpdateAnnuityRateRequestValidator _validator = new();

    #region SingleRate Decimal Place Tests

    [Fact(DisplayName = "Validator - SingleRate with 0 decimal places should be valid")]
    [Description("PS-2382 : Validates integer rates like 10")]
    public void Validate_SingleRateZeroDecimals_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 10m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - SingleRate with 1 decimal place should be valid")]
    [Description("PS-2382 : Validates rates like 10.5")]
    public void Validate_SingleRateOneDecimal_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 10.5m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - SingleRate with 2 decimal places should be valid")]
    [Description("PS-2382 : Validates rates like 10.55")]
    public void Validate_SingleRateTwoDecimals_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 10.55m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - SingleRate with 3 decimal places should be valid")]
    [Description("PS-2382 : Validates rates like 10.555")]
    public void Validate_SingleRateThreeDecimals_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 10.555m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - SingleRate with exactly 4 decimal places should be valid")]
    [Description("PS-2382 : Validates rates like 1.0111 (bug fix case)")]
    public void Validate_SingleRateFourDecimals_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 1.0111m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - SingleRate with max value and 4 decimal places should be valid")]
    [Description("PS-2382 : Validates maximum valid rate 99.9999")]
    public void Validate_SingleRateMaxWithFourDecimals_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 99.9999m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - SingleRate with 5 decimal places should be invalid")]
    [Description("PS-2382 : Rejects rates with more than 4 decimal places")]
    public void Validate_SingleRateFiveDecimals_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 1.01111m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "SingleRate" && e.ErrorMessage.Contains("4 decimal places"));
    }

    [Fact(DisplayName = "Validator - SingleRate with 6 decimal places should be invalid")]
    [Description("PS-2382 : Rejects rates with many decimal places")]
    public void Validate_SingleRateSixDecimals_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 1.011111m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "SingleRate" && e.ErrorMessage.Contains("4 decimal places"));
    }

    [Fact(DisplayName = "Validator - SingleRate exceeding max value should be invalid")]
    [Description("PS-2382 : Validates upper bound range")]
    public void Validate_SingleRateExceedsMax_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 100.0000m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "SingleRate" && e.ErrorMessage.Contains("99.9999"));
    }

    [Fact(DisplayName = "Validator - SingleRate below min value should be invalid")]
    [Description("PS-2382 : Validates lower bound range")]
    public void Validate_SingleRateBelowMin_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = -0.0001m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "SingleRate");
    }

    #endregion

    #region JointRate Decimal Place Tests

    [Fact(DisplayName = "Validator - JointRate with exactly 4 decimal places should be valid")]
    [Description("PS-2382 : Validates joint rates like 1.0111")]
    public void Validate_JointRateFourDecimals_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 10m,
            JointRate = 1.0111m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - JointRate with 5 decimal places should be invalid")]
    [Description("PS-2382 : Rejects joint rates with more than 4 decimal places")]
    public void Validate_JointRateFiveDecimals_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 10m,
            JointRate = 1.01111m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "JointRate" && e.ErrorMessage.Contains("4 decimal places"));
    }

    #endregion

    #region Year and Age Validation Tests

    [Fact(DisplayName = "Validator - Valid year and age should pass")]
    [Description("PS-2382 : Validates year and age bounds")]
    public void Validate_ValidYearAndAge_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 65,
            SingleRate = 10.5m,
            JointRate = 15.75m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - Year below 1900 should be invalid")]
    [Description("PS-2382 : Validates year lower bound")]
    public void Validate_YearBelowMinimum_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 1899,
            Age = 65,
            SingleRate = 10m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Year");
    }

    [Fact(DisplayName = "Validator - Year above 2100 should be invalid")]
    [Description("PS-2382 : Validates year upper bound")]
    public void Validate_YearAboveMaximum_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2101,
            Age = 65,
            SingleRate = 10m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Year");
    }

    [Fact(DisplayName = "Validator - Age above 120 should be invalid")]
    [Description("PS-2382 : Validates age upper bound")]
    public void Validate_AgeAboveMaximum_ShouldBeInvalid()
    {
        // Arrange
        var request = new UpdateAnnuityRateRequest
        {
            Year = 2024,
            Age = 121,
            SingleRate = 10m,
            JointRate = 15m
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Age");
    }

    #endregion
}
