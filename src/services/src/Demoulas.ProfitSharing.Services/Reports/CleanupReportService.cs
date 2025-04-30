using System.Globalization;
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
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.ProfitSharing.Services.ItOperations;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsnAsync(SortedPaginationRequestDto req,
        CancellationToken ct)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            int cutoffYear = DateTime.UtcNow.Year - 5;

            var dupSsns = await ctx.Demographics
                .GroupBy(x => x.Ssn)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSetAsync(ct);

            var rslts = await ctx.Demographics
                .Include(x => x.EmploymentStatus)
                .Where(dem => dupSsns.Contains(dem.Ssn))
                .OrderBy(d => d.Ssn)
                .Select(dem => new PayrollDuplicateSsnResponseDto
                {
                    BadgeNumber = dem.BadgeNumber,
                    Ssn = dem.Ssn.MaskSsn(),
                    Name = dem.ContactInfo.FullName,
                    Address = new AddressResponseDto
                    {
                        Street = dem.Address.Street,
                        City = dem.Address.City,
                        State = dem.Address.State,
                        PostalCode = dem.Address.PostalCode,
                        CountryIso = Country.Constants.Us
                    },
                    HireDate = dem.HireDate,
                    TerminationDate = dem.TerminationDate,
                    RehireDate = dem.ReHireDate,
                    Status = dem.EmploymentStatusId,
                    EmploymentStatusName = dem.EmploymentStatus!.Name,
                    StoreNumber = dem.StoreNumber,
                    ProfitSharingRecords = dem.PayProfits.Count(pp => pp.ProfitYear >= cutoffYear),
                    PayProfits = dem.PayProfits
                        .Where(pp => pp.ProfitYear >= cutoffYear)
                        .OrderByDescending(pp => pp.ProfitYear)
                        .Select(pp => new PayProfitResponseDto
                        {
                            DemographicId = pp.DemographicId,
                            ProfitYear = pp.ProfitYear,
                            CurrentHoursYear = pp.CurrentHoursYear,
                            CurrentIncomeYear = pp.CurrentIncomeYear,
                            WeeksWorkedYear = pp.WeeksWorkedYear,
                            LastUpdate = pp.LastUpdate,
                            PointsEarned = pp.PointsEarned
                        }).ToList()
                })
                .ToPaginationResultsAsync(req, ct);

            return new ReportResponseBase<PayrollDuplicateSsnResponseDto>
            {
                ReportName = "Duplicate SSNs on Demographics", Response = rslts
            };
        });
    }


    public async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>>
        GetNegativeETVAForSSNsOnPayProfitResponseAsync(ProfitYearRequest req,
            CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request NEGATIVE ETVA FOR SSNs ON PAYPROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                var ssnUnion = c.Demographics.Select(d => d.Ssn)
                    .Union(c.Beneficiaries
                        .Include(b => b.Contact)
                        .Select(b => b.Contact!.Ssn));

                return c.PayProfits
                    .Include(p => p.Demographic)
                    .Where(p => p.ProfitYear == req.ProfitYear
                                && ssnUnion.Contains(p.Demographic!.Ssn)
                                && p.Etva < 0)
                    .Select(p => new NegativeEtvaForSsNsOnPayProfitResponse
                    {
                        BadgeNumber = p.Demographic!.BadgeNumber,
                        Ssn = p.Demographic.Ssn.MaskSsn(),
                        EtvaValue = p.Etva
                    })
                    .OrderBy(p => p.BadgeNumber)
                    .ToPaginationResultsAsync(req, cancellationToken);
            });

            _logger.LogWarning("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
            {
                ReportName = "NEGATIVE ETVA FOR SSNs ON PAYPROFIT",
                ReportDate = DateTimeOffset.Now,
                Response = results
            };
        }
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
                        Date = pd.CreatedUtc,
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

    public async Task<YearEndProfitSharingReportResponse> GetYearEndProfitSharingReportAsync(
        YearEndProfitSharingReportRequest req, CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var birthDate21 = calInfo.FiscalEndDate.AddYears(-21);

        var response = new YearEndProfitSharingReportResponse
        {
            ReportDate = DateTimeOffset.Now,
            ReportName = $"PROFIT SHARE YEAR END REPORT FOR {req.ProfitYear}",
            Response = new PaginatedResponseDto<YearEndProfitSharingReportDetail>()
        };

        var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var qry = from pp in ctx.PayProfits.Include(p => p.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                join et in ctx.EmploymentTypes on pp.Demographic!.EmploymentTypeId equals et.Id
                join yipTbl in _totalService.GetYearsOfService(ctx, req.ProfitYear) on pp.Demographic!.Ssn equals
                    yipTbl.Ssn into yipTmp
                from yip in yipTmp.DefaultIfEmpty()
                select new
                {
                    pp.Demographic!.BadgeNumber,
                    pp.CurrentHoursYear,
                    pp.HoursExecutive,
                    pp.Demographic!.DateOfBirth,
                    pp.Demographic!.EmploymentStatusId,
                    pp.Demographic!.TerminationDate,
                    pp.Demographic!.Ssn,
                    pp.Demographic!.ContactInfo.LastName,
                    pp.Demographic!.ContactInfo.FirstName,
                    pp.Demographic!.StoreNumber,
                    EmploymentTypeId =
                        pp.Demographic!.EmploymentTypeId
                            .ToString(), //There seems to be some sort of issue in the oracle ef provider that struggles with the char type.  It maps this expression
                    //.CAST((BITXOR("s"."EMPLOYMENT_TYPE_ID", N'')) AS NUMBER(1)) )
                    //Converting to .fix this issue.
                    EmploymentTypeName = et.Name,
                    pp.CurrentIncomeYear,
                    pp.IncomeExecutive,
                    pp.PointsEarned,
                    yip.Years
                };
            if (req.IsYearEnd)
            {
                qry = from pp in ctx.PayProfits.Include(p => p.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                    join d in FrozenService.GetDemographicSnapshot(ctx, req.ProfitYear) on pp.DemographicId equals d.Id
                    join et in ctx.EmploymentTypes on pp.Demographic!.EmploymentTypeId equals et.Id
                    join yipTbl in _totalService.GetYearsOfService(ctx, req.ProfitYear) on pp.Demographic!.Ssn equals
                        yipTbl.Ssn into yipTmp
                    from yip in yipTmp.DefaultIfEmpty()
                    select new
                    {
                        pp.Demographic!.BadgeNumber,
                        pp.CurrentHoursYear,
                        pp.HoursExecutive,
                        pp.Demographic!.DateOfBirth,
                        pp.Demographic!.EmploymentStatusId,
                        pp.Demographic!.TerminationDate,
                        pp.Demographic!.Ssn,
                        pp.Demographic!.ContactInfo.LastName,
                        pp.Demographic!.ContactInfo.FirstName,
                        pp.Demographic!.StoreNumber,
                        EmploymentTypeId = pp.Demographic!.EmploymentTypeId.ToString(),
                        EmploymentTypeName = et.Name,
                        pp.CurrentIncomeYear,
                        pp.IncomeExecutive,
                        pp.PointsEarned,
                        yip.Years
                    };


            }

            if (req.IncludeBeneficiaries)
            {
                qry = from b in ctx.Beneficiaries
                        .Include(b => b.Contact)
                        .ThenInclude(c => c!.ContactInfo)
                    where (!ctx.Demographics.Any(x =>
                        x.Ssn == b.Contact!.Ssn)) //Filter out employees who are beneficiaries
                    select new
                    {
                        BadgeNumber = 0,
                        CurrentHoursYear = 0m,
                        HoursExecutive = 0m,
                        b.Contact!.DateOfBirth,
                        EmploymentStatusId = ' ',
                        TerminationDate = (DateOnly?)null,
                        b.Contact!.Ssn,
                        b.Contact!.ContactInfo.LastName,
                        b.Contact!.ContactInfo.FirstName,
                        StoreNumber = (short)0,
                        EmploymentTypeId = " ",
                        EmploymentTypeName = "",
                        CurrentIncomeYear = 0m,
                        IncomeExecutive = 0m,
                        PointsEarned = (decimal?)null,
                        Years = (byte?)0
                    };
            }

            if (req.MinimumHoursInclusive.HasValue)
            {
                qry = qry.Where(p => (p.CurrentHoursYear + p.HoursExecutive) >= req.MinimumHoursInclusive.Value);
            }

            if (req.MaximumHoursInclusive.HasValue)
            {
                qry = qry.Where(p => (p.CurrentHoursYear + p.HoursExecutive) <= req.MaximumHoursInclusive.Value);
            }

            if (req.MinimumAgeInclusive.HasValue)
            {
                var minBirthDate = calInfo.FiscalEndDate.AddYears(req.MinimumAgeInclusive.Value * -1);
                qry = qry.Where(p => p.DateOfBirth <= minBirthDate);
            }

            if (req.MaximumAgeInclusive.HasValue)
            {
                var maxBirthDate = calInfo.FiscalEndDate.AddYears((req.MaximumAgeInclusive.Value + 1) * -1).AddDays(1);
                qry = qry.Where(p => p.DateOfBirth >= maxBirthDate);
            }

            if (!req.IncludeBeneficiaries && (!req.IncludeActiveEmployees || !req.IncludeEmployeesTerminatedThisYear ||
                                              !req.IncludeInactiveEmployees))
            {
                var validStatus = new List<char>();
                if (req.IncludeActiveEmployees)
                {
                    validStatus.Add(EmploymentStatus.Constants.Active);
                }

                if (req.IncludeInactiveEmployees)
                {
                    validStatus.Add(EmploymentStatus.Constants.Inactive);
                }

                if (req.IncludeEmployeesTerminatedThisYear || req.IncludeTerminatedEmployees)
                {
                    validStatus.Add(EmploymentStatus.Constants.Terminated);
                }

                if (req is { IncludeActiveEmployees: true, IncludeEmployeesTerminatedThisYear: false })
                {
                    qry = qry.Where(p =>
                        validStatus.Contains(p.EmploymentStatusId) || p.TerminationDate > calInfo.FiscalEndDate);
                }
                else if (req.IncludeEmployeesTerminatedThisYear && req is
                             { IncludeActiveEmployees: false, IncludeInactiveEmployees: false })
                {
                    qry = qry.Where(p =>
                        validStatus.Contains(p.EmploymentStatusId) && p.TerminationDate <= calInfo.FiscalEndDate &&
                        p.TerminationDate >= calInfo.FiscalBeginDate);
                }
                else if (req.IncludeTerminatedEmployees && req is
                             { IncludeInactiveEmployees: false, IncludeActiveEmployees: false })
                {
                    qry = qry.Where(p =>
                        validStatus.Contains(p.EmploymentStatusId) && p.TerminationDate <= calInfo.FiscalEndDate);
                }
                else
                {
                    qry = qry.Where(p => validStatus.Contains(p.EmploymentStatusId));
                }

            }

            var joinedQry = from pp in qry
                            join totTbl in _totalService.GetTotalBalanceSet(ctx, req.ProfitYear) on pp.Ssn equals totTbl.Ssn into totTmp
                            from tot in totTmp.DefaultIfEmpty()
                            select new { pp, total = tot != null ? tot.Total : 0m };

            if (req is
                {
                    IncludeEmployeesWithNoPriorProfitSharingAmounts: false,
                    IncludeEmployeesWithPriorProfitSharingAmounts: true
                })
            {
                joinedQry = joinedQry.Where(jq => jq.total > 0);
            }

            if (req is
                {
                    IncludeEmployeesWithNoPriorProfitSharingAmounts: true,
                    IncludeEmployeesWithPriorProfitSharingAmounts: false
                })
            {
                joinedQry = joinedQry.Where(jq => jq.total == 0);
            }

            var firstContributionSubquery = from pd in ctx.ProfitDetails
                where pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                group pd by pd.Ssn
                into pdGrp
                select new { Ssn = pdGrp.Key, FirstContributionYear = (short?)pdGrp.Min(x => x.ProfitYear) };

            var qryWithContributionYear = from j in joinedQry
                                          join fcTbl in firstContributionSubquery on j.pp.Ssn equals fcTbl.Ssn into fcTmp
                                          from fc in fcTmp.DefaultIfEmpty()
                                          select new
                                          {
                                              j.pp,
                                              j.total,
                                              fc.FirstContributionYear
                                          };


            if (req.IncludeTotals)
            {
                var totalsQry = from q in qryWithContributionYear
                    group q by true
                    into qGrp
                    select new
                    {
                        WagesTotal = qGrp.Sum(x => x.pp.IncomeExecutive + x.pp.CurrentIncomeYear),
                        HoursTotal = qGrp.Sum(x => x.pp.HoursExecutive + x.pp.CurrentHoursYear),
                        PointsTotal = qGrp.Sum(x => x.pp.PointsEarned),
                        TerminatedWagesTotal =
                            qGrp.Where(y =>
                                    y.pp.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                                    y.pp.TerminationDate < calInfo.FiscalEndDate)
                                .Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                        TerminatedHoursTotal =
                            qGrp.Where(y =>
                                    y.pp.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                                    y.pp.TerminationDate < calInfo.FiscalEndDate)
                                .Sum(y => y.pp.HoursExecutive + y.pp.CurrentHoursYear),
                        NumberOfEmployees = qGrp.Count(),
                        NumberOfNewEmployees =
                            qGrp.Count(y =>
                                y.FirstContributionYear == null && (y.pp.HoursExecutive + y.pp.CurrentHoursYear) >
                                ReferenceData.MinimumHoursForContribution()),
                        NumberOfEmployeesUnder21 = qGrp.Count(y => y.pp.DateOfBirth > birthDate21),
                    };

                var totals = await totalsQry.FirstOrDefaultAsync(cancellationToken);
                if (totals != null)
                {
                    response.WagesTotal = totals.WagesTotal;
                    response.HoursTotal = totals.HoursTotal;
                    response.PointsTotal = totals.PointsTotal ?? 0m;
                    response.TerminatedWagesTotal = totals.TerminatedWagesTotal;
                    response.TerminatedHoursTotal = totals.TerminatedHoursTotal;
                    response.NumberOfEmployees = totals.NumberOfEmployees;
                    response.NumberOfNewEmployees = totals.NumberOfNewEmployees;
                    response.NumberOfEmployeesInPlan = totals.NumberOfEmployees - totals.NumberOfEmployeesUnder21 -
                                                       totals.NumberOfNewEmployees;
                    response.NumberOfEmployeesUnder21 = totals.NumberOfEmployeesUnder21;

                }

            }

            if (req.IncludeDetails)
            {

                return await qryWithContributionYear
                          .OrderBy(p => p.pp.LastName)
                          .ThenBy(p => p.pp.FirstName)
                          .Select(x => new YearEndProfitSharingReportDetail()
                          {
                              BadgeNumber = x.pp.BadgeNumber,
                              EmployeeName = $"{x.pp.LastName}, {x.pp.FirstName}",
                              FirstName = x.pp.FirstName,
                              LastName = x.pp.LastName,
                              StoreNumber = x.pp.StoreNumber,
                              EmployeeTypeCode = x.pp.EmploymentTypeId[0],
                              EmployeeTypeName = x.pp.EmploymentTypeName,
                              DateOfBirth = x.pp.DateOfBirth,
                              Age = 0, //Filled out below after materialization
                              Ssn = x.pp.Ssn.MaskSsn(),
                              Wages = x.pp.CurrentIncomeYear + x.pp.IncomeExecutive,
                              Hours = x.pp.CurrentHoursYear + x.pp.HoursExecutive,
                              Points = Convert.ToInt16(x.pp.PointsEarned),
                              IsNew = (x.FirstContributionYear == null && x.pp.HoursExecutive + x.pp.CurrentHoursYear > ReferenceData.MinimumHoursForContribution()),
                              IsUnder21 = false, //Filled out below after materialization
                              EmployeeStatus = x.pp.EmploymentStatusId,
                              Balance = x.total ?? 0,
                              YearsInPlan = x.pp.Years ?? 0
                          })
                          .ToPaginationResultsAsync(req, cancellationToken);
            }

            return new PaginatedResponseDto<YearEndProfitSharingReportDetail>();
        });

        foreach (var item in rslt.Results)
        {
            item.Age = (byte)item.DateOfBirth.Age(
                calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local));
            if (item.Age < 21)
            {
                item.IsUnder21 = true;
                item.Points = 0;
            }
        }

        response.Response = rslt;

        return response;
    }

    public async Task<YearEndProfitSharingReportSummaryResponse> GetYearEndProfitSharingSummaryReportAsync(
        FrozenProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
            var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
            var nonTerminatedStatuses =
                new List<char>() { EmploymentStatus.Constants.Active, EmploymentStatus.Constants.Inactive };
            var response = new YearEndProfitSharingReportSummaryResponse()
            {
                LineItems = new List<YearEndProfitSharingReportSummaryLineItem>()
            };

            // AGE 18-20 WITH >= 1000 PS HOURS
            var qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday18 && x.Demographic!.DateOfBirth > birthday21)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            var lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "1",
                LineItemTitle = "AGE 18-20 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            // >= AGE 21 WITH >= 1000 PS HOURS
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday21)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "2",
                LineItemTitle = ">= AGE 21 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            // <  AGE 18
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => x.Demographic!.DateOfBirth > birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "3",
                LineItemTitle = "<  AGE 18",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //>= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth < birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total > 0);
            
            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
                {
                    Subgroup = "Active and Inactive",
                    LineItemPrefix = "4",
                    LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                    NumberOfMembers = x.Count(),
                    TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                    TotalBalance = x.Sum(y => y.tot.Total ?? 0)
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //>= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth < birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total == 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
                {
                    Subgroup = "Active and Inactive",
                    LineItemPrefix = "5",
                    LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                    NumberOfMembers = x.Count(),
                    TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                    TotalBalance = x.Sum(y => y.tot.Total ?? 0)
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated >= AGE 18 WITH >= 1000 PS HOURS 
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "6",
                LineItemTitle = ">= AGE 18 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) < 1000 && x.Demographic!.DateOfBirth <= birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total == 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "7",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) < 1000 && x.Demographic!.DateOfBirth <= birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total != 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "8",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated <  AGE 18           NO WAGES :   0
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentIncomeYear + x.IncomeExecutive) == 0 && x.Demographic!.DateOfBirth > birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "X",
                LineItemTitle = "<  AGE 18           NO WAGES :   0",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            var beneQry = ctx.BeneficiaryContacts.Where(bc=>!ctx.Demographics.Any(x=>x.Ssn == bc.Ssn))
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await beneQry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "N",
                LineItemTitle = "NON-EMPLOYEE BENEFICIARIES",
                NumberOfMembers = x.Count(),
                TotalWages = 0,
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            return response;
        });
    }
}
