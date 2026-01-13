using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators.Administration;

public sealed class UpdateBankRequestValidator : AbstractValidator<UpdateBankRequest>
{
    public UpdateBankRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Bank ID must be greater than zero.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Bank name is required.")
            .MaximumLength(200)
            .WithMessage("Bank name cannot exceed 200 characters.");

        RuleFor(x => x.OfficeType)
            .MaximumLength(50)
            .WithMessage("Office type cannot exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.OfficeType));

        RuleFor(x => x.City)
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.State)
            .Length(2)
            .WithMessage("State must be a 2-character abbreviation.")
            .When(x => !string.IsNullOrWhiteSpace(x.State));

        RuleFor(x => x.Phone)
            .MaximumLength(24)
            .WithMessage("Phone cannot exceed 24 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Status)
            .MaximumLength(24)
            .WithMessage("Status cannot exceed 24 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }
}
