using System.Linq.Expressions;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.ItDevOps;

/// <summary>
/// This service contains logic related to getting temporal data for tables with ValidFrom and ValidTo.
/// </summary>
public class FrozenService : IFrozenService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICommitGuardOverride _guardOverride;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDistributedCache _distributedCache;
    private readonly INavigationService _navigationService;
    private readonly ILogger<FrozenService>? _logger;

    // Cache key pattern for frozen demographics endpoint - matches FastEndpoints output cache
    private const string FrozenDemographicsCacheKeyPrefix = "FEEndpointCache:";

    public FrozenService(IProfitSharingDataContextFactory dataContextFactory,
        ICommitGuardOverride guardOverride,
        IServiceProvider serviceProvider,
        IDistributedCache distributedCache,
        INavigationService navigationService,
        ILogger<FrozenService>? logger = null)
    {
        _dataContextFactory = dataContextFactory;
        _guardOverride = guardOverride;
        _serviceProvider = serviceProvider;
        _distributedCache = distributedCache;
        _navigationService = navigationService;
        _logger = logger;
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
                ContactInfo = new ContactInfo
                {
                    FirstName = dh.FirstName ?? string.Empty,
                    LastName = dh.LastName ?? string.Empty,
                    MiddleName = dh.MiddleName,
                    PhoneNumber = dh.PhoneNumber,
                    MobileNumber = dh.MobileNumber,
                    EmailAddress = dh.EmailAddress,
                    FullName = DtoCommonExtensions.ComputeFullNameWithInitial(dh.LastName ?? string.Empty, dh.FirstName ?? string.Empty, dh.MiddleName)
                },
                Address = new Address
                {
                    Street = dh.Street ?? string.Empty,
                    Street2 = dh.Street2,
                    City = dh.City ?? string.Empty,
                    State = dh.State ?? string.Empty,
                    PostalCode = dh.PostalCode ?? string.Empty,
                    CountryIso = "US"
                },
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
    public async Task<FrozenStateResponse> FreezeDemographics(short profitYear, DateTime asOfDateTime, string? userName = "Unknown", CancellationToken cancellationToken = default)
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
            var result = await _dataContextFactory.UseWritableContext(async ctx =>
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

            // Bust the GetFrozenDemographicsEndpoint output cache after successful freeze
            // FastEndpoints uses a prefix pattern for output cache keys
            await BustGetFrozenDemographicsCacheAsync(cancellationToken);

            // Reset all navigation statuses to "Not Started" when creating a new freeze point
            // This ensures year-end workflows restart cleanly after demographics are frozen
            // Delegates to NavigationService for proper separation of concerns
            await _navigationService.ResetAllStatusesToNotStartedAsync(cancellationToken);

            return result;
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
        }, cancellationToken);
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
        }, cancellationToken);
    }

    /// <summary>
    /// Busts the output cache for GetFrozenDemographicsEndpoint by removing all cached entries
    /// that match the FastEndpoints cache key pattern for this endpoint.
    /// </summary>
    /// <remarks>
    /// FastEndpoints output cache stores entries with keys that start with "FEEndpointCache:"
    /// followed by the endpoint path and query parameters. Since IDistributedCache doesn't support
    /// pattern-based removal, we need to target specific cache keys or use cache tags.
    ///
    /// For now, we attempt to remove common cache key patterns. A more robust solution would be
    /// to use cache tags (if supported by the distributed cache provider) or implement a cache
    /// key tracking mechanism.
    /// </remarks>
    private async Task BustGetFrozenDemographicsCacheAsync(CancellationToken cancellationToken)
    {
        try
        {
            // FastEndpoints generates cache keys like: "FEEndpointCache:/api/it-operations/frozen?page=1&pageSize=10&..."
            // Since we can't pattern-match with IDistributedCache, we'll remove common pagination combinations
            // This is a pragmatic approach - in production you may want to use cache tags or Redis pattern deletion

            var endpointPath = "/api/it-operations/frozen";
            var commonPageSizes = new[] { 10, 25, 50, 100 };
            var maxPages = 5; // Bust first 5 pages of common page sizes

            var cacheKeysToRemove = new List<string>();

            // Generate cache keys for common pagination combinations
            foreach (var pageSize in commonPageSizes)
            {
                for (var page = 1; page <= maxPages; page++)
                {
                    // FastEndpoints cache key format (simplified - actual format may include more parameters)
                    var cacheKey = $"{FrozenDemographicsCacheKeyPrefix}{endpointPath}?page={page}&pageSize={pageSize}";
                    cacheKeysToRemove.Add(cacheKey);
                }
            }

            // Also bust the default/first page without explicit parameters
            cacheKeysToRemove.Add($"{FrozenDemographicsCacheKeyPrefix}{endpointPath}");
            cacheKeysToRemove.Add($"{FrozenDemographicsCacheKeyPrefix}{endpointPath}?");

            // Remove all identified cache keys
            foreach (var key in cacheKeysToRemove)
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }

            _logger?.LogInformation("Busted GetFrozenDemographicsEndpoint cache ({KeyCount} keys removed)", cacheKeysToRemove.Count);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the freeze operation if cache busting fails
            _logger?.LogError(ex, "Failed to bust GetFrozenDemographicsEndpoint cache");
        }
    }

}

