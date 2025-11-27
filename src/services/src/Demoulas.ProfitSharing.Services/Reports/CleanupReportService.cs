using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
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
        _demographicReaderService = demographicReaderService;
        _logger = factory.CreateLogger<CleanupReportService>();
    }


    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>>
        GetDemographicBadgesNotInPayProfitAsync(ProfitYearRequest req,
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
            }, cancellationToken);

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
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
            return new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = calInfo.FiscalBeginDate,
                EndDate = calInfo.FiscalEndDate,
                ReportName = "DEMOGRAPHICS BADGES NOT ON PAYPROFIT",
                Response = results
            };
        }
    }

    public async Task<Result<DistributionsAndForfeitureTotalsResponse>> GetDistributionsAndForfeitureAsync(
        DistributionsAndForfeituresRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DISTRIBUTIONS AND FORFEITURES"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext<Result<DistributionsAndForfeitureTotalsResponse>>(async ctx =>
            {
                if (!await ctx.PayProfits.AnyAsync(cancellationToken))
                {
                    return Result<DistributionsAndForfeitureTotalsResponse>.Failure(Error.NoPayProfitsDataAvailable);
                }

                // Always use live/now data
                // This assumes all the payprofits for lastest year are available 
                var latestYear = await ctx.PayProfits.MaxAsync(p => p.ProfitYear, cancellationToken);
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                var nameAndDobQuery = demographics
                    .Include(d => d.PayProfits.Where(p => p.ProfitYear == latestYear))
                    .Select(x => new
                    {
                        x.Ssn,
                        x.ContactInfo.FullName,
                        x.DateOfBirth,
                        x.BadgeNumber,
                        x.PayFrequencyId,
                        PsnSuffix = (short)0,
                        EnrollmentId = x.PayProfits
                            .Where(p => p.ProfitYear == latestYear)
                            .Select(p => p.EnrollmentId)
                            .FirstOrDefault()
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

                var startDate = req.StartDate ?? ReferenceData.DsmMinValue;
                // force to start of month, so returned reference range is correct - the day of the month is ignored.
                startDate = new DateOnly(startDate.Year, startDate.Month, 1);

                var endDate = req.EndDate ?? DateTime.Now.ToDateOnly();
                // force to end of month, so returned reference range is correct - day of the month is ignored.
                endDate = new DateOnly(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));

                var query = from pd in ctx.ProfitDetails
                            join nameAndDob in nameAndDobQuery on pd.Ssn equals nameAndDob.Ssn
                            where _validProfitCodes.Contains(pd.ProfitCodeId) &&
                                  (pd.ProfitCodeId != ProfitCode.Constants.Outgoing100PercentVestedPayment.Id ||
                                   (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id &&
                                    (!pd.CommentTypeId.HasValue ||
                                     !transferAndQdroCommentTypes.Contains(pd.CommentTypeId.Value)))) &&
                                      // PROFIT_DETAIL.profitYear <--- is the year selector 
                                      // PROFIT_DETAIL.MonthToDate <--- is the month selector  See QPAY129.pco
                                      (pd.ProfitYear > startDate.Year || (pd.ProfitYear == startDate.Year && pd.MonthToDate >= startDate.Month)) &&
                                      (pd.ProfitYear < endDate.Year || (pd.ProfitYear == endDate.Year && pd.MonthToDate <= endDate.Month)) &&
                                      !(pd.ProfitCodeId == /*9*/ ProfitCode.Constants.Outgoing100PercentVestedPayment && pd.CommentTypeId.HasValue && transferAndQdroCommentTypes.Contains(pd.CommentTypeId.Value)) &&
                                      // State filter - apply if specified (supports multiple states)
                                      (req.States == null || req.States.Length == 0 || req.States.Contains(pd.CommentRelatedState)) &&
                                      // Tax code filter - apply if specified (supports multiple tax codes)
                                      (req.TaxCodes == null || req.TaxCodes.Length == 0 || (pd.TaxCodeId.HasValue && req.TaxCodes.Contains(pd.TaxCodeId.Value)))

                            select new
                            {
                                BadgePsn = (long)(nameAndDob.PsnSuffix > 0 ? (nameAndDob.BadgeNumber * 10_000 + nameAndDob.PsnSuffix) : nameAndDob.BadgeNumber),
                                pd.Ssn,
                                EmployeeName = nameAndDob.FullName,
                                DistributionAmount = _distributionProfitCodes.Contains(pd.ProfitCodeId) ? pd.Forfeiture : 0m,
                                TaxCode = pd.TaxCodeId,
                                State = pd.CommentRelatedState,
                                StateTax = pd.StateTaxes,
                                FederalTax = pd.FederalTaxes,
                                ForfeitAmount = pd.ProfitCodeId == /*2*/ ProfitCode.Constants.OutgoingForfeitures.Id ? pd.Forfeiture : 0m,
                                pd.CommentTypeId,
                                pd.Remark,
                                pd.YearToDate,
                                pd.MonthToDate,
                                Date = pd.CreatedAtUtc,
                                nameAndDob.DateOfBirth,
                                HasForfeited = nameAndDob.EnrolledId == /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords ||
                                               nameAndDob.EnrolledId == /*4*/ Enrollment.Constants.NewVestingPlanHasForfeitureRecords,
                                nameAndDob.PayFrequencyId
                            };


                var totals = await query.GroupBy(_ => true)
                    .Select(g => new
                    {
                        DistributionTotal = g.Sum(x => x.DistributionAmount),
                        StateTaxTotal = g.Where(x => !string.IsNullOrEmpty(x.State)).Sum(x => x.StateTax),
                        FederalTaxTotal = g.Sum(x => x.FederalTax),
                        ForfeitureTotal = g.Sum(x => x.ForfeitAmount),
                        // MAIN-2170: Breakdown forfeitures by type
                        ForfeitureRegular = g.Where(x =>
                            x.ForfeitAmount != 0 &&
                            x.CommentTypeId != CommentType.Constants.ForfeitClassAction.Id &&
                            x.CommentTypeId != CommentType.Constants.ForfeitAdministrative.Id &&
                            (x.Remark == null || (!x.Remark.Contains("ADMINISTRATIVE") && !x.Remark.Contains("FORFEIT CA") && !x.Remark.Contains("UN-FORFEIT CA")))
                        ).Sum(x => x.ForfeitAmount),
                        ForfeitureAdministrative = g.Where(x =>
                            x.ForfeitAmount != 0 &&
                            (x.CommentTypeId == CommentType.Constants.ForfeitAdministrative.Id ||
                             (x.Remark != null && x.Remark.Contains("ADMINISTRATIVE")))
                        ).Sum(x => x.ForfeitAmount),
                        ForfeitureClassAction = g.Where(x =>
                            x.ForfeitAmount != 0 &&
                            (x.CommentTypeId == CommentType.Constants.ForfeitClassAction.Id ||
                             (x.Remark != null && (x.Remark.Contains("FORFEIT CA") || x.Remark.Contains("UN-FORFEIT CA"))))
                        ).Sum(x => x.ForfeitAmount)
                    })
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken) ?? new
                    {
                        DistributionTotal = 0m,
                        StateTaxTotal = 0m,
                        FederalTaxTotal = 0m,
                        ForfeitureTotal = 0m,
                        ForfeitureRegular = 0m,
                        ForfeitureAdministrative = 0m,
                        ForfeitureClassAction = 0m
                    };

                // Calculate state tax totals by state
                var allStateTaxRecords = await query
                    .Where(s => s.StateTax > 0)
                    .ToListAsync(cancellationToken: cancellationToken);

                // Separate unattributed (NULL state) records
                var unattributedRecords = allStateTaxRecords
                    .Where(r => string.IsNullOrEmpty(r.State))
                    .ToList();

                var unattributedTotals = new UnattributedTotals
                {
                    Count = unattributedRecords.Count,
                    FederalTax = unattributedRecords.Sum(x => x.FederalTax),
                    StateTax = unattributedRecords.Sum(x => x.StateTax),
                    NetProceeds = unattributedRecords.Sum(x => x.DistributionAmount) // Net proceeds from distribution amount
                };

                // Build state tax totals, excluding NULL states (will be tracked separately)
                var stateTaxTotals = allStateTaxRecords
                    .Where(s => !string.IsNullOrEmpty(s.State))
                    .GroupBy(x => x.State)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.StateTax));

                //// Check if sorting by Age - if so, we need to handle pagination manually since Age can't be calculated in SQL


                var sortReq = req;
                if (req.SortBy != null && "Age".Equals(req.SortBy, StringComparison.OrdinalIgnoreCase))
                {
                    sortReq = req with { SortBy = "DateOfBirth" };
                }
                // Normal database-level pagination for other sort fields
                var paginated = await query.ToPaginationResultsAsync(sortReq, cancellationToken);

                var apiResponse = paginated.Results.Select(pd => new DistributionsAndForfeitureResponse
                {
                    BadgePsn = pd.BadgePsn,
                    Ssn = pd.Ssn.MaskSsn(),
                    EmployeeName = pd.EmployeeName,
                    DistributionAmount = pd.DistributionAmount,
                    TaxCode = pd.TaxCode,
                    StateTax = pd.StateTax,
                    State = pd.State,
                    FederalTax = pd.FederalTax,
                    ForfeitAmount = pd.ForfeitAmount,
                    ForfeitType = pd.ForfeitAmount != 0
                        ? (pd.CommentTypeId == CommentType.Constants.ForfeitAdministrative.Id ||
                           (pd.Remark != null && pd.Remark.Contains("ADMINISTRATIVE")))
                            ? 'A'
                            : (pd.CommentTypeId == CommentType.Constants.ForfeitClassAction.Id ||
                               (pd.Remark != null && (pd.Remark.Contains("FORFEIT CA") || pd.Remark.Contains("UN-FORFEIT CA"))))
                                ? 'C'
                                : null
                        : null,
                    Date = pd.MonthToDate is > 0 and <= 12 ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1) : pd.Date.ToDateOnly(),
                    Age = (byte)(pd.MonthToDate is > 0 and < 13
                        ? pd.DateOfBirth.Age(
                            new DateOnly(pd.YearToDate, pd.MonthToDate, 1).ToDateTime(TimeOnly.MinValue))
                        : pd.DateOfBirth.Age(endDate.ToDateTime(TimeOnly.MinValue))),
                    HasForfeited = pd.HasForfeited,
                    IsExecutive = pd.PayFrequencyId == PayFrequency.Constants.Monthly
                });

                PaginatedResponseDto<DistributionsAndForfeitureResponse> paginatedResponse = new(req)
                {
                    Results = apiResponse.ToList(),
                    Total = paginated.Total
                };

                var response = new DistributionsAndForfeitureTotalsResponse()
                {
                    ReportName = ReportNameInfo.DistributionAndForfeitures.Name,
                    ReportDate = DateTimeOffset.UtcNow,
                    StartDate = startDate,
                    EndDate = endDate,
                    DistributionTotal = totals.DistributionTotal,
                    StateTaxTotal = totals.StateTaxTotal,
                    FederalTaxTotal = totals.FederalTaxTotal,
                    ForfeitureTotal = totals.ForfeitureTotal,
                    ForfeitureRegularTotal = totals.ForfeitureRegular,
                    ForfeitureAdministrativeTotal = totals.ForfeitureAdministrative,
                    ForfeitureClassActionTotal = totals.ForfeitureClassAction,
                    StateTaxTotals = stateTaxTotals,
                    UnattributedTotals = unattributedTotals.Count > 0 ? unattributedTotals : null,
                    HasUnattributedRecords = unattributedTotals.Count > 0,
                    Response = paginatedResponse
                };

                // Log unattributed records as business metric (PS-2031)
                if (unattributedTotals.Count > 0)
                {
                    _logger.LogWarning(
                        "Unattributed state records detected in GetDistributionsAndForfeitureAsync: {UnattributedCount} records with taxes but no state code. " +
                        "Total unattributed state taxes: {UnattributedStateTax}. This indicates data quality issues in state extraction (PS-2031).",
                        unattributedTotals.Count,
                        unattributedTotals.StateTax);
                }

                return Result<DistributionsAndForfeitureTotalsResponse>.Success(response);
            }, cancellationToken);

            return results;
        }
    }
}
