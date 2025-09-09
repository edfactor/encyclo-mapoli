using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class CleanupReportService : ICleanupReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly ILogger<CleanupReportService> _logger;
    private readonly TotalService _totalService;
    private readonly IHostEnvironment _host;
    private readonly IDemographicReaderService _demographicReaderService;

    private readonly byte[] _distributionProfitCodes =
    [
        ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
        ProfitCode.Constants.OutgoingDirectPayments.Id,
        ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
    ];

    private readonly byte[] _validProfitCodes =
    [
        ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
        ProfitCode.Constants.OutgoingForfeitures.Id,
        ProfitCode.Constants.OutgoingDirectPayments.Id,
        ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
    ];

    public CleanupReportService(IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory factory,
        ICalendarService calendarService,
        TotalService totalService,
        IHostEnvironment host,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _host = host;
        _demographicReaderService = demographicReaderService;
        _logger = factory.CreateLogger<CleanupReportService>();
    }

  
    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>>
        GetDemographicBadgesNotInPayProfitAsync(SortedPaginationRequestDto req,
            CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var data = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                var query = from dem in demographics
                        .Include(d => d.EmploymentStatus)
                    where !(from pp in ctx.PayProfits select pp.DemographicId).Contains(dem.Id)
                    select new 
                    {
                        dem.BadgeNumber,
                        dem.Ssn,
                        EmployeeName = dem.ContactInfo.FullName ?? "",
                        Status = dem.EmploymentStatusId,
                        StatusName = dem.EmploymentStatus!.Name,
                        Store = dem.StoreNumber,
                        IsExecutive = dem.PayFrequencyId == PayFrequency.Constants.Monthly,
                    };
                return await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });

            var results = new PaginatedResponseDto<DemographicBadgesNotInPayProfitResponse>
            {
                Total = data.Total,
                Results = data.Results.Select(x => new DemographicBadgesNotInPayProfitResponse
                {
                    BadgeNumber = x.BadgeNumber,
                    EmployeeName = x.EmployeeName,
                    Ssn = x.Ssn.MaskSsn(),
                    Status = x.Status,
                    StatusName = x.StatusName,
                    Store = x.Store,
                    IsExecutive = x.IsExecutive
                }).ToList()
            };  

            _logger.LogInformation("Returned {Results} records", results.Results.Count());
            return new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = ReferenceData.DsmMinValue,
                EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
                ReportName = "DEMOGRAPHICS BADGES NOT ON PAYPROFIT",
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingCommaAsync(
        SortedPaginationRequestDto req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                var query = from dem in demographics
#pragma warning disable CA1847
                    where dem.ContactInfo.FullName == null || !dem.ContactInfo.FullName.Contains(",")
#pragma warning restore CA1847
                    select new NamesMissingCommaResponse
                    {
                        BadgeNumber = dem.BadgeNumber,
                        Ssn = dem.Ssn.MaskSsn(),
                        EmployeeName = dem.ContactInfo.FullName ?? "",
                        IsExecutive = dem.PayFrequencyId == PayFrequency.Constants.Monthly,
                    };
                return await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<NamesMissingCommaResponse>
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = ReferenceData.DsmMinValue,
                EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
                ReportName = "MISSING COMMA IN PY_NAME", Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdaysAsync(
        ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DUPLICATE NAMES AND BIRTHDAYS"))
        {
            var dict = new Dictionary<int, byte>();
            var balanceByBadge = new Dictionary<int, decimal>();
            var dupInfo = new HashSet<DemographicMatchDto>();
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                IQueryable<DemographicMatchDto> dupNameSlashDateOfBirth;
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                // Fallback for mocked (in-memory) db context which does not support raw SQL
                if (_host.IsTestEnvironment())
                {
                    dupNameSlashDateOfBirth = demographics
                        .Include(d => d.ContactInfo)
                        .Select(d => new DemographicMatchDto { FullName = d.ContactInfo.FullName!, MatchedId = d.Id });
                }
                else
                {
                    string dupQuery =
                        @"WITH FILTERED_DEMOGRAPHIC AS (SELECT /*+ MATERIALIZE */ ID, FULL_NAME, DATE_OF_BIRTH, BADGE_NUMBER
                              FROM DEMOGRAPHIC
                              WHERE NOT EXISTS (SELECT /*+ INDEX(fs) */ 1
                                                FROM FAKE_SSNS fs
                                                WHERE fs.SSN = DEMOGRAPHIC.SSN))
SELECT /*+ USE_HASH(p1 p2) */ p1.FULL_NAME as FullName, p2.BADGE_NUMBER MatchedId
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id < p2.Id /* Avoid self-joins and duplicate pairs */
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )
UNION ALL
SELECT /*+ USE_HASH(p1 p2) */ p2.FULL_NAME as FullName, p1.BADGE_NUMBER MatchedId
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id < p2.Id /* Avoid self-joins and duplicate pairs */
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )";

                    dupNameSlashDateOfBirth = ctx.Database
                        .SqlQueryRaw<DemographicMatchDto>(dupQuery);
                }

                dupInfo = await dupNameSlashDateOfBirth
                    .Where(d => !string.IsNullOrEmpty(d.FullName))
                    .ToHashSetAsync(cancellationToken);

                var names = dupInfo.Select(x => x.FullName).ToHashSet();

                var query = from dem in demographics.Include(d => d.EmploymentStatus)
                        join ppLj in ctx.PayProfits on new { DemographicId = dem.Id, req.ProfitYear } equals new
                        {
                            ppLj.DemographicId,
                            ppLj.ProfitYear
                        } into tmpPayProfit
                        from pp in tmpPayProfit.DefaultIfEmpty()
                        join b in _totalService.GetTotalBalanceSet(ctx, req.ProfitYear) on dem.Ssn equals b.Ssn into tmpBalance
                        from bal in tmpBalance.DefaultIfEmpty()
                        join yos in _totalService.GetYearsOfService(ctx, req.ProfitYear) on dem.Ssn equals yos.Ssn into tmpYos
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

                var rslt = await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

                return rslt;
            });
            var projectedResults = results.Results.Select(r => new DuplicateNamesAndBirthdaysResponse
            {
                BadgeNumber = r.BadgeNumber,
                Ssn = r.Ssn.MaskSsn(),
                Name = r.Name,
                DateOfBirth = r.DateOfBirth,
                Address = new AddressResponseDto
                {
                    Street = r.Address,
                    City = r.City,
                    State = r.State,
                    PostalCode = r.PostalCode,
                    CountryIso = r.CountryIso
                },
                Years = r.Years,
                HireDate = r.HireDate,
                TerminationDate = r.TerminationDate,
                Status = r.EmploymentStatusId,
                StoreNumber = r.StoreNumber,
                Count = dict.ContainsKey(r.BadgeNumber)
                            ? ++dict[r.BadgeNumber]
                            : dict[r.BadgeNumber] = 1,
                NetBalance = r.NetBalance ?? 0,
                HoursCurrentYear = r.HoursCurrentYear,
                IncomeCurrentYear = r.IncomeCurrentYear,
                EmploymentStatusName = r.EmploymentStatusName ?? "",
                IsExecutive = r.PayFrequencyId == PayFrequency.Constants.Monthly
            }).ToList();

            foreach (var r in projectedResults)
            {
                r.Count = dupInfo.Count(x => x.MatchedId == r.BadgeNumber);
            }

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = ReferenceData.DsmMinValue,
                EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
                ReportName = "DUPLICATE NAMES AND BIRTHDAYS", 
                Response = new PaginatedResponseDto<DuplicateNamesAndBirthdaysResponse>() 
                { 
                    Total = results.Total, 
                    Results = projectedResults
                }
            };
        }
    }

    public async Task<DistributionsAndForfeitureTotalsResponse> GetDistributionsAndForfeitureAsync(
        DistributionsAndForfeituresRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DISTRIBUTIONS AND FORFEITURES"))
        {
            var results = _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                var nameAndDobQuery = demographics
                    .Include(d => d.PayProfits.Where(p => p.ProfitYear == req.ProfitYear))
                    .Select(x => new
                    {
                        x.Ssn,
                        x.ContactInfo.FullName,
                        x.DateOfBirth,
                        x.BadgeNumber,
                        x.PayFrequencyId,
                        PsnSuffix = (short)0,
                        EnrollmentId = x.PayProfits.FirstOrDefault() != null
                            ? x.PayProfits.FirstOrDefault()!.EnrollmentId
                            : Enrollment.Constants.Import_Status_Unknown,
                    }).Union(ctx.Beneficiaries.Include(b => b.Contact).Select(x => new
                    {
                        x.Contact!.Ssn,
                        x.Contact.ContactInfo.FullName,
                        x.Contact.DateOfBirth,
                        x.BadgeNumber,
                        PayFrequencyId = (byte)0,
                        x.PsnSuffix,
                        EnrollmentId = Enrollment.Constants.Import_Status_Unknown
                    }))
                    .GroupBy(x => x.Ssn)
                    .Select(x => new
                    {
                        Ssn = x.Key,
                        FullName = x.Max(m => m.FullName),
                        DateOfBirth = x.Max(m => m.DateOfBirth),
                        BadgeNumber = x.Max(m => m.BadgeNumber),
                        PsnSuffix = x.Max(m => m.PsnSuffix),
                        EnrolledId = x.Max(m => m.EnrollmentId),
                        PayFrequencyId = x.Max(m => m.PayFrequencyId),
                    });

                var transferAndQdroCommentTypes = new List<int>()
                {
                    CommentType.Constants.TransferIn.Id,
                    CommentType.Constants.TransferOut.Id,
                    CommentType.Constants.QdroIn.Id,
                    CommentType.Constants.QdroOut.Id
                };

                var startDate = (DateTimeOffset?)(!req.StartDate.HasValue ? null : req.StartDate.Value.ToDateTimeOffset());
                var endDate = (DateTimeOffset?)(!req.EndDate.HasValue ? null : req.EndDate.Value.ToDateTimeOffset());

                var query = from pd in ctx.ProfitDetails
                    join nameAndDob in nameAndDobQuery on pd.Ssn equals nameAndDob.Ssn
                    where pd.ProfitYear == req.ProfitYear &&
                          _validProfitCodes.Contains(pd.ProfitCodeId) &&
                          (pd.ProfitCodeId != ProfitCode.Constants.Outgoing100PercentVestedPayment.Id ||
                           (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id &&
                            (!pd.CommentTypeId.HasValue ||
                             !transferAndQdroCommentTypes.Contains(pd.CommentTypeId.Value)))) &&
                            (!req.StartDate.HasValue || pd.CreatedAtUtc >= startDate) &&
                            (!req.EndDate.HasValue || pd.CreatedAtUtc <= endDate) &&
                            !(pd.ProfitCodeId == 9 && pd.CommentTypeId.HasValue && transferAndQdroCommentTypes.Contains(pd.CommentTypeId.Value))

                    select new
                    {
                        nameAndDob.BadgeNumber,
                        nameAndDob.PsnSuffix,
                        pd.Ssn,
                        EmployeeName = nameAndDob.FullName,
                        DistributionAmount = _distributionProfitCodes.Contains(pd.ProfitCodeId) ? pd.Forfeiture : 0,
                        TaxCode = pd.TaxCodeId,
                        State = pd.CommentRelatedState,
                        StateTax = pd.StateTaxes,
                        FederalTax = pd.FederalTaxes,
                        ForfeitAmount = pd.ProfitCodeId == 2 ? pd.Forfeiture : 0,
                        pd.YearToDate,
                        pd.MonthToDate,
                        Date = pd.CreatedAtUtc,
                        nameAndDob.DateOfBirth,
                        nameAndDob.EnrolledId,
                        nameAndDob.PayFrequencyId,
                    };
                

                var totals = await query.GroupBy(_ => true)
                    .Select(g => new
                    {
                        DistributionTotal = g.Sum(x => x.DistributionAmount),
                        StateTaxTotal = g.Sum(x => x.StateTax),
                        FederalTaxTotal = g.Sum(x => x.FederalTax),
                        ForfeitureTotal = g.Sum(x => x.ForfeitAmount)
                    })
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken) ?? new
                {
                    DistributionTotal = 0m, StateTaxTotal = 0m, FederalTaxTotal = 0m, ForfeitureTotal = 0m
                };

                // Calculate state tax totals by state
                var stateTaxTotals = await query
                    .Where(s=> s.StateTax > 0)
                    .GroupBy(x => x.State)
                    .Select(g => new { State = g.Key, Total = g.Sum(x => x.StateTax) })
                    .ToDictionaryAsync(x => x.State ?? string.Empty, x => x.Total, cancellationToken);

                var calInfo =
                    await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
                var sortReq = req;
                if (sortReq.SortBy != null && sortReq.SortBy.Equals("Age", StringComparison.OrdinalIgnoreCase))
                {
                    sortReq = req with { SortBy = "DateOfBirth" };
                }
                var paginated = await query.ToPaginationResultsAsync(sortReq, cancellationToken);

                var apiResponse = paginated.Results.Select(pd => new DistributionsAndForfeitureResponse
                {
                    BadgeNumber = pd.BadgeNumber,
                    PsnSuffix = pd.PsnSuffix,
                    Ssn = pd.Ssn.MaskSsn(),
                    EmployeeName = pd.EmployeeName,
                    DistributionAmount = pd.DistributionAmount,
                    TaxCode = pd.TaxCode,
                    StateTax = pd.StateTax,
                    State = pd.State,
                    FederalTax = pd.FederalTax,
                    ForfeitAmount = pd.ForfeitAmount,
                    Date = pd.MonthToDate is > 0 and <= 12 ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1) : pd.Date.ToDateOnly(),
                    Age = (byte)(pd.MonthToDate is > 0 and < 13
                        ? pd.DateOfBirth.Age(
                            new DateOnly(pd.YearToDate, pd.MonthToDate, 1).ToDateTime(TimeOnly.MinValue))
                        : pd.DateOfBirth.Age(
                            calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local))),
                    EnrolledId = pd.EnrolledId,
                    IsExecutive = pd.PayFrequencyId == PayFrequency.Constants.Monthly
                });


                return new DistributionsAndForfeitureTotalsResponse()
                {
                    ReportName = "Distributions and Forfeitures",
                    ReportDate = DateTimeOffset.UtcNow,
                    StartDate = calInfo.FiscalBeginDate,
                    EndDate = calInfo.FiscalEndDate,
                    DistributionTotal = totals.DistributionTotal,
                    StateTaxTotal = totals.StateTaxTotal,
                    FederalTaxTotal = totals.FederalTaxTotal,
                    ForfeitureTotal = totals.ForfeitureTotal,
                    StateTaxTotals = stateTaxTotals,
                    Response = new PaginatedResponseDto<DistributionsAndForfeitureResponse>(req)
                    {
                        Results = apiResponse.ToList(), Total = paginated.Total
                    }
                };
            });
            
            return await results;
        }
    }
}
