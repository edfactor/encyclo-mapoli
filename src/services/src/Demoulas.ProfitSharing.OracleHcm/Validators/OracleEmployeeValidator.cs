using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.OracleHcm.Validators;
public class OracleEmployeeValidator : Validator<OracleEmployee>
{
    private const int MAX_STORE_ID = 899;

    private const string BadAddress = "No address found for employee";
    private const string BadWorkRelationship = "No work relationship found for employee";
    private const string UnknownEmploymentType = "Unknown Employment Type for employee";
    private const string UnknownPayFrequency = "Unknown pay frequency for employee";
    private const string UnknownStoreLocation = "Unknown store location for employee";
    private const string UnknownPayClassification = "Unknown pay classification for employee";
    private const string UnknownDepartment = "Unknown department for employee";
    

    public OracleEmployeeValidator()
    {
        RuleFor(e => e.Address)
            .Must(v => v != null)
            .WithMessage(e=> BadAddress);

        RuleFor(e => e.WorkRelationship)
            .Must(v => v != null)
            .WithMessage(e => BadWorkRelationship);

        RuleFor(e => e.WorkRelationship!.Assignment.GetEmploymentType())
            .Must(v => v is not char.MinValue)
            .WithMessage(e=> UnknownEmploymentType)
            .WithState(e => e.WorkRelationship?.Assignment.FullPartTime)
            .OverridePropertyName("WorkRelationship.Assignment.FullPartTime");

        RuleFor(e => e.WorkRelationship!.Assignment.GetPayFrequency())
            .Must(v => v is not byte.MinValue)
            .WithMessage(e => UnknownPayFrequency)
            .WithState(e => e.WorkRelationship?.Assignment.Frequency)
            .OverridePropertyName("WorkRelationship.Assignment.Frequency");


        RuleFor(e => e.WorkRelationship!.Assignment.LocationCode)
            .Must(v => v < MAX_STORE_ID)
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

    }

    private async Task<bool> ValidatePayClassificationAsync(byte? jobCode, CancellationToken ct)
    {
        var payCacheService = Resolve<IBaseCacheService<PayClassificationResponseCache>>();
        var payClassifications = await payCacheService.GetAllAsync(ct);
        var codes = payClassifications.Select(p => p.Id).ToHashSet();
        return codes.Contains(jobCode ?? 0);
    }

    private Task<bool> ValidateDepartmentIdAsync(byte departmentId, CancellationToken ct)
    {
        var dbContextFactory = Resolve<IProfitSharingDataContextFactory>();
        return dbContextFactory.UseReadOnlyContext(c =>
        {
            return c.Departments.AnyAsync(d=> d.Id == departmentId, ct);
        });
    }
}
