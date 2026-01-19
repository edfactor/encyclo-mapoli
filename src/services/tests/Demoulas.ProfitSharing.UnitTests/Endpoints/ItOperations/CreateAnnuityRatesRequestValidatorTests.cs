using System.ComponentModel;
using System.Linq;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Validators;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;

/// <summary>
/// Unit tests for CreateAnnuityRatesRequestValidator.
/// </summary>
[Collection("Validation Tests")]
public sealed class CreateAnnuityRatesRequestValidatorTests
{
    private readonly CreateAnnuityRatesRequestValidator _validator = new();

    [Fact(DisplayName = "Validator - Full age range should be valid")]
    [Description("PS-2454 : Validates full age range 67-120")]
    public void Validate_FullAgeRange_ShouldBeValid()
    {
        var request = new CreateAnnuityRatesRequest
        {
            Year = 2025,
            Rates = BuildRates()
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Validator - Missing age should be invalid")]
    [Description("PS-2454 : Detects missing age in rate list")]
    public void Validate_MissingAge_ShouldBeInvalid()
    {
        var rates = BuildRates();
        rates.RemoveAt(0);

        var request = new CreateAnnuityRatesRequest
        {
            Year = 2025,
            Rates = rates
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
    }

    [Fact(DisplayName = "Validator - Duplicate age should be invalid")]
    [Description("PS-2454 : Detects duplicate age in rate list")]
    public void Validate_DuplicateAge_ShouldBeInvalid()
    {
        var rates = BuildRates();
        rates.Add(new AnnuityRateInputRequest
        {
            Age = 67,
            SingleRate = 10.0000m,
            JointRate = 15.0000m
        });

        var request = new CreateAnnuityRatesRequest
        {
            Year = 2025,
            Rates = rates
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
    }

    [Fact(DisplayName = "Validator - Rate with more than 4 decimals should be invalid")]
    [Description("PS-2454 : Validates decimal precision for rates")]
    public void Validate_RateWithFiveDecimals_ShouldBeInvalid()
    {
        var rates = BuildRates();
        rates[0] = new AnnuityRateInputRequest
        {
            Age = 67,
            SingleRate = 10.00001m,
            JointRate = 15.0000m
        };

        var request = new CreateAnnuityRatesRequest
        {
            Year = 2025,
            Rates = rates
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
    }

    [Fact(DisplayName = "Validator - Year outside range should be invalid")]
    [Description("PS-2454 : Validates year bounds")]
    public void Validate_YearOutsideRange_ShouldBeInvalid()
    {
        var request = new CreateAnnuityRatesRequest
        {
            Year = 1800,
            Rates = BuildRates()
        };

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
    }

    private static List<AnnuityRateInputRequest> BuildRates()
    {
        return Enumerable.Range(67, 54)
            .Select(age => new AnnuityRateInputRequest
            {
                Age = (byte)age,
                SingleRate = 10.0000m,
                JointRate = 15.0000m
            })
            .ToList();
    }
}
