using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Utilities;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class PostFrozenService : IPostFrozenService
{
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly TotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger _logger;

    // Static readonly arrays for profit code filtering - zero per-instance allocation
    // EF Core translates array.Contains() to SQL IN clause
    private static readonly int[] s_earningsProfitCodes =
    [
        ProfitCode.Constants.IncomingContributions.Id,
        ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
        ProfitCode.Constants.OutgoingForfeitures.Id,
        ProfitCode.Constants.OutgoingDirectPayments.Id,
        ProfitCode.Constants.Incoming100PercentVestedEarnings.Id,
    ];

    private static readonly int[] s_contributionProfitCodes =
    [
        ProfitCode.Constants.IncomingContributions.Id,
        ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
        ProfitCode.Constants.OutgoingForfeitures.Id,
        ProfitCode.Constants.OutgoingDirectPayments.Id,
    ];

    private static readonly int[] s_distributionProfitCodes =
    [
        ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
        ProfitCode.Constants.OutgoingForfeitures.Id,
        ProfitCode.Constants.Outgoing100PercentVestedPayment.Id,
    ];

    public PostFrozenService(
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        TotalService totalService,
        ICalendarService calendarService,
        ILoggerFactory loggerFactory,
        IDemographicReaderService demographicReaderService
    )
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
        _demographicReaderService = demographicReaderService;
        _logger = loggerFactory.CreateLogger<PostFrozenService>();
    }

    public async Task<ProfitSharingUnder21ReportResponse> ProfitSharingUnder21Report(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var lastProfitYear = (short)(request.ProfitYear - 1);
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        var lastCalInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(lastProfitYear, cancellationToken);


        //Report uses the current date as the offset to calculate the age.
        var birthDate21 = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-21));

        // Helper function to build the base query for counts - each task will use its own context
        Func<IProfitSharingDbContext, Task<IQueryable<Under21IntermediaryResult>>> buildBaseQuery = async (ctx) =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            return from d in demographics.Where(x => x.DateOfBirth >= birthDate21)
                   join balTbl in _totalService.TotalVestingBalance(ctx, request.ProfitYear, request.ProfitYear, calInfo.FiscalEndDate) on d.Ssn equals balTbl.Ssn into balTmp
                   from bal in balTmp.DefaultIfEmpty()
                   join lyBalTbl in _totalService.TotalVestingBalance(ctx, lastProfitYear, lastProfitYear, calInfo.FiscalEndDate) on d.Ssn equals lyBalTbl.Ssn into lyBalTmp
                   from lyBal in lyBalTmp.DefaultIfEmpty()
                   where bal.YearsInPlan > 0 || bal.VestedBalance > 0
                   select new Under21IntermediaryResult() { d = d, bal = bal, lyBal = lyBal, };
        };

        // Execute all counts in parallel using separate contexts
        var totalUnder21Task = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            return await baseQuery.CountAsync(cancellationToken);
        }, cancellationToken);

        // Active counts - each in its own context
        var activeTotalVestedTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var activeQuery = baseQuery.Where(x => x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate);
            return await activeQuery.CountAsync(x => x.bal.YearsInPlan > 6 || x.lyBal.VestedBalance > 0, cancellationToken);
        }, cancellationToken);

        var activePartiallyVestedTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var activeQuery = baseQuery.Where(x => x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate);
            return await activeQuery.CountAsync(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 2 && x.bal.YearsInPlan < 6),
                cancellationToken);
        }, cancellationToken);


        var activePartiallyVestedButLessThanThreeYearsTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var activeQuery = baseQuery.Where(x => x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate);
            return await activeQuery.CountAsync(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 0 && x.bal.YearsInPlan < 3),
                cancellationToken);
        }, cancellationToken);

        // Inactive counts - each in its own context
        var inactiveTotalVestedTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var inactiveQuery = baseQuery.Where(x =>
                !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) &&
                x.d.EmploymentStatusId != EmploymentStatus.Constants.Terminated);
            return await inactiveQuery.CountAsync(x => (x.bal != null && x.bal.YearsInPlan > 6) || (x.lyBal != null && x.lyBal.VestedBalance > 0), cancellationToken);
        }, cancellationToken);

        var inactivePartiallyVestedTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var inactiveQuery = baseQuery.Where(x =>
                !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) &&
                x.d.EmploymentStatusId != EmploymentStatus.Constants.Terminated);
            return await inactiveQuery.CountAsync(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 2 && x.bal.YearsInPlan < 6),
                cancellationToken);
        }, cancellationToken);

        var inactivePartiallyVestedButLessThanThreeYearsTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var inactiveQuery = baseQuery.Where(x =>
                !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) &&
                x.d.EmploymentStatusId != EmploymentStatus.Constants.Terminated);
            return await inactiveQuery.CountAsync(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 0 && x.bal.YearsInPlan < 3),
                cancellationToken);
        }, cancellationToken);

        // Terminated counts - each in its own context
        var terminatedTotalVestedTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var terminatedQuery = baseQuery.Where(x =>
                !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) &&
                x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated);
            return await terminatedQuery.CountAsync(x => (x.bal != null && x.bal.YearsInPlan > 6) || (x.lyBal != null && x.lyBal.VestedBalance > 0), cancellationToken);
        }, cancellationToken);

        var terminatedPartiallyVestedTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var terminatedQuery = baseQuery.Where(x =>
                !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) &&
                x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated);
            return await terminatedQuery.CountAsync(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 2 && x.bal.YearsInPlan < 6),
                cancellationToken);
        }, cancellationToken);

        var terminatedPartiallyVestedButLessThanThreeYearsTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = await buildBaseQuery(ctx);
            var terminatedQuery = baseQuery.Where(x =>
                !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) &&
                x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated);
            return await terminatedQuery.CountAsync(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 0 && x.bal.YearsInPlan < 3),
                cancellationToken);
        }, cancellationToken);

        // Page data query in its own context
        var pagedDataTask = _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var sortRequest = request with
            {
                SortBy = request.SortBy switch
                {
                    "profitSharingYears" => "YearsInPlan",
                    "isNew" => "IsNewLastYear",
                    "thisYearHours" => "CurrentYearHours",
                    "age" => "DateOfBirth",
                    _ => request.SortBy,
                }
            };

            // First get anonymous type from database query (no Age() or MaskSsn() calls in query)
            var rawPagedData = await (
                from d in demographics
                    .Include(d => d.EmploymentStatus)
                    .Where(x => x.DateOfBirth >= birthDate21)
                join bal in _totalService.TotalVestingBalance(ctx, request.ProfitYear, request.ProfitYear, calInfo.FiscalEndDate) on d.Ssn equals bal.Ssn
                join lyPpTbl in ctx.PayProfits.Include(pp => pp.Enrollment).Where(x => x.ProfitYear == request.ProfitYear - 1) on d.Id equals lyPpTbl.DemographicId into lyPpTmp
                from lyPp in lyPpTmp.DefaultIfEmpty()
                join tyPpTbl in ctx.PayProfits.Where(x => x.ProfitYear == request.ProfitYear) on d.Id equals tyPpTbl.DemographicId into tyPpTmp
                from tyPp in tyPpTmp.DefaultIfEmpty()
                where bal.YearsInPlan > 0 || bal.VestedBalance > 0
                orderby d.StoreNumber, d.ContactInfo.FullName
                select new
                {
                    d.StoreNumber,
                    d.BadgeNumber,
                    d.ContactInfo.FirstName,
                    d.ContactInfo.LastName,
                    d.Ssn, // Raw SSN - will be masked after query
                    YearsInPlan = (bal.YearsInPlan ?? 0),
                    IsNewLastYear = d.EmploymentTypeId.ToString() == EmployeeType.Constants.NewLastYear.ToString(),
                    CurrentYearHours = tyPp != null ? tyPp.CurrentHoursYear : 0,
                    LastYearHours = lyPp != null ? lyPp.CurrentHoursYear : 0,
                    d.HireDate,
                    d.FullTimeDate,
                    d.TerminationDate,
                    d.DateOfBirth, // Raw birth date - age will be calculated after query
                    EmploymentStatusId = d.EmploymentStatus!.Name,
                    CurrentBalance = (bal.CurrentBalance ?? 0),
                    EnrollmentId = tyPp != null ? tyPp.Enrollment!.Name : "",
                    IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly
                }
            ).ToPaginationResultsAsync(sortRequest, cancellationToken: cancellationToken);

            // Then project to final type with post-query SSN masking and age calculation
            return new PaginatedResponseDto<ProfitSharingUnder21ReportDetail>(request)
            {
                Total = rawPagedData.Total,
                Results = rawPagedData.Results.Select(x => new ProfitSharingUnder21ReportDetail
                {
                    StoreNumber = x.StoreNumber,
                    BadgeNumber = x.BadgeNumber,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Ssn = x.Ssn.MaskSsn(), // SSN masking after database query
                    ProfitSharingYears = x.YearsInPlan,
                    IsNew = x.IsNewLastYear,
                    ThisYearHours = x.CurrentYearHours,
                    LastYearHours = x.LastYearHours,
                    HireDate = x.HireDate,
                    FullTimeDate = x.FullTimeDate,
                    TerminationDate = x.TerminationDate,
                    DateOfBirth = x.DateOfBirth,
                    Age = x.DateOfBirth.Age(), // Age calculation after database query
                    EmploymentStatusId = x.EmploymentStatusId,
                    CurrentBalance = x.CurrentBalance,
                    EnrollmentId = x.EnrollmentId,
                    IsExecutive = x.IsExecutive
                })
            };
        }, cancellationToken);

        // Await all parallel operations
        await Task.WhenAll(
            totalUnder21Task,
            activeTotalVestedTask, activePartiallyVestedTask, activePartiallyVestedButLessThanThreeYearsTask,
            inactiveTotalVestedTask, inactivePartiallyVestedTask, inactivePartiallyVestedButLessThanThreeYearsTask,
            terminatedTotalVestedTask, terminatedPartiallyVestedTask, terminatedPartiallyVestedButLessThanThreeYearsTask,
            pagedDataTask
        );

        // Get results from tasks
        var totalUnder21 = await totalUnder21Task;
        var activeTotalVested = await activeTotalVestedTask;
        var activePartiallyVested = await activePartiallyVestedTask;
        var activePartiallyVestedButLessThanThreeYears = await activePartiallyVestedButLessThanThreeYearsTask;
        var inactiveTotalVested = await inactiveTotalVestedTask;
        var inactivePartiallyVested = await inactivePartiallyVestedTask;
        var inactivePartiallyVestedButLessThanThreeYears = await inactivePartiallyVestedButLessThanThreeYearsTask;
        var terminatedTotalVested = await terminatedTotalVestedTask;
        var terminatedPartiallyVested = await terminatedPartiallyVestedTask;
        var terminatedPartiallyVestedButLessThanThreeYears = await terminatedPartiallyVestedButLessThanThreeYearsTask;
        var pagedData = await pagedDataTask;

        // Assemble final response
        var rslt = new ProfitSharingUnder21ReportResponse
        {
            ActiveTotals = new ProfitSharingUnder21TotalForStatus(activeTotalVested, activePartiallyVested, activePartiallyVestedButLessThanThreeYears),
            InactiveTotals = new ProfitSharingUnder21TotalForStatus(inactiveTotalVested, inactivePartiallyVested, inactivePartiallyVestedButLessThanThreeYears),
            TerminatedTotals = new ProfitSharingUnder21TotalForStatus(terminatedTotalVested, terminatedPartiallyVested, terminatedPartiallyVestedButLessThanThreeYears),
            TotalUnder21 = totalUnder21,
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = lastCalInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            ReportName = ProfitSharingUnder21ReportResponse.REPORT_NAME,
            Response = pagedData
        };
        return rslt;
    }

    public async Task<ReportResponseBase<ProfitSharingUnder21BreakdownByStoreResponse>> ProfitSharingUnder21BreakdownByStore(ProfitYearRequest request,
        CancellationToken cancellation)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellation);
        var age21 = calInfo.FiscalEndDate.AddYears(-21);
        short lastYear = (short)(request.ProfitYear - 1);
        var lastCalInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(lastYear, cancellation);

        var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {

            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

            var qry = (
                from pp in ctx.PayProfits.Where(x => x.ProfitYear == request.ProfitYear)
                join d in demographics on pp.DemographicId equals d.Id
                join lyTotTbl in _totalService.GetTotalBalanceSet(ctx, lastYear) on d.Ssn equals lyTotTbl.Ssn into lyTotTmp
                from lyTot in lyTotTmp.DefaultIfEmpty()
                join tyTotTbl in _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate) on d.Ssn equals tyTotTbl.Ssn into tyTotalTmp
                from tyTot in tyTotalTmp.DefaultIfEmpty()
                join tyPdGrpTbl in (
                    from pd in ctx.ProfitDetails.Where(x => x.ProfitYear == request.ProfitYear)
                    group pd by pd.Ssn
                    into pdGrp
                    select new
                    {
                        Ssn = pdGrp.Key,
                        Earnings = pdGrp.Where(x => s_earningsProfitCodes.Contains(x.ProfitCodeId)).Sum(x => x.Earnings),
                        Contributions = pdGrp.Where(x => s_contributionProfitCodes.Contains(x.ProfitCodeId)).Sum(x => x.Contribution),
                        Forfeitures = pdGrp.Where(x => x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id).Sum(x => x.Forfeiture) -
                                      pdGrp.Where(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id).Sum(x => x.Forfeiture),
                        Distributions = pdGrp.Where(x => s_distributionProfitCodes.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture * -1)
                    }
                ) on d.Ssn equals tyPdGrpTbl.Ssn into tyPdGrpTmp
                from tyPdGrp in tyPdGrpTmp.DefaultIfEmpty()
                where d.DateOfBirth > age21
                orderby d.StoreNumber, d.ContactInfo.LastName, d.ContactInfo.FirstName
                select new ProfitSharingUnder21BreakdownByStoreResponse()
                {
                    StoreNumber = d.StoreNumber,
                    BadgeNumber = d.BadgeNumber,
                    FullName = d.ContactInfo.FullName ?? string.Empty,
                    BeginningBalance = lyTot.TotalAmount ?? 0,
                    Earnings = tyPdGrp.Earnings,
                    Contributions = tyPdGrp.Contributions,
                    Forfeitures = tyPdGrp.Forfeitures,
                    Distributions = tyPdGrp.Distributions,
                    VestedAmount = tyTot.VestedBalance,
                    EndingBalance = tyTot.CurrentBalance,
                    VestingPercentage = tyTot.VestingPercent * 100,
                    DateOfBirth = d.DateOfBirth,
                    Age = 0, //To be determined after materializing
                    EnrollmentId = pp.EnrollmentId,
                    IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly
                }
            );
            var pagedResults = await qry.ToPaginationResultsAsync(request, cancellation);
            var fiscalEndDateTime = calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
            foreach (var row in pagedResults.Results)
            {
                row.Age = (byte)row.DateOfBirth.Age(fiscalEndDateTime);
            }

            return pagedResults;
        }, cancellation);

        return new ReportResponseBase<ProfitSharingUnder21BreakdownByStoreResponse>()
        {
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = lastCalInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            ReportName = ProfitSharingUnder21BreakdownByStoreResponse.REPORT_NAME,
            Response = rslt
        };
    }

    public async Task<ReportResponseBase<ProfitSharingUnder21InactiveNoBalanceResponse>> ProfitSharingUnder21InactiveNoBalance(ProfitYearRequest request,
        CancellationToken cancellationToken)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        var age21 = calInfo.FiscalEndDate.AddYears(-21);
        var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);

            return await (
                from d in demographicQuery.Where(x => x.DateOfBirth >= age21)
                join pp in ctx.PayProfits.Where(x => x.ProfitYear == request.ProfitYear) on d.Id equals pp.DemographicId
                join balTbl in _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate)
                    on d.Ssn equals balTbl.Ssn into balTmp
                from bal in balTmp.DefaultIfEmpty()
                where
                    d.TerminationCodeId != TerminationCode.Constants.Retired &&
                    (
                        d.EmploymentStatusId == EmploymentStatus.Constants.Inactive ||
                        (
                            d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                        )
                    )
                    && (bal.VestedBalance > 0 || bal.YearsInPlan > 0)
                    && (bal.CurrentBalance <= 0)
                orderby d.ContactInfo.LastName, d.ContactInfo.FirstName
                select new ProfitSharingUnder21InactiveNoBalanceResponse()
                {
                    BadgeNumber = d.BadgeNumber,
                    LastName = d.ContactInfo.LastName,
                    FirstName = d.ContactInfo.FirstName,
                    FullName = d.ContactInfo.FullName ?? string.Empty,
                    BirthDate = d.DateOfBirth,
                    HireDate = d.HireDate,
                    TerminationDate = d.TerminationDate,
                    Age = 0,
                    EnrollmentId = pp.EnrollmentId,
                    IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                }
            ).ToPaginationResultsAsync(request, cancellationToken);
        }, cancellationToken);

        var fiscalEndDateTime = calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
        foreach (var row in rslt.Results)
        {
            row.Age = (byte)row.BirthDate.Age(fiscalEndDateTime);
        }

        return new ReportResponseBase<ProfitSharingUnder21InactiveNoBalanceResponse>
        {
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            ReportName = ProfitSharingUnder21InactiveNoBalanceResponse.REPORT_NAME,
            Response = rslt
        };
    }

    public async Task<ProfitSharingUnder21TotalsResponse> GetUnder21Totals(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        var age21 = calInfo.FiscalEndDate.AddYears(-21);
        short lastYear = (short)(request.ProfitYear - 1);
        var rslt = new ProfitSharingUnder21TotalsResponse();

        _ = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);

            var rootQuery = from d in demographicQuery.Where(x => x.DateOfBirth >= age21)
                            join pp in ctx.PayProfits.Where(x => x.ProfitYear == request.ProfitYear) on d.Id equals pp.DemographicId
                            select new { d, pp };
            var baseQuery = from r in rootQuery
                            join bal in _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate) on r.d.Ssn equals bal.Ssn
                            where bal.YearsInPlan > 0 || bal.VestedBalance > 0
                            select new { r.d, r.pp, bal };

            rslt.NumberOfEmployees = await baseQuery.CountAsync(cancellationToken);
            rslt.NumberOfActiveUnder21With1to2Years = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Active ||
                        (x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated && x.d.TerminationDate > calInfo.FiscalEndDate)
                    ) &&
                    x.bal.YearsInPlan >= 1 &&
                    x.bal.YearsInPlan <= 2,
                cancellationToken);
            rslt.NumberOfActiveUnder21With20to80PctVested = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Active ||
                        (x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated && x.d.TerminationDate > calInfo.FiscalEndDate)
                    ) &&
                    x.bal.VestingPercent >= .2m &&
                    x.bal.VestingPercent <= .8m,
                cancellationToken);
            rslt.NumberOfActiveUnder21With100PctVested = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Active ||
                        (x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated && x.d.TerminationDate > calInfo.FiscalEndDate)
                    ) &&
                    x.bal.VestingPercent == 1,
                cancellationToken);

            rslt.NumberOfInActiveUnder21With1to2Years = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Inactive &&
                        x.d.TerminationDate <= calInfo.FiscalEndDate
                    ) &&
                    x.bal.YearsInPlan >= 1 &&
                    x.bal.YearsInPlan <= 2,
                cancellationToken);
            rslt.NumberOfInActiveUnder21With20to80PctVested = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Inactive &&
                        x.d.TerminationDate <= calInfo.FiscalEndDate
                    ) &&
                    x.bal.VestingPercent >= .2m &&
                    x.bal.VestingPercent <= .8m,
                cancellationToken);
            rslt.NumberOfInActiveUnder21With100PctVested = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Inactive &&
                        x.d.TerminationDate <= calInfo.FiscalEndDate
                    ) &&
                    x.bal.VestingPercent == 1,
                cancellationToken);

            rslt.NumberOfTerminatedUnder21With1to2Years = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                        x.d.TerminationDate <= calInfo.FiscalEndDate
                    ) &&
                    x.bal.YearsInPlan >= 1 &&
                    x.bal.YearsInPlan <= 2,
                cancellationToken);
            rslt.NumberOfTerminatedUnder21With20to80PctVested = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                        x.d.TerminationDate <= calInfo.FiscalEndDate
                    ) &&
                    x.bal.VestingPercent >= .2m &&
                    x.bal.VestingPercent <= .8m,
                cancellationToken);
            rslt.NumberOfTerminatedUnder21With100PctVested = await baseQuery.CountAsync(x =>
                    (
                        x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                        x.d.TerminationDate <= calInfo.FiscalEndDate
                    ) &&
                    x.bal.VestingPercent == 1,
                cancellationToken);

            rslt.TotalBeginningBalance = await (
                from b in rootQuery
                join bal in _totalService.GetTotalBalanceSet(ctx, lastYear) on b.d.Ssn equals bal.Ssn
                group bal by true
                into grp
                select grp.Sum(x => x.TotalAmount)
            ).FirstOrDefaultAsync(cancellationToken);

            rslt.TotalEarnings = await (
                from b in rootQuery
                join pd in ctx.ProfitDetails on b.d.Ssn equals pd.Ssn
                where s_earningsProfitCodes.Contains(pd.ProfitCodeId)
                group pd by true
                into grp
                select grp.Sum(x => x.Earnings)
            ).FirstOrDefaultAsync(cancellationToken);

            rslt.TotalContributions = await (
                from b in rootQuery
                join pd in ctx.ProfitDetails on b.d.Ssn equals pd.Ssn
                where s_contributionProfitCodes.Contains(pd.ProfitCodeId)
                group pd by true
                into grp
                select grp.Sum(x => x.Contribution)
            ).FirstOrDefaultAsync(cancellationToken);

            rslt.TotalForfeitures = await (
                from b in rootQuery
                join pd in ctx.ProfitDetails on b.d.Ssn equals pd.Ssn
                where pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                group pd by true
                into grp
                select grp.Sum(x => x.Forfeiture)
            ).FirstOrDefaultAsync(cancellationToken);

            rslt.TotalForfeitures -= await (
                from b in rootQuery
                join pd in ctx.ProfitDetails on b.d.Ssn equals pd.Ssn
                where pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id
                group pd by true
                into grp
                select grp.Sum(x => x.Forfeiture)
            ).FirstOrDefaultAsync(cancellationToken);

            rslt.TotalDisbursements = await (
                from b in rootQuery
                join pd in ctx.ProfitDetails on b.d.Ssn equals pd.Ssn
                where s_distributionProfitCodes.Contains(pd.ProfitCodeId)
                group pd by true
                into grp
                select grp.Sum(x => x.Forfeiture * -1)
            ).FirstOrDefaultAsync(cancellationToken);


            rslt.TotalEndingBalance = await (
                from b in rootQuery
                join bal in _totalService.GetTotalBalanceSet(ctx, request.ProfitYear) on b.d.Ssn equals bal.Ssn
                group bal by true
                into grp
                select grp.Sum(x => x.TotalAmount)
            ).FirstOrDefaultAsync(cancellationToken);

            rslt.TotalVestingBalance = await (
                from b in rootQuery
                join bal in _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate) on b.d.Ssn equals bal.Ssn
                group bal by true
                into grp
                select grp.Sum(x => x.VestedBalance)
            ).FirstOrDefaultAsync(cancellationToken);



            return true;
        }, cancellationToken);

        return rslt;
    }

    public async Task<List<string>> GetNewProfitSharingLabelsForMailMerge(ProfitYearRequest request, CancellationToken ct)
    {
        var people = await GetNewProfitSharingLabels(request, ct);
        var resultsList = people.Results?.ToList() ?? [];
        var rslt = new List<string>(resultsList.Count * 4); // Pre-allocate capacity

        foreach (var person in resultsList)
        {
            // Use string.Create for better performance with formatting
            rslt.Add(string.Create(
                System.Globalization.CultureInfo.InvariantCulture,
                $"{person.StoreNumber:000}-{person.DepartmentId:0}-{person.PayClassificationId}-{person.BadgeNumber}"));
            rslt.Add(person.EmployeeName);
            rslt.Add(person.Address1 ?? string.Empty);
            rslt.Add(string.Create(
                System.Globalization.CultureInfo.InvariantCulture,
                $"{person.City ?? string.Empty}, {person.State ?? string.Empty}, {person.PostalCode ?? string.Empty}"));
        }

        return rslt;
    }

    public async Task<PaginatedResponseDto<NewProfitSharingLabelResponse>> GetNewProfitSharingLabels(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("Request NEW PROFIT SHARING EMPLOYEE LABEL REPORT"))
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var age21 = calInfo.FiscalEndDate.AddYears(-21);
            var rslt = await (_profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var qry = (
                    from pp in ctx.PayProfits
                        .Include(x => x.Demographic)
                        .Include(x => x.Demographic!.Address)
                        .Include(x => x.Demographic!.PayClassification)
                        .Include(x => x.Demographic!.Department)
                        .Include(x => x.Demographic!.EmploymentType)

                    where pp.Demographic!.DateOfBirth > age21 || pp.EmployeeTypeId == EmployeeType.Constants.NewLastYear
                    orderby pp.Demographic!.StoreNumber, pp.Demographic!.ContactInfo.LastName, pp.Demographic!.ContactInfo.FirstName
                    select new
                    {
                        pp.Demographic!.StoreNumber,
                        pp.Demographic.PayClassificationId,
                        PayClassificationName = pp.Demographic.PayClassification!.Name,
                        pp.Demographic.DepartmentId,
                        DepartmentName = pp.Demographic.Department!.Name,
                        pp.Demographic.BadgeNumber,
                        pp.Demographic.Ssn,
                        EmployeeName = string.IsNullOrEmpty(pp.Demographic.ContactInfo.MiddleName) ? pp.Demographic.ContactInfo.FirstName + " " + pp.Demographic.ContactInfo.LastName : pp.Demographic.ContactInfo.FirstName + " " + pp.Demographic.ContactInfo.MiddleName[0] + " " + pp.Demographic.ContactInfo.LastName,
                        EmployeeTypeId = pp.Demographic.EmploymentTypeId,
                        EmployeeTypeName = pp.Demographic.EmploymentType!.Name,
                        Hours = pp.CurrentHoursYear,
                        pp.Demographic.Address.Street,
                        pp.Demographic.Address.City,
                        pp.Demographic.Address.State,
                        pp.Demographic.Address.PostalCode,
                        pp.Demographic!.PayFrequencyId,
                    }
                );

                var rawData = await qry.ToPaginationResultsAsync(request, cancellationToken);
                var ssns = rawData.Results.Select(x => x.Ssn).ToList();

                var balanceInfo = await _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate).Where(x => ssns.Contains(x.Ssn))
                    .ToListAsync(cancellationToken);


                return new PaginatedResponseDto<NewProfitSharingLabelResponse>(request)
                {
                    Total = rawData.Total,
                    Results = (
                        from r in rawData.Results
                        join bTbl in balanceInfo on r.Ssn equals bTbl.Ssn into bTmp
                        from b in bTmp.DefaultIfEmpty()
                        select new NewProfitSharingLabelResponse()
                        {
                            StoreNumber = r.StoreNumber,
                            PayClassificationId = r.PayClassificationId,
                            PayClassificationName = r.PayClassificationName,
                            DepartmentId = r.DepartmentId,
                            DepartmentName = r.DepartmentName,
                            BadgeNumber = r.BadgeNumber,
                            Ssn = r.Ssn.MaskSsn(),
                            EmployeeName = r.EmployeeName,
                            EmployeeTypeId = r.EmployeeTypeId,
                            EmployeeTypeName = r.EmployeeTypeName,
                            Hours = r.Hours,
                            Balance = b != null && b.CurrentBalance != null ? b.CurrentBalance : 0,
                            Years = b != null && b.YearsInPlan != null ? b.YearsInPlan : 0,
                            Address1 = r.Street,
                            City = r.City,
                            State = r.State,
                            PostalCode = r.PostalCode,
                            IsExecutive = r.PayFrequencyId == PayFrequency.Constants.Monthly
                        }
                    )
                };
            }, cancellationToken));

            return rslt;
        }
    }

    public async Task<List<string>> GetProfitSharingLabelsExport(ProfitYearRequest request, CancellationToken ct)
    {
        var rawData = await GetProfitSharingLabels(request, ct);

        // Use CsvStringHandler for better performance - eliminates intermediate allocations
        var rslt = rawData.Results.Select(x =>
        {
            var csv = new CsvStringHandler(10);
            csv.AppendField(x.EmployeeName);
            csv.AppendField(x.Address1);
            csv.AppendField(x.City);
            csv.AppendField(x.State);
            csv.AppendField(x.PostalCode);
            csv.AppendField(x.FirstName);
            csv.AppendField(x.StoreNumber, "000");
            csv.AppendField(x.DepartmentId, "0");
            csv.AppendField(x.PayClassificationId);
            csv.AppendField(x.BadgeNumber);
            return csv.ToString();
        }).ToList();

        return rslt;
    }

    public async Task<PaginatedResponseDto<ProfitSharingLabelResponse>> GetProfitSharingLabels(ProfitYearRequest request, CancellationToken ct)
    {
        using (_logger.BeginScope("Request PROFIT SHARING EMPLOYEE LABEL REPORT"))
        {
            return await (_profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);

                var demoInfo = (
                    from d in demographicQuery
                    join pc in ctx.PayClassifications on d.PayClassificationId equals pc.Id
                    join dp in ctx.Departments on d.DepartmentId equals dp.Id
                    select new
                    {
                        d.Ssn,
                        d.StoreNumber,
                        d.PayClassificationId,
                        PayClassificationName = pc.Name,
                        d.DepartmentId,
                        DepartmentName = dp.Name,
                        d.BadgeNumber,
                        d.ContactInfo.FirstName,
                        d.ContactInfo.LastName,
                        d.ContactInfo.MiddleName,
                        Address1 = d.Address.Street,
                        d.Address.City,
                        State = d.Address.Street,
                        d.Address.PostalCode,
                        d.PayFrequencyId,
                    }
                );

                var beneInfo = (
                    from bc in ctx.BeneficiaryContacts
                    join b in ctx.Beneficiaries on bc.Id equals b.BeneficiaryContactId
                    where !demographicQuery.Any(d => d.Ssn == bc.Ssn)
                    select new
                    {
                        bc.Ssn,
                        StoreNumber = (short)0,
                        PayClassificationId = string.Empty,
                        PayClassificationName = "",
                        DepartmentId = (byte)0,
                        DepartmentName = "",
                        b.BadgeNumber,
                        bc.ContactInfo.FirstName,
                        bc.ContactInfo.LastName,
                        bc.ContactInfo.MiddleName,
                        Address1 = bc.Address.Street,
                        bc.Address.City,
                        bc.Address.State,
                        bc.Address.PostalCode,
                        PayFrequencyId = (byte)0,
                    }
                );

                var demoAndBeneficiaries = demoInfo.Union(beneInfo);

                return await (
                    from pd in ctx.ProfitDetails.Where(x => x.ProfitYear == request.ProfitYear).Select(x => x.Ssn).Distinct()
                    join d in demoAndBeneficiaries on pd equals d.Ssn
                    join pc in ctx.PayClassifications on d.PayClassificationId equals pc.Id
                    join dp in ctx.Departments on d.DepartmentId equals dp.Id
                    orderby d.StoreNumber, d.LastName, d.FirstName
                    select new ProfitSharingLabelResponse()
                    {
                        StoreNumber = d.StoreNumber,
                        PayClassificationId = d.PayClassificationId,
                        PayClassificationName = pc.Name,
                        DepartmentId = d.DepartmentId,
                        DepartmentName = dp.Name,
                        BadgeNumber = d.BadgeNumber,
                        EmployeeName = string.IsNullOrEmpty(d.MiddleName) ? d.FirstName + " " + d.LastName : d.FirstName + " " + d.MiddleName[0] + " " + d.LastName,
                        FirstName = d.FirstName,
                        Address1 = d.Address1,
                        City = d.City,
                        State = d.State,
                        PostalCode = d.PostalCode,
                        IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly
                    }
                ).ToPaginationResultsAsync(request, ct);
            }, ct));
        }
    }

    internal class Under21IntermediaryResult
    {
        internal required Demographic d { get; init; }
        internal required ParticipantTotalVestingBalance bal { get; init; }
        internal required ParticipantTotalVestingBalance lyBal { get; init; }
    }
}
