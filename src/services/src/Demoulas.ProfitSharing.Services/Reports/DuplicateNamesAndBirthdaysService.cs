using Demoulas.Common.Caching.Interfaces;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class DuplicateNamesAndBirthdaysService : IDuplicateNamesAndBirthdaysService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly ILogger<DuplicateNamesAndBirthdaysService> _logger;
    private readonly TotalService _totalService;
    private readonly IHostEnvironment _host;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IServiceProvider _serviceProvider;

    public DuplicateNamesAndBirthdaysService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory factory,
        ICalendarService calendarService,
        TotalService totalService,
        IHostEnvironment host,
        IDemographicReaderService demographicReaderService,
        IServiceProvider serviceProvider)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _host = host;
        _demographicReaderService = demographicReaderService;
        _serviceProvider = serviceProvider;
        _logger = factory.CreateLogger<DuplicateNamesAndBirthdaysService>();
    }

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdaysAsync(
        ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DUPLICATE NAMES AND BIRTHDAYS"))
        {
            var dict = new Dictionary<int, byte>();
            var dupInfo = new HashSet<DemographicMatchDto>();
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                IQueryable<DemographicMatchDto> dupNameSlashDateOfBirth;
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                // Fallback for mocked (in-memory) db context which does not support raw SQL
                if (_host.IsTestEnvironment())
                {
                    _logger.LogWarning(1, "Inside the test environment");
                    dupNameSlashDateOfBirth = demographics
                        .Include(d => d.ContactInfo)
                        .Select(d => new DemographicMatchDto { FullName = d.ContactInfo.FullName!, MatchedId = d.Id });
                }
                else
                {
                    _logger.LogWarning(2, "Outside the test environment");
                    string dupQuery =
                        @"WITH FILTERED_DEMOGRAPHIC AS (SELECT /*+ MATERIALIZE */ ID, FULL_NAME, DATE_OF_BIRTH, BADGE_NUMBER, SSN
                              FROM DEMOGRAPHIC)
SELECT /*+ USE_HASH(p1 p2) */ p1.FULL_NAME as FullName, p2.BADGE_NUMBER MatchedId
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id <> p2.Id /* Avoid self-joins and duplicate pairs */
                  AND SUBSTR(p1.FULL_NAME,1,1) = SUBSTR(p2.FULL_NAME,1,1) -- Eliminate by first letter before using more CPU intensive functions
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */
UNION ALL
SELECT /*+ USE_HASH(p1 p2) */ p2.FULL_NAME as FullName, p1.BADGE_NUMBER MatchedId
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id <> p2.Id /* Avoid self-joins and duplicate pairs */
                  AND SUBSTR(p1.FULL_NAME,1,1) = SUBSTR(p2.FULL_NAME,1,1) -- Eliminate by first letter before using more CPU intensive functions
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )                  
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */";

                    dupNameSlashDateOfBirth = ctx.Database
                        .SqlQueryRaw<DemographicMatchDto>(dupQuery);
                    _logger.LogWarning(3, "Got value in dupNmaeSlashDateOfBirth");
                }

                dupInfo = await dupNameSlashDateOfBirth
                    .Where(d => !string.IsNullOrEmpty(d.FullName))
                    .ToHashSetAsync(cancellationToken);

                var names = dupInfo.Select(x => x.FullName).ToHashSet();
                var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
                _logger.LogWarning(4, "Running the linq query to get from demographics");
                var query = from dem in demographics.Include(d => d.EmploymentStatus)
                            join ppLj in ctx.PayProfits on new { DemographicId = dem.Id, req.ProfitYear } equals new
                            {
                                ppLj.DemographicId,
                                ppLj.ProfitYear
                            } into tmpPayProfit
                            from pp in tmpPayProfit.DefaultIfEmpty()
                            join b in _totalService.GetTotalBalanceSet(ctx, req.ProfitYear) on dem.Ssn equals b.Ssn into tmpBalance
                            from bal in tmpBalance.DefaultIfEmpty()
                            join yos in _totalService.GetYearsOfService(ctx, req.ProfitYear, calInfo.FiscalEndDate) on dem.Ssn equals yos.Ssn into tmpYos
                            from yos in tmpYos.DefaultIfEmpty()
                            where dem.ContactInfo.FullName != null && names.Contains(dem!.ContactInfo!.FullName!)
                            select new
                            {
                                dem.BadgeNumber,
                                dem.Ssn,
                                Name = dem.ContactInfo.FullName,
                                dem.DateOfBirth,
                                Address = dem.Address.Street,
                                dem.Address.City,
                                dem.Address.State,
                                dem.Address.PostalCode,
                                dem.Address.CountryIso,
                                dem.HireDate,
                                dem.TerminationDate,
                                dem.EmploymentStatusId,
                                EmploymentStatusName = dem.EmploymentStatus!.Name,
                                dem.StoreNumber,
                                HoursCurrentYear = pp != null ? pp.CurrentHoursYear : 0,
                                IncomeCurrentYear = pp != null ? pp.CurrentIncomeYear : 0,
                                NetBalance = bal != null ? bal.TotalAmount : 0,
                                Years = yos != null ? yos.Years : (byte)0,
                                dem.PayFrequencyId,
                            };
                _logger.LogWarning(5, "Calling ToPaginationResultsAsync");
                return await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            }, cancellationToken);

            // Load fake SSNs before projection (so we can mark before masking)
            var fakeSsns = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                return await ctx.FakeSsns
                    .TagWith("GetFakeSsns-DuplicateNamesAndBirthdays")
                    .Select(f => f.Ssn)
                    .ToHashSetAsync(cancellationToken);
            }, cancellationToken);

            // Project and mark IsFakeSsn BEFORE masking SSN
            var projectedResults = results.Results.Select(r => new DuplicateNamesAndBirthdaysResponse
            {
                BadgeNumber = r.BadgeNumber,
                Ssn = r.Ssn.MaskSsn(),
                Name = r.Name,
                DateOfBirth = r.DateOfBirth,
                Address = r.Address,
                City = r.City,
                State = r.State,
                PostalCode = r.PostalCode,
                CountryIso = r.CountryIso,
                Years = r.Years,
                HireDate = r.HireDate,
                TerminationDate = r.TerminationDate,
                Status = r.EmploymentStatusId,
                StoreNumber = r.StoreNumber,
                Count = GetDictValue(dict, r.BadgeNumber),
                NetBalance = r.NetBalance ?? 0,
                HoursCurrentYear = r.HoursCurrentYear,
                IncomeCurrentYear = r.IncomeCurrentYear,
                EmploymentStatusName = r.EmploymentStatusName ?? "",
                IsExecutive = r.PayFrequencyId == PayFrequency.Constants.Monthly,
                IsFakeSsn = fakeSsns.Contains(r.Ssn) // Mark based on unmasked SSN before MaskSsn() is applied
            }).ToList();

            foreach (var r in projectedResults)
            {
                r.Count = dupInfo.Count(x => x.MatchedId == r.BadgeNumber);
            }
            _logger.LogWarning(6, "Calling GetYearStartAndEndAccountingDatesAsync function");
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
            _logger.Log(LogLevel.Debug, "Final return statement");
            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = calInfo.FiscalBeginDate,
                EndDate = calInfo.FiscalEndDate,
                ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                Response = new PaginatedResponseDto<DuplicateNamesAndBirthdaysResponse>
                {
                    Total = results.Total,
                    Results = projectedResults
                }
            };
        }
    }

    public async Task<DuplicateNamesAndBirthdaysCachedResponse?> GetCachedDuplicateNamesAndBirthdaysAsync(
        ProfitYearRequest request, CancellationToken cancellationToken = default)
    {
        if (_host.IsTestEnvironment())
        {
            var testData = await GetDuplicateNamesAndBirthdaysAsync(request, cancellationToken);
            return new DuplicateNamesAndBirthdaysCachedResponse
            {
                AsOfDate = DateTimeOffset.UtcNow,
                Data = new PaginatedResponseDto<DuplicateNamesAndBirthdaysResponse>
                {
                    Total = testData.Response.Total,
                    Results = testData.Response.Results
                }
            };
        }


        try
        {
            // Resolve cache service at runtime to avoid circular dependency
            var cacheService = _serviceProvider.GetKeyedService<IBaseCacheService<DuplicateNamesAndBirthdaysCachedResponse>>(
                "DuplicateNamesAndBirthdaysHostedService");

            if (cacheService == null)
            {
                _logger.LogWarning("Cache service not available, returning null");
                return null;
            }

            var allCachedData = await cacheService.GetAllAsync(cancellationToken);
            var cachedResponse = allCachedData.FirstOrDefault();

            if (cachedResponse == null)
            {
                return null;
            }

            // Filter out fake SSN pairs if this is a DuplicateNamesAndBirthdaysRequest with filtering requested
            var results = cachedResponse.Data.Results;
            if (request is DuplicateNamesAndBirthdaysRequest dRequest && !dRequest.IncludeFictionalSsnPairs)
            {
                results = results.Where(r => !r.IsFakeSsn).ToList();
                _logger.LogInformation("Filtered cached results to exclude {FilteredCount} records with fictional SSNs",
                    cachedResponse.Data.Results.Count() - results.Count());
            }

            // Apply pagination to the sorted data
            var skip = request.Skip ?? 0;
            var take = request.Take ?? byte.MaxValue; // Default to byte.MaxValue if not specified

            var paginatedResults = await results.AsQueryable().ToPaginationResultsAsync(request, cancellationToken: cancellationToken);

            return new DuplicateNamesAndBirthdaysCachedResponse
            {
                AsOfDate = cachedResponse.AsOfDate,
                Data = paginatedResults
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve cached duplicate names and birthdays data");
            return null;
        }
    }

    public async Task ForceRefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Resolve cache service at runtime to avoid circular dependency
            var cacheService = _serviceProvider.GetKeyedService<IBaseCacheService<DuplicateNamesAndBirthdaysCachedResponse>>(
                "DuplicateNamesAndBirthdaysHostedService");

            if (cacheService == null)
            {
                _logger.LogWarning("Cache service not available for force refresh");
                throw new InvalidOperationException("Cache service is not available");
            }

            _logger.LogInformation("Force refreshing duplicate names and birthdays cache");

            // Force a complete cache refresh by clearing and reloading all data
            await cacheService.ClearAsync(cancellationToken);
            await cacheService.InitializeCacheAsync(cancellationToken);

            _logger.LogInformation("Cache force refresh completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to force refresh duplicate names and birthdays cache");
            throw new InvalidOperationException("Cache force refresh failed for duplicate names and birthdays", ex);
        }
    }

    private static int GetDictValue(Dictionary<int, byte> dict, int key)
    {
        if (dict.ContainsKey(key))
        {
            dict[key]++;
            return dict[key];
        }

        dict[key] = 1;
        return 1;
    }
}

