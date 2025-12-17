using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation.Legacy;

public sealed class LegacySaveProfitSharingAdjustmentsRequestValidator : AbstractValidator<SaveProfitSharingAdjustmentsRequest>
{
    private const int MaxRows = 18;

    public LegacySaveProfitSharingAdjustmentsRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .Must(y => y is >= 1900 and <= 2500)
            .WithMessage("ProfitYear must be between 1900 and 2500.");

        RuleFor(x => x.BadgeNumber)
            .GreaterThan(0)
            .WithMessage("BadgeNumber must be greater than zero.");

        RuleFor(x => x.SequenceNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SequenceNumber must be zero or greater.");

        RuleFor(x => x.Rows)
            .NotNull()
            .Must(x => x.Count is > 0 and <= MaxRows)
            .WithMessage($"Rows must contain between 1 and {MaxRows} rows.");

        RuleForEach(x => x.Rows).ChildRules(row =>
        {
            row.RuleFor(r => r.RowNumber)
                .InclusiveBetween(1, MaxRows)
                .WithMessage($"RowNumber must be between 1 and {MaxRows}.");

            row.RuleFor(r => r.ProfitCodeId)
                .Equal((byte)0)
                .WithMessage("ProfitCodeId must be 0.");

            row.RuleFor(r => r.Comment)
                .NotNull()
                .WithMessage("Comment is required.");
        });

        RuleFor(x => x.Rows)
            .Must(rows => rows.Select(r => r.RowNumber).Distinct().Count() == rows.Count)
            .WithMessage("Duplicate RowNumber values are not allowed.");
    }
}
