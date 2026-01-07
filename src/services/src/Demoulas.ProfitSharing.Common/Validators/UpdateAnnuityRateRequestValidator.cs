using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

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
        // Convert to string using invariant culture to ensure consistent decimal separator
        var valueString = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var decimalIndex = valueString.IndexOf('.');

        if (decimalIndex == -1)
        {
            // No decimal point, so 0 decimal places
            return true;
        }

        // Count digit characters after the decimal point
        int decimalPlaces = 0;
        for (int i = decimalIndex + 1; i < valueString.Length; i++)
        {
            if (char.IsDigit(valueString[i]))
            {
                decimalPlaces++;
            }
        }

        return decimalPlaces <= 4;
    }
}
