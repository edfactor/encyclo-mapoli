using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public sealed class CreateAnnuityRatesRequestValidator : AbstractValidator<CreateAnnuityRatesRequest>
{
    private const decimal MaxRate = 99.9999m;
    private const byte MinimumAge = 67;
    private const byte MaximumAge = 120;

    public CreateAnnuityRatesRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween((short)1900, (short)2100)
            .WithMessage("Year must be between 1900 and 2100.");

        RuleFor(x => x.Rates)
            .NotNull()
            .WithMessage("Rates are required.")
            .Must(ContainFullAgeRange)
            .WithMessage("Rates must include each age between 67 and 120 exactly once.");

        RuleForEach(x => x.Rates).ChildRules(rate =>
        {
            rate.RuleFor(x => x.Age)
                .InclusiveBetween(MinimumAge, MaximumAge)
                .WithMessage("Age must be between 67 and 120.");

            rate.RuleFor(x => x.SingleRate)
                .InclusiveBetween(0m, MaxRate)
                .WithMessage("SingleRate must be between 0 and 99.9999.")
                .Must(HasAtMostFourDecimals)
                .WithMessage("SingleRate can have at most 4 decimal places.");

            rate.RuleFor(x => x.JointRate)
                .InclusiveBetween(0m, MaxRate)
                .WithMessage("JointRate must be between 0 and 99.9999.")
                .Must(HasAtMostFourDecimals)
                .WithMessage("JointRate can have at most 4 decimal places.");
        });
    }

    private static bool ContainFullAgeRange(IReadOnlyList<AnnuityRateInputRequest>? rates)
    {
        if (rates is null || rates.Count == 0)
        {
            return false;
        }

        var ages = rates.Select(r => r.Age).ToList();
        if (ages.Count != ages.Distinct().Count())
        {
            return false;
        }

        var minAge = ages.Min();
        var maxAge = ages.Max();
        if (minAge != MinimumAge || maxAge != MaximumAge)
        {
            return false;
        }

        var expectedCount = MaximumAge - MinimumAge + 1;
        return ages.Count == expectedCount;
    }

    private static bool HasAtMostFourDecimals(decimal value)
    {
        var valueString = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var decimalIndex = valueString.IndexOf('.');

        if (decimalIndex == -1)
        {
            return true;
        }

        var decimalPlaces = 0;
        for (var i = decimalIndex + 1; i < valueString.Length; i++)
        {
            if (char.IsDigit(valueString[i]))
            {
                decimalPlaces++;
            }
        }

        return decimalPlaces <= 4;
    }
}
