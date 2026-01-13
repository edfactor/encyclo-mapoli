using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Validators;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;

/// <summary>
/// Unit tests for UpdateAnnuityRateRequestValidator to verify decimal place validation.
/// Tests ensure that rates with up to 4 decimal places are accepted.
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

    #endregion
}
