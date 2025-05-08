using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class CleanupReportService : ICleanupReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ContributionService _contributionService;
    private readonly ICalendarService _calendarService;
    private readonly ILogger<CleanupReportService> _logger;
    private readonly TotalService _totalService;
    private readonly IHostEnvironment _host;

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
        ContributionService contributionService,
        ILoggerFactory factory,
        ICalendarService calendarService,
        TotalService totalService,
        IHostEnvironment host)
    {
        _dataContextFactory = dataContextFactory;
        _contributionService = contributionService;
        _calendarService = calendarService;
        _totalService = totalService;
        _host = host;
        _logger = factory.CreateLogger<CleanupReportService>();

    }

  
    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>>
        GetDemographicBadgesNotInPayProfitAsync(SortedPaginationRequestDto req,
            CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(ctx =>
            {
                var query = from dem in ctx.Demographics
                        .Include(d => d.EmploymentStatus)
                    where !(from pp in ctx.PayProfits select pp.DemographicId).Contains(dem.Id)
                    select new DemographicBadgesNotInPayProfitResponse
                    {
                        BadgeNumber = dem.BadgeNumber,
                        Ssn = dem.Ssn.MaskSsn(),
                        EmployeeName = dem.ContactInfo.FullName ?? "",
                        Status = dem.EmploymentStatusId,
                        StatusName = dem.EmploymentStatus!.Name,
                        Store = dem.StoreNumber,
                    };
                return query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
            {
                ReportDate = DateTimeOffset.Now,
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
                var query = from dem in ctx.Demographics
#pragma warning disable CA1847
                    where dem.ContactInfo.FullName == null || !dem.ContactInfo.FullName.Contains(",")
#pragma warning restore CA1847
                    select new NamesMissingCommaResponse
                    {
                        BadgeNumber = dem.BadgeNumber,
                        Ssn = dem.Ssn.MaskSsn(),
                        EmployeeName = dem.ContactInfo.FullName ?? "",
                    };
                return await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<NamesMissingCommaResponse>
            {
                ReportDate = DateTimeOffset.Now, ReportName = "MISSING COMMA IN PY_NAME", Response = results
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
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                IQueryable<DemographicMatchDto> dupNameSlashDateOfBirth;

                // Fallback for mocked (in-memory) db context which does not support raw SQL
                if (_host.IsTestEnvironment())
                {
                    dupNameSlashDateOfBirth = ctx.Demographics
                        .Include(d => d.ContactInfo)
                        .Select(d => new DemographicMatchDto { FullName = d.ContactInfo.FullName! });
                }
                else
                {
                    string dupQuery =
                        @"WITH FILTERED_DEMOGRAPHIC AS (SELECT /*+ MATERIALIZE */ ID, FULL_NAME, DATE_OF_BIRTH
                              FROM DEMOGRAPHIC
                              WHERE NOT EXISTS (SELECT /*+ INDEX(fs) */ 1
                                                FROM FAKE_SSNS fs
                                                WHERE fs.SSN = DEMOGRAPHIC.SSN))
SELECT /*+ USE_HASH(p1 p2) */ p1.FULL_NAME as FullName
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id < p2.Id /* Avoid self-joins and duplicate pairs */
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )
UNION ALL
SELECT /*+ USE_HASH(p1 p2) */ p2.FULL_NAME as FullName
FROM FILTERED_DEMOGRAPHIC p1
         JOIN FILTERED_DEMOGRAPHIC p2
              ON p1.Id < p2.Id /* Avoid self-joins and duplicate pairs */
                  AND UTL_MATCH.EDIT_DISTANCE(p1.FULL_NAME, p2.FULL_NAME) < 3 /* Name similarity threshold */
                  AND SOUNDEX(p1.FULL_NAME) = SOUNDEX(p2.FULL_NAME) /* Phonetic similarity */
                  AND (ABS(TRUNC(p1.DATE_OF_BIRTH) - TRUNC(p2.DATE_OF_BIRTH)) <= 3 /* Allowable 3-day difference */ )";

                    dupNameSlashDateOfBirth = ctx.Database
                        .SqlQueryRaw<DemographicMatchDto>(dupQuery);
                }

                var names = await dupNameSlashDateOfBirth
                    .Where(d => !string.IsNullOrEmpty(d.FullName))
                    .Select(d => d.FullName)
                    .ToHashSetAsync(cancellationToken);

                var query = from dem in ctx.Demographics.Include(d => d.EmploymentStatus)
                    join ppLj in ctx.PayProfits on new { DemographicId = dem.Id, req.ProfitYear } equals new
                    {
                        ppLj.DemographicId, ppLj.ProfitYear
                    } into tmpPayProfit
                    from pp in tmpPayProfit.DefaultIfEmpty()
                    join pdLj in ctx.ProfitDetails on new { dem.Ssn, req.ProfitYear } equals new
                    {
                        pdLj.Ssn, pdLj.ProfitYear
                    } into tmpProfitDetails
                    from pd in tmpProfitDetails.DefaultIfEmpty()
                    where dem.ContactInfo.FullName != null && names.Contains(dem!.ContactInfo!.FullName!)
                    group new { dem, pp, pd } by new
                    {
                        dem.BadgeNumber,
                        SSN = dem.Ssn,
                        dem.ContactInfo.FullName,
                        dem.DateOfBirth,
                        dem.Address.Street,
                        dem.Address.City,
                        dem.Address.State,
                        dem.Address.PostalCode,
                        CountryISO = dem.Address.CountryIso,
                        dem.HireDate,
                        dem.TerminationDate,
                        dem.EmploymentStatusId,
                        dem.EmploymentStatus!.Name,
                        dem.StoreNumber,
                        PdSsn = pd != null ? pd.Ssn : 0,
                        CurrentHoursYear = pp != null ? pp.CurrentHoursYear : 0,
                        CurrentIncomeYear = pp != null ? pp.CurrentIncomeYear : 0
                    }
                    into g
                    orderby g.Key.FullName, g.Key.DateOfBirth, g.Key.SSN, g.Key.BadgeNumber
                    select new DuplicateNamesAndBirthdaysResponse
                    {
                        BadgeNumber = g.Key.BadgeNumber,
                        Ssn = g.Key.SSN.MaskSsn(),
                        Name = g.Key.FullName,
                        DateOfBirth = g.Key.DateOfBirth,
                        Address = new AddressResponseDto
                        {
                            City = g.Key.City,
                            State = g.Key.State,
                            Street = g.Key.Street,
                            CountryIso = g.Key.CountryISO,
                            PostalCode = g.Key.PostalCode,
                        },
                        HireDate = g.Key.HireDate,
                        TerminationDate = g.Key.TerminationDate,
                        Status = g.Key.EmploymentStatusId,
                        EmploymentStatusName = g.Key.Name,
                        StoreNumber = g.Key.StoreNumber,
                        Count = g.Count(),
                        HoursCurrentYear = g.Key.CurrentHoursYear,
                        IncomeCurrentYear = g.Key.CurrentIncomeYear
                    };

                var rslt = await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

                dict = await (
                    from yis in _totalService.GetYearsOfService(ctx, req.ProfitYear)
                    join d in ctx.Demographics on yis.Ssn equals d.Ssn
                    select new { d.BadgeNumber, Years = yis.Years ?? 0 }
                ).ToDictionaryAsync(x => x.BadgeNumber, x => x.Years, cancellationToken: cancellationToken);

                return rslt;
            });

            ISet<int> badgeNumbers = results.Results.Select(r => r.BadgeNumber).ToHashSet();
            var balanceDict = await _contributionService.GetNetBalance(req.ProfitYear, badgeNumbers, cancellationToken);

            foreach (DuplicateNamesAndBirthdaysResponse dup in results.Results)
            {
                _ = dict.TryGetValue(dup.BadgeNumber, out byte years);
                dup.Years = years;

                balanceDict.TryGetValue(dup.BadgeNumber, out var balance);
                dup.NetBalance = balance?.TotalEarnings ?? 0;
            }

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.Now, ReportName = "DUPLICATE NAMES AND BIRTHDAYS", Response = results
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
                var nameAndDobQuery = ctx.Demographics
                    .Include(d => d.PayProfits.Where(p => p.ProfitYear == req.ProfitYear))
                    .Select(x => new
                    {
                        x.Ssn,
                        x.ContactInfo.FullName,
                        x.DateOfBirth,
                        x.BadgeNumber,
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
                        EnrolledId = x.Max(m => m.EnrollmentId)
                    });

                var transferAndQdroCommentTypes = new List<int>()
                {
                    CommentType.Constants.TransferIn.Id,
                    CommentType.Constants.TransferOut.Id,
                    CommentType.Constants.QdroIn.Id,
                    CommentType.Constants.QdroOut.Id
                };

                var query = from pd in ctx.ProfitDetails
                    join nameAndDob in nameAndDobQuery on pd.Ssn equals nameAndDob.Ssn
                    where pd.ProfitYear == req.ProfitYear &&
                          _validProfitCodes.Contains(pd.ProfitCodeId) &&
                          (pd.ProfitCodeId != ProfitCode.Constants.Outgoing100PercentVestedPayment.Id ||
                           (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id &&
                            (!pd.CommentTypeId.HasValue ||
                             !transferAndQdroCommentTypes.Contains(pd.CommentTypeId.Value)))) &&
                          (req.StartMonth == 0 || pd.MonthToDate >= req.StartMonth) &&
                          (req.EndMonth == 0 || pd.MonthToDate <= req.EndMonth)
                    select new
                    {
                        BadgeNumber = nameAndDob.BadgeNumber,
                        PsnSuffix = nameAndDob.PsnSuffix,
                        Ssn = pd.Ssn.MaskSsn(),
                        EmployeeName = nameAndDob.FullName,
                        DistributionAmount = _distributionProfitCodes.Contains(pd.ProfitCodeId) ? pd.Forfeiture : 0,
                        TaxCode = pd.TaxCodeId,
                        StateTax = pd.StateTaxes,
                        FederalTax = pd.FederalTaxes,
                        ForfeitAmount = pd.ProfitCodeId == 2 ? pd.Forfeiture : 0,
                        pd.YearToDate,
                        pd.MonthToDate,
                        Date = pd.TransactionDate,
                        nameAndDob.DateOfBirth,
                        EnrolledId = nameAndDob.EnrolledId,
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

                var paginated = await query.ToPaginationResultsAsync(req, cancellationToken);

                var calInfo =
                    await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
                var apiResponse = paginated.Results.Select(pd => new DistributionsAndForfeitureResponse
                {
                    BadgeNumber = pd.BadgeNumber,
                    PsnSuffix = pd.PsnSuffix,
                    Ssn = pd.Ssn,
                    EmployeeName = pd.EmployeeName,
                    DistributionAmount = pd.DistributionAmount,
                    TaxCode = pd.TaxCode,
                    StateTax = pd.StateTax,
                    FederalTax = pd.FederalTax,
                    ForfeitAmount = pd.ForfeitAmount,
                    Date = pd.MonthToDate is > 0 and <= 12 ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1) : pd.Date.ToDateOnly(),
                    Age = (byte)(pd.MonthToDate is > 0 and < 13
                        ? pd.DateOfBirth.Age(
                            new DateOnly(pd.YearToDate, pd.MonthToDate, 1).ToDateTime(TimeOnly.MinValue))
                        : pd.DateOfBirth.Age(
                            calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local))),
                    EnrolledId = pd.EnrolledId
                });


                return new DistributionsAndForfeitureTotalsResponse()
                {
                    ReportName = "Distributions and Forfeitures",
                    ReportDate = DateTimeOffset.Now,
                    DistributionTotal = totals.DistributionTotal,
                    StateTaxTotal = totals.StateTaxTotal,
                    FederalTaxTotal = totals.FederalTaxTotal,
                    ForfeitureTotal = totals.ForfeitureTotal,
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
