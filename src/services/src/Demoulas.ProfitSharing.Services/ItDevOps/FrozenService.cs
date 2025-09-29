using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Data.Contexts.Interceptor;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Demoulas.ProfitSharing.Services.ItDevOps;

/// <summary>
/// This service contains logic related to getting temporal data for tables with ValidFrom and ValidTo.
/// </summary>
public class FrozenService : IFrozenService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICommitGuardOverride _guardOverride;
    private readonly IServiceProvider _serviceProvider;

    public FrozenService(IProfitSharingDataContextFactory dataContextFactory,
        ICommitGuardOverride guardOverride,
        IServiceProvider serviceProvider)
    {
        _dataContextFactory = dataContextFactory;
        _guardOverride = guardOverride;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Returns a query object representing Demographic data as of a certain date time
    /// </summary>
    /// <param name="ctx">The data context used for querying</param>
    /// <param name="profitYear">Show data as of the last frozen date for a profit year.</param>
    /// <returns></returns>
    internal static IQueryable<Demographic> GetDemographicSnapshot(IProfitSharingDbContext ctx, short profitYear)
    {
        // Use a correlated subquery against FrozenStates to evaluate the window; this avoids duplicating the projection.
        Expression<Func<DemographicHistory, bool>> window = dh =>
            ctx.FrozenStates.Any(fs => fs.ProfitYear == profitYear && fs.IsActive && fs.AsOfDateTime >= dh.ValidFrom && fs.AsOfDateTime < dh.ValidTo);

        return BuildDemographicSnapshot(ctx, window);
    }

    /// <summary>
    /// Returns a query object representing Demographic data as-of an explicit timestamp.
    /// </summary>
    /// <param name="ctx">The data context used for querying</param>
    /// <param name="asOf">Point-in-time for the snapshot (UTC offset preserved)</param>
    /// <returns></returns>
    internal static IQueryable<Demographic> GetDemographicSnapshotAsOf(IProfitSharingDbContext ctx, DateTimeOffset asOf)
    {
        Expression<Func<DemographicHistory, bool>> window = dh => asOf >= dh.ValidFrom && asOf < dh.ValidTo;
        return BuildDemographicSnapshot(ctx, window);
    }

    private static IQueryable<Demographic> BuildDemographicSnapshot(IProfitSharingDbContext ctx, Expression<Func<DemographicHistory, bool>> withinWindow)
    {
        return
            from dh in ctx.DemographicHistories.Where(withinWindow)
#pragma warning disable DSMPS001
            join d in ctx.Demographics on dh.DemographicId equals d.Id
#pragma warning restore DSMPS001
            join dpts in ctx.Departments on dh.DepartmentId equals dpts.Id
            select new Demographic
            {
                Id = dh.DemographicId,
                OracleHcmId = dh.OracleHcmId,
                Ssn = d.Ssn,
                BadgeNumber = dh.BadgeNumber,
                ModifiedAtUtc = dh.ValidFrom,
                StoreNumber = dh.StoreNumber,
                PayClassificationId = dh.PayClassificationId,
                ContactInfo = d.ContactInfo,
                Address = d.Address,
                DateOfBirth = dh.DateOfBirth,
                FullTimeDate = d.FullTimeDate,
                HireDate = dh.HireDate,
                ReHireDate = dh.ReHireDate,
                TerminationDate = dh.TerminationDate,
                DepartmentId = dh.DepartmentId,
                Department = dpts,
                EmploymentTypeId = dh.EmploymentTypeId,
                GenderId = d.GenderId,
                PayFrequencyId = dh.PayFrequencyId,
                TerminationCodeId = dh.TerminationCodeId,
                EmploymentStatusId = dh.EmploymentStatusId,
            };
    }

    /// <summary>
    /// Sets the cutoff data for a particular profit year.  Deactivates any prior "Freezes" for the year.
    /// </summary>
    /// <param name="profitYear">Profit year for which to set the freeze date/time</param>
    /// <param name="asOfDateTime"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<FrozenStateResponse> FreezeDemographics(short profitYear, DateTime asOfDateTime, string? userName, CancellationToken cancellationToken = default)
    {
        var validator = new InlineValidator<short>();

        var thisYear = DateTime.Today.Year;
        validator.RuleFor(r => r)
            .InclusiveBetween((short)(thisYear - 1), (short)thisYear)
            .WithMessage($"ProfitYear must be between {thisYear - 1} and {thisYear}.");


        var duplicateSsnReportService = _serviceProvider.GetRequiredService<IPayrollDuplicateSsnReportService>();
        // Inline async rule to prevent freezing when duplicate SSNs exist.
        validator.RuleFor(r => r)
            .MustAsync(async (_, ct) => !await duplicateSsnReportService.DuplicateSsnExistsAsync(ct))
            .WithMessage("Cannot freeze demographics when duplicate SSNs exist.  Please resolve duplicate SSNs and try again.");

        await validator.ValidateAndThrowAsync(profitYear, cancellationToken);

        using (_guardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _dataContextFactory.UseWritableContext(async ctx =>
            {
                //Inactivate any prior frozen states
                await ctx.FrozenStates.Where(x => x.IsActive).ForEachAsync(x => x.IsActive = false, cancellationToken);

                if (userName == null)
                {
                    userName = "Unknown"; // aka test driven, got through cert validation, but user is undefined.  Only happens during testing.
                }

                //Create new record
                var frozenState = new FrozenState { IsActive = true, ProfitYear = profitYear, AsOfDateTime = asOfDateTime, FrozenBy = userName };
                ctx.FrozenStates.Add(frozenState);

                // Copy the annuity rates from the prior year, if rows don't yet exist.
                var lastYear = profitYear - 1;
                if (!(await ctx.AnnuityRates.AnyAsync(x => x.Year == profitYear, cancellationToken)) &&
                    (await ctx.AnnuityRates.AnyAsync(x => x.Year == lastYear, cancellationToken)))
                {
                    var cloneFromLastYearRates = await ctx.AnnuityRates
                        .Where(x => x.Year == lastYear)
                        .Select(x => new AnnuityRate
                        {
                            Year = profitYear,
                            Age = x.Age,
                            SingleRate = x.SingleRate,
                            JointRate = x.JointRate,
                            CreatedAtUtc = DateTimeOffset.UtcNow,
                            UserName = userName
                        })
                        .ToListAsync(cancellationToken);
                    ctx.AnnuityRates.AddRange(cloneFromLastYearRates);
                }

                await ctx.SaveChangesAsync(cancellationToken);

                return new FrozenStateResponse
                {
                    Id = frozenState.Id,
                    ProfitYear = frozenState.ProfitYear,
                    FrozenBy = frozenState.FrozenBy,
                    AsOfDateTime = frozenState.AsOfDateTime,
                    IsActive = frozenState.IsActive,
                    CreatedDateTime = frozenState.CreatedDateTime
                };
            }, cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves a list of frozen demographic states.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of 
    /// <see cref="FrozenStateResponse"/> objects representing the frozen demographic states.
    /// </returns>
    public Task<PaginatedResponseDto<FrozenStateResponse>> GetFrozenDemographics(SortedPaginationRequestDto request, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            //Inactivate any prior frozen states
            return ctx.FrozenStates.Select(f => new FrozenStateResponse
            {
                Id = f.Id,
                ProfitYear = f.ProfitYear,
                FrozenBy = f.FrozenBy,
                AsOfDateTime = f.AsOfDateTime,
                IsActive = f.IsActive,
                CreatedDateTime = f.CreatedDateTime
            }).ToPaginationResultsAsync(request, cancellationToken);
        });
    }

    public Task<FrozenStateResponse> GetActiveFrozenDemographic(CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            //Inactivate any prior frozen states
            var frozen = await ctx.FrozenStates.Where(f => f.IsActive).Select(f => new FrozenStateResponse
            {
                Id = f.Id,
                ProfitYear = f.ProfitYear,
                FrozenBy = f.FrozenBy,
                AsOfDateTime = f.AsOfDateTime,
                IsActive = f.IsActive,
                CreatedDateTime = f.CreatedDateTime
            }).FirstOrDefaultAsync(cancellationToken);

            return frozen ?? new FrozenStateResponse
            {
                Id = 0,
                ProfitYear = (short)DateTime.Today.Year,
                CreatedDateTime = ReferenceData.DsmMinValue.ToDateTime(TimeOnly.MinValue),
                AsOfDateTime = DateTimeOffset.UtcNow,
                IsActive = false
            };
        });
    }

}
