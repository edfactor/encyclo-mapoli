using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Beneficiaries.Validators;

public class BeneficiaryDatabaseValidator
{
    private readonly IDemographicReaderService _demographicReaderService;

    public BeneficiaryDatabaseValidator(IDemographicReaderService demographicReaderService)
    {
        _demographicReaderService = demographicReaderService;
    }

    public async Task<ValidationResult> ValidateCreateBeneficiaryAsync(
        int beneficiaryContactId,
        int employeeBadgeNumber,
        ProfitSharingDbContext ctx,
        CancellationToken cancellationToken)
    {
        var result = new ValidationResult();

        // Validate beneficiary contact exists
        var beneficiaryContactExists = await ctx.BeneficiaryContacts
            .AnyAsync(x => x.Id == beneficiaryContactId, cancellationToken);

        if (!beneficiaryContactExists)
        {
            result.Errors.Add(new ValidationFailure(
                nameof(beneficiaryContactId),
                "Beneficiary Contact does not exist"));
        }

        // Validate employee badge exists
        var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
        var demographicExists = await demographicQuery
            .AnyAsync(x => x.BadgeNumber == employeeBadgeNumber, cancellationToken);

        if (!demographicExists)
        {
            result.Errors.Add(new ValidationFailure(
                nameof(employeeBadgeNumber),
                "Employee Badge does not exist"));
        }

        return result;
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
