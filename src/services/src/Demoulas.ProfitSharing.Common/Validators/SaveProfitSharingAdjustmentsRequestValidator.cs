using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// FluentValidation validator for SaveProfitSharingAdjustmentsRequest.
/// </summary>
public sealed class SaveProfitSharingAdjustmentsRequestValidator : Validator<SaveProfitSharingAdjustmentsRequest>
{
    private const int MaxRows = 18;

    private readonly IEmployeeLookupService _employeeLookupService;

    public SaveProfitSharingAdjustmentsRequestValidator(IEmployeeLookupService employeeLookupService)
    {
        _employeeLookupService = employeeLookupService;

        RuleFor(x => x.ProfitYear)
            .Must(y => y is >= 1900 and <= 2500)
            .WithMessage("ProfitYear must be between 1900 and 2500.");

        RuleFor(x => x.BadgeNumber)
            .GreaterThan(0)
            .WithMessage("BadgeNumber must be greater than zero.");

        RuleFor(x => x.SequenceNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SequenceNumber must be zero or greater.");

        RuleFor(x => x)
            .MustAsync(async (req, ct) =>
            {
                var dob = await _employeeLookupService.GetDateOfBirthAsync(req.BadgeNumber, ct);
                if (dob is null)
                {
                    // If the employee is not found or DOB is missing, let the service layer decide.
                    return true;
                }

                var asOf = new DateOnly(req.ProfitYear, 12, 31);
                var age = CalculateAgeYears(dob.Value, asOf);
                return age <= 22;
            })
            .WithName(nameof(SaveProfitSharingAdjustmentsRequest.ProfitYear))
            .WithMessage("Member age must be 22 or younger for the selected ProfitYear.");

        RuleFor(x => x.Rows)
            .NotNull()
            .Must(x => x.Count is > 0 and <= MaxRows)
            .WithMessage($"Rows must contain between 1 and {MaxRows} rows.");

        RuleForEach(x => x.Rows).ChildRules(row =>
        {
            row.RuleFor(r => r.RowNumber)
                .InclusiveBetween(1, MaxRows)
                .WithMessage($"RowNumber must be between 1 and {MaxRows}.");

            row.RuleFor(r => r.ProfitYearIteration)
                .Must(v => v is 0 or 3)
                .WithMessage("ProfitYearIteration (EXT) must be 0 or 3.");

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

    private static int CalculateAgeYears(DateOnly dateOfBirth, DateOnly asOf)
    {
        var years = asOf.Year - dateOfBirth.Year;
        if (asOf < dateOfBirth.AddYears(years))
        {
            years--;
        }

        return years;
    }
}
