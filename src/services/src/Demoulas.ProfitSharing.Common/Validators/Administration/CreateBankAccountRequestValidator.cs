using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Demoulas.ProfitSharing.Common.Validators.Administration;

public sealed partial class CreateBankAccountRequestValidator : AbstractValidator<CreateBankAccountRequest>
{
    [GeneratedRegex(@"^\d{9}$")]
    private static partial Regex RoutingNumberRegex();

    public CreateBankAccountRequestValidator()
    {
        RuleFor(x => x.BankId)
            .GreaterThan(0)
            .WithMessage("Bank ID must be greater than zero.");

        RuleFor(x => x.RoutingNumber)
            .NotEmpty()
            .WithMessage("Routing number is required.")
            .Must(BeValidRoutingNumber)
            .WithMessage("Routing number must be exactly 9 digits.");

        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .WithMessage("Account number is required.")
            .MaximumLength(34)
            .WithMessage("Account number cannot exceed 34 characters.");

        RuleFor(x => x.ServicingFedRoutingNumber)
            .Must(BeValidRoutingNumber)
            .WithMessage("Servicing Fed routing number must be exactly 9 digits.")
            .When(x => !string.IsNullOrWhiteSpace(x.ServicingFedRoutingNumber));

        RuleFor(x => x.ServicingFedAddress)
            .MaximumLength(200)
            .WithMessage("Servicing Fed address cannot exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ServicingFedAddress));

        RuleFor(x => x.FedwireTelegraphicName)
            .MaximumLength(50)
            .WithMessage("Fedwire telegraphic name cannot exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FedwireTelegraphicName));

        RuleFor(x => x.FedwireLocation)
            .MaximumLength(100)
            .WithMessage("Fedwire location cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FedwireLocation));

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.EffectiveDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Effective date cannot be in the future.")
            .When(x => x.EffectiveDate.HasValue);
    }

    private static bool BeValidRoutingNumber(string? routingNumber)
    {
        if (string.IsNullOrWhiteSpace(routingNumber))
        {
            return false;
        }

        return RoutingNumberRegex().IsMatch(routingNumber);
    }
}
