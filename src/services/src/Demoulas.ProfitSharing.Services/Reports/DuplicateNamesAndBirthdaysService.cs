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
        DuplicateNamesAndBirthdaysRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DUPLICATE NAMES AND BIRTHDAYS"))
        {
            var dict = new Dictionary<long, byte>();
            var dupInfo = new HashSet<DemographicMatchDto>();
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                IQueryable<DemographicMatchDto> dupNameSlashDateOfBirth;
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                // Fallback for mocked (in-memory) db context which does not support raw SQL
                if (_host.IsTestEnvironment())
                {
                    _logger.LogDebug(1, "Inside the test environment");
                    dupNameSlashDateOfBirth = demographics
                        .TagWith("GetDuplicateDemographics-Test")
                        .Include(d => d.ContactInfo)
                        .Select(d => new DemographicMatchDto { DemographicId = d.Id, MatchedDemographicId = d.Id });
                }
                else
                {
                    _logger.LogDebug(2, "Outside the test environment");
                    string dupQuery =
                        @"WITH FILTERED_DEMOGRAPHIC AS (SELECT /*+ MATERIALIZE */ ID, FULL_NAME, DATE_OF_BIRTH, BADGE_NUMBER, SSN
                              FROM DEMOGRAPHIC)
SELECT /*+ USE_HASH(p1 p2) */ p1.ID as DemographicId, p2.ID as MatchedDemographicId
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id <> p2.Id /* Avoid self-joins and duplicate pairs */
                  AND SUBSTR(p1.FULL_NAME,1,1) = SUBSTR(p2.FULL_NAME,1,1) -- Eliminate by first letter before using more CPU intensive functions
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */
UNION ALL
SELECT /*+ USE_HASH(p1 p2) */ p2.ID as DemographicId, p1.ID as MatchedDemographicId
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id <> p2.Id /* Avoid self-joins and duplicate pairs */
                  AND SUBSTR(p1.FULL_NAME,1,1) = SUBSTR(p2.FULL_NAME,1,1) -- Eliminate by first letter before using more CPU intensive functions
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )                  
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */";

                    dupNameSlashDateOfBirth = ctx.Database.SqlQueryRaw<DemographicMatchDto>(dupQuery);
                    _logger.LogDebug(3, "Got value in dupNameSlashDateOfBirth");
                }

                dupInfo = await dupNameSlashDateOfBirth
                    .TagWith("GetDuplicateMatches-DuplicateNamesAndBirthdays")
                    .ToHashSetAsync(cancellationToken);

                var demographicIds = dupInfo.Select(x => x.DemographicId).ToHashSet();
                var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
                _logger.LogDebug(4, "Running the linq query to get from demographics");
                var query = from dem in demographics.TagWith("GetDuplicateDemographicsReport-MainQuery").Include(d => d.EmploymentStatus)
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
                            where demographicIds.Contains(dem.Id)
                            select new
                            {
                                dem.Id,
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
                _logger.LogDebug(5, "Calling ToPaginationResultsAsync");
                return await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            }, cancellationToken);

            // Load fake SSNs and SSNs with change history before projection (so we can mark before masking)
            // Treat changed SSNs the same as fake SSNs - they typically indicate a fake SSN was assigned upstream
            var ssnsToExclude = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // Get all fake SSNs
                var fakeSsns = await ctx.FakeSsns
                    .TagWith("GetFakeSsns-DuplicateNamesAndBirthdays")
                    .Select(f => f.Ssn)
                    .ToHashSetAsync(cancellationToken);

                // Get all SSNs that have change history (treat as fake/problematic)
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                var changedSsns = await demographics
                    .TagWith("GetChangedSsns-DuplicateNamesAndBirthdays")
                    .Where(d => d.DemographicSsnChangeHistories.Any())
                    .Select(d => d.Ssn)
                    .ToHashSetAsync(cancellationToken);

                fakeSsns.UnionWith(changedSsns);
                return fakeSsns;
            }, cancellationToken);

            // Project and mark IsFakeSsn BEFORE masking SSN
            var projectedResults = results.Results.Select(r => new DuplicateNamesAndBirthdaysResponse
            {
                DemographicId = r.Id,
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
                Count = GetDictValue(dict, r.Id),
                NetBalance = r.NetBalance ?? 0,
                HoursCurrentYear = r.HoursCurrentYear,
                IncomeCurrentYear = r.IncomeCurrentYear,
                EmploymentStatusName = r.EmploymentStatusName ?? "",
                IsExecutive = r.PayFrequencyId == PayFrequency.Constants.Monthly,
                IsFakeSsn = ssnsToExclude.Contains(r.Ssn) // Mark based on unmasked SSN (includes fake and changed SSNs)
            }).ToList();

            foreach (var r in projectedResults)
            {
                r.Count = dupInfo.Count(x => x.MatchedDemographicId == r.DemographicId);
            }
            _logger.LogDebug(6, "Calling GetYearStartAndEndAccountingDatesAsync function");
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
        DuplicateNamesAndBirthdaysRequest request, CancellationToken cancellationToken = default)
    {
        if (_host.IsTestEnvironment())
        {
            try
            {
                var testData = await GetDuplicateNamesAndBirthdaysAsync(request, cancellationToken);

                if (testData?.Response == null)
                {
                    _logger.LogWarning("Test data returned null response");
                    return null;
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing test data for duplicate names and birthdays - returning null");
                return null;
            }
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

            if (cachedResponse == null || cachedResponse.Data == null)
            {
                return null;
            }

            // Get results from cached response - ensure Results is not null
            var results = cachedResponse.Data.Results?.ToList() ?? new List<DuplicateNamesAndBirthdaysResponse>();

            return new DuplicateNamesAndBirthdaysCachedResponse
            {
                AsOfDate = cachedResponse.AsOfDate,
                Data = await results.ToPaginationResultsAsync(request, cancellationToken)
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

    private static int GetDictValue(Dictionary<long, byte> dict, long key)
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

