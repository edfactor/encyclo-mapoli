using System.Linq;
using System.Threading;
using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.InternalEntities;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Validators;
public class OracleEmployeeValidator : Validator<OracleEmployee>
{
    private const int MAX_STORE_ID = 899;

    private const string BadAddress = "No address found for employee with BadgeNumber '{BadgeNumber}'";
    private const string BadWorkRelationship = "No work relationship found for employee with BadgeNumber '{BadgeNumber}'";
    private const string UnknownEmploymentType = "Unknown Employment Type for employee with BadgeNumber {BadgeNumber}. Value received is '{FullPartTime}'";
    private const string UnknownPayClassification = "Unknown pay classification for employee with BadgeNumber {BadgeNumber}. Value received is '{JobCode}' ";
    private const string UnknownDepartment = "Unknown department for employee with BadgeNumber {BadgeNumber}. Value received is '{PositionCode}' ";
    private const string UnknownPayFrequency = "Unknown pay frequency for employee with BadgeNumber {BadgeNumber}. Value received is '{Frequency}'";
    private const string UnknownStoreLocation = "Unknown store location for employee with BadgeNumber {BadgeNumber}. Value received is '{LocationCode}'";


    public OracleEmployeeValidator()
    {
        RuleFor(e => e.Address)
            .Must(v => v != null)
            .WithMessage(e=> BadAddress.ReplaceNamedParams(e.BadgeNumber));

        RuleFor(e => e.WorkRelationship)
            .Must(v => v != null )
            .WithMessage(e=> BadWorkRelationship.ReplaceNamedParams(e.BadgeNumber));

        RuleFor(e => e.WorkRelationship!.Assignment.GetEmploymentType())
            .Must(v => v is not char.MinValue)
            .WithMessage(e=> UnknownEmploymentType.ReplaceNamedParams(e.BadgeNumber, e.WorkRelationship?.Assignment.FullPartTime));

        RuleFor(e => e.WorkRelationship!.Assignment.GetPayFrequency())
            .Must(v => v is not byte.MinValue)
            .WithMessage(e => UnknownPayFrequency.ReplaceNamedParams(e.BadgeNumber, e.WorkRelationship?.Assignment.Frequency));


        RuleFor(e => e.WorkRelationship!.Assignment.LocationCode)
            .Must(v => v < MAX_STORE_ID)
            .WithMessage(e => UnknownStoreLocation.ReplaceNamedParams(e.BadgeNumber, e.WorkRelationship?.Assignment.LocationCode));
        

        RuleFor(e => e.WorkRelationship!.Assignment.JobCode)
            .MustAsync(ValidatePayClassificationAsync)
            .WithMessage(e => UnknownPayClassification.ReplaceNamedParams(e.BadgeNumber, e.WorkRelationship?.Assignment.JobCode));

        RuleFor(e => e.WorkRelationship!.Assignment.GetDepartmentId())
            .MustAsync(ValidateDepartmentIdAsync)
            .WithMessage(e => UnknownDepartment.ReplaceNamedParams(e.BadgeNumber, e.WorkRelationship?.Assignment.PositionCode));

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
