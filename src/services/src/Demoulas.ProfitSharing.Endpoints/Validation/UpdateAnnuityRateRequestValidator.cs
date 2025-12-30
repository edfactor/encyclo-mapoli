using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation;

public sealed class UpdateAnnuityRateRequestValidator : AbstractValidator<UpdateAnnuityRateRequest>
{
    private const decimal MaxRate = 99.9999m;

    public UpdateAnnuityRateRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween((short)1900, (short)2100)
            .WithMessage("Year must be between 1900 and 2100.");

        RuleFor(x => x.Age)
            .InclusiveBetween((byte)0, (byte)120)
            .WithMessage("Age must be between 0 and 120.");

        RuleFor(x => x.SingleRate)
            .InclusiveBetween(0m, MaxRate)
            .WithMessage("SingleRate must be between 0 and 99.9999.")
            .Must(rate => HasAtMostFourDecimals(rate))
            .WithMessage("SingleRate can have at most 4 decimal places.");

        RuleFor(x => x.JointRate)
            .InclusiveBetween(0m, MaxRate)
            .WithMessage("JointRate must be between 0 and 99.9999.")
            .Must(rate => HasAtMostFourDecimals(rate))
            .WithMessage("JointRate can have at most 4 decimal places.");
    }

    private static bool HasAtMostFourDecimals(decimal value)
    {
        // Convert to string and count decimal places
        // This avoids floating-point precision issues
        var valueString = value.ToString();
        var decimalIndex = valueString.IndexOf('.');

        if (decimalIndex == -1)
        {
            // No decimal point, so 0 decimal places
            return true;
        }

        var decimalPlaces = valueString.Length - decimalIndex - 1;
        return decimalPlaces <= 4;
    }
}
