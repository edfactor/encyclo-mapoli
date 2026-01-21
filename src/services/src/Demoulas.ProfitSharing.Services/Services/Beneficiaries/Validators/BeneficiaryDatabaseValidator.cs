using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Services.Beneficiaries.Validators;

public class BeneficiaryDatabaseValidator : AbstractValidator<BeneficiaryDatabaseValidationModel>
{
    private readonly IDemographicReaderService _demographicReaderService;

    public BeneficiaryDatabaseValidator(IDemographicReaderService demographicReaderService)
    {
        _demographicReaderService = demographicReaderService;

        RuleFor(x => x.BeneficiaryContactId)
            .MustAsync(async (model, contactId, cancellationToken) =>
            {
                return await model.Context.BeneficiaryContacts
                    .AnyAsync(x => x.Id == contactId, cancellationToken);
            })
            .WithMessage("Beneficiary Contact does not exist");

        RuleFor(x => x.EmployeeBadgeNumber)
            .MustAsync(async (model, badgeNumber, cancellationToken) =>
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(model.Context, false);
                return await demographicQuery
                    .AnyAsync(x => x.BadgeNumber == badgeNumber, cancellationToken);
            })
            .WithMessage("Employee Badge does not exist");
    }

    public static async Task<ValidationResult> ValidateCreateBeneficiaryContactAsync(
        int contactSsn,
        ProfitSharingDbContext ctx,
        CancellationToken cancellationToken)
    {
        var result = new ValidationResult();

        // Check if SSN already exists
        var ssnExists = await ctx.BeneficiaryContacts
            .AnyAsync(x => x.Ssn == contactSsn, cancellationToken);

        if (ssnExists)
        {
            result.Errors.Add(new ValidationFailure(
                nameof(contactSsn),
                "Contact SSN already exists"));
        }

        return result;
    }

    public static async Task<ValidationResult> ValidateBeneficiaryContactExistsAsync(
        int beneficiaryContactId,
        ProfitSharingDbContext ctx,
        CancellationToken cancellationToken)
    {
        var result = new ValidationResult();

        var contactExists = await ctx.BeneficiaryContacts
            .AnyAsync(x => x.Id == beneficiaryContactId, cancellationToken);

        if (!contactExists)
        {
            result.Errors.Add(new ValidationFailure(
                nameof(beneficiaryContactId),
                "Beneficiary Contact does not exist"));
        }

        return result;
    }
}
