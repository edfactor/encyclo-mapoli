using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.Services.Caching.HostedServices;
using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.OracleHcm.Validators;

public sealed class OracleEmployeeValidator : Validator<OracleEmployee>
{
    private readonly NameItemValidator _nameItemValidator = new NameItemValidator();
    private readonly AddressItemValidator _addressValidator = new AddressItemValidator();

    private readonly IBaseCacheService<LookupTableCache<string>> _accountCache;
    private readonly IBaseCacheService<LookupTableCache<byte>> _depCache;
    private const int MaxStoreId = 10_000;

    private const string BadAddress = "No address found for employee";
    private const string BadWorkRelationship = "No work relationship found for employee";
    private const string UnknownEmploymentType = "Unknown Employment Type for employee";
    private const string UnknownPayFrequency = "Unknown pay frequency for employee";
    private const string UnknownStoreLocation = "Unknown store location for employee";
    private const string UnknownPayClassification = "Unknown pay classification for employee";
    private const string UnknownDepartment = "Unknown department for employee";


    public OracleEmployeeValidator(
        [FromKeyedServices(nameof(PayClassificationHostedService))]
    IBaseCacheService<LookupTableCache<string>> accountCache,
        [FromKeyedServices(nameof(DepartmentHostedService))]
        IBaseCacheService<LookupTableCache<byte>> depCache)
    {
        _accountCache = accountCache;
        _depCache = depCache;

        RuleFor(e => e.Address)
            .Must(v => v != null)
            .WithMessage(e => BadAddress);

        // Use the AddressRequestDtoValidator to validate the Address object
        RuleFor(e => e.Address)
            .Custom((address, context) =>
            {
                if (address != null)
                {
                    ValidationResult result = _addressValidator.Validate(address);
                    foreach (ValidationFailure? error in result.Errors)
                    {
                        context.AddFailure(error);
                    }
                }
            });

        RuleFor(e => e.Name)
            .Custom((nameItem, context) =>
            {
                if (nameItem != null)
                {
                    ValidationResult result = _nameItemValidator.Validate(nameItem);
                    foreach (ValidationFailure? error in result.Errors)
                    {
                        context.AddFailure(error);
                    }
                }
            });

        RuleFor(e => e.WorkRelationship)
            .Must(v => v != null)
            .WithMessage(e => BadWorkRelationship);

        RuleFor(e => e.WorkRelationship!.Assignment.GetEmploymentType())
            .Must(v => v is not char.MinValue)
            .WithMessage(e => UnknownEmploymentType)
            .WithState(e => e.WorkRelationship?.Assignment.FullPartTime)
            .OverridePropertyName("WorkRelationship.Assignment.FullPartTime");

        RuleFor(e => e.WorkRelationship!.Assignment.GetPayFrequency())
            .Must(v => v is not byte.MinValue)
            .WithMessage(e => UnknownPayFrequency)
            .WithState(e => e.WorkRelationship?.Assignment.Frequency)
            .OverridePropertyName("WorkRelationship.Assignment.Frequency");


        RuleFor(e => e.WorkRelationship!.Assignment.LocationCode)
            .Must(v => v < MaxStoreId)
            .WithMessage(e => UnknownStoreLocation)
            .WithState(e => e.WorkRelationship?.Assignment.LocationCode);


        RuleFor(e => e.WorkRelationship!.Assignment.JobCode)
            .MustAsync(ValidatePayClassificationAsync)
            .WithMessage(e => UnknownPayClassification)
            .WithState(e => e.WorkRelationship?.Assignment.JobCode);

        RuleFor(e => e.WorkRelationship!.Assignment.GetDepartmentId())
            .MustAsync(ValidateDepartmentIdAsync)
            .WithMessage(e => UnknownDepartment)
            .WithState(e => e.WorkRelationship?.Assignment.PositionCode)
            .OverridePropertyName("WorkRelationship.Assignment.PositionCode");


        RuleFor(x => x.BadgeNumber)
            .InclusiveBetween(1, 9_999_999).WithMessage("BadgeNumber must be a 7-digit number.");

        RuleFor(x => x.PersonId)
            .NotEmpty()
            .InclusiveBetween(1, 999_999_999_999_999).WithMessage("OracleHcmId(PersonId) must be a 15-digit number.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("DateOfBirth is required.");
    }

    private async Task<bool> ValidatePayClassificationAsync(string? jobCode, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(jobCode))
        {
            return false;
        }
        ISet<LookupTableCache<string>> lookup = await _accountCache.GetAllAsync(ct).ConfigureAwait(false);
        HashSet<string> codes = lookup.Select(p => p.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return codes.Contains(jobCode);
    }

    private async Task<bool> ValidateDepartmentIdAsync(byte departmentId, CancellationToken ct)
    {
        ISet<LookupTableCache<byte>> lookup = await _depCache.GetAllAsync(ct).ConfigureAwait(false);
        HashSet<byte> codes = lookup.Select(p => p.Id).ToHashSet();
        return codes.Contains(departmentId);
    }
}
