using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class UnforfeitService : IUnforfeitService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;
    private readonly ILogger<UnforfeitService> _logger;

    public UnforfeitService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService,
        ILogger<UnforfeitService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
        _logger = logger;
    }

    public async Task<ReportResponseBase<UnforfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(StartAndEndDateRequest req,
        CancellationToken cancellationToken)
    {
        StartAndEndDateRequestValidator? validator = new();
        await validator.ValidateAndThrowAsync(req, cancellationToken);

        PaginatedResponseDto<UnforfeituresResponse>? rehiredEmployees;

        // Time the database query execution
        using (DatabaseTelemetryScope.StartQuery("GetRehireUnforfeitures", nameof(UnforfeitService), "read"))
        {
            rehiredEmployees = await _dataContextFactory.UseReadOnlyContext(async context =>
            {
                IQueryable<ParticipantTotalYear>? yearsOfServiceQuery = _totalService.GetYearsOfService(context, req.ProfitYear, req.EndingDate);
                IQueryable<ParticipantTotalVestingBalance>? vestingServiceQuery = _totalService.TotalVestingBalance(context, req.ProfitYear, req.EndingDate);
                IQueryable<Demographic>? demo = await _demographicReaderService.BuildDemographicQuery(context);

                IQueryable<UnforfeituresResponse>? query =
                    // transactions
                    from pd in context.ProfitDetails

                        // the employee definition in the "profit year"
                    join d in demo on pd.Ssn equals d.Ssn
                    join ppYE in context.PayProfits.Include(p => p.Enrollment)
                        on new { d.Id, req.ProfitYear } equals new { Id = ppYE.DemographicId, ppYE.ProfitYear }

                        // Years of service as of "profit year"
                    join yos in yearsOfServiceQuery on d.Ssn equals yos.Ssn into yosTmp
                    from yos in yosTmp.DefaultIfEmpty()

                        // Vesting on req.EndingDate
                    join vest in vestingServiceQuery on d.Ssn equals vest.Ssn into vestTmp
                    from vest in vestTmp.DefaultIfEmpty()

                        // employee data at the time of the PROFIT_DETAIL transaction - ie. hours/wages (in transaction year)
                    join pp in context.PayProfits.Include(p => p.Enrollment)
                        on new { d.Id, pd.ProfitYear } equals new { Id = pp.DemographicId, pp.ProfitYear } into ppTmp
                    from pp in ppTmp.DefaultIfEmpty()
                    where
                        // only get the forfeit/unforfeit transactions 
                        pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id
                        // active
                        && d.EmploymentStatusId == EmploymentStatus.Constants.Active
                        // rehire range that user wanted
                        && d.ReHireDate >= req.BeginningDate && d.ReHireDate <= req.EndingDate
                        // exclude zero current/vest balance if so desired.
                        && (!req.ExcludeZeroBalance || vest != null && (vest.CurrentBalance != 0 || vest.VestedBalance != 0))
                    group new
                    {
                        d,
                        pp,
                        ppYE,
                        pd,
                        yos,
                        vest
                    } by new
                    {
                        d.BadgeNumber,
                        d.ContactInfo.FullName,
                        d.Ssn,
                        d.HireDate,
                        d.ReHireDate,
                        d.StoreNumber,
                        YearsOfService = yos != null ? yos.Years : (byte)0,
                        NetBalanceLastYear = vest != null ? vest.CurrentBalance ?? 0 : 0,
                        VestedBalanceLastYear = vest != null ? vest.VestedBalance ?? 0 : 0,
                        d.EmploymentStatusId,
                        d.PayFrequencyId,
                        ppYE.EnrollmentId,
                        EnrollmentName = ppYE.Enrollment!.Name,
                        HoursProfitYear = ppYE.HoursExecutive + ppYE.CurrentHoursYear,
                        WagesProfitYear = ppYE.IncomeExecutive + ppYE.CurrentIncomeYear
                    }
                    into g
                    select new UnforfeituresResponse
                    {
                        BadgeNumber = g.Key.BadgeNumber,
                        FullName = g.Key.FullName,
                        Ssn = g.Key.Ssn.MaskSsn(),
                        HireDate = g.Key.HireDate,
                        ReHiredDate = g.Key.ReHireDate ?? default,
                        StoreNumber = g.Key.StoreNumber,
                        CompanyContributionYears = g.Key.YearsOfService,
                        NetBalanceLastYear = g.Key.NetBalanceLastYear,
                        VestedBalanceLastYear = g.Key.VestedBalanceLastYear,
                        EnrollmentName = g.Key.EnrollmentName,
                        EnrollmentId = g.Key.EnrollmentId,
                        HoursProfitYear = g.Key.HoursProfitYear,
                        WagesProfitYear = g.Key.WagesProfitYear,
                        IsExecutive = g.Key.PayFrequencyId == PayFrequency.Constants.Monthly,
                        Details = g.Select(x => new RehireTransactionDetailResponse
                        {
                            ProfitYear = x.pd.ProfitYear,
                            HoursTransactionYear = x.pp != null ? x.pp.CurrentHoursYear : null,
                            Forfeiture = x.pd.Forfeiture,
                            Remark = x.pd.Remark,
                            ProfitCodeId = x.pd.ProfitCodeId,
                            WagesTransactionYear = x.pp != null ? x.pp.CurrentIncomeYear + x.pp.IncomeExecutive : null,
                            SuggestedUnforfeiture =
                                    // we only care about the latest forf/unforf transaction 
                                    x.pd.Id == g.Max(item => item.pd.Id) &&
                                    // check if this row is a forfeit
                                    x.pd.CommentType != null && x.pd.CommentType == CommentType.Constants.Forfeit &&
                                    // see if the employee qualifies for forfeiture
                                    (g.Key.YearsOfService == 1 && g.Key.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions
                                     || g.Key.EnrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords
                                     || g.Key.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords)
                                        ? x.pd.Forfeiture
                                        : null,
                            ProfitDetailId = x.pd.Id
                        })
                            .OrderByDescending(x => x.ProfitDetailId)
                            .ToList()
                    };

                var results = await query.ToPaginationResultsAsync(req, cancellationToken);

                // Record result count metric
                var resultCount = results.Results.Count();
                EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                    new("record_type", "unforfeiture"),
                    new("service", nameof(UnforfeitService)));

                return results;
            });
        }

        // Business logic: Build response wrapper (minimal processing)
        ReportResponseBase<UnforfeituresResponse> response;
        using (BusinessLogicTelemetryScope.Start("BuildReportResponse", nameof(UnforfeitService)))
        {
            response = new ReportResponseBase<UnforfeituresResponse>
            {
                ReportName = "REHIRE'S PROFIT SHARING DATA",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = req.BeginningDate,
                EndDate = req.EndingDate,
                Response = rehiredEmployees
            };
        }

        return response;
    }
}
