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
using System.Linq.Dynamic.Core;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class UnforfeitService : IUnforfeitService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public UnforfeitService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
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

                // PERFORMANCE: Pre-filter demographics to reduce join volume
                IQueryable<Demographic>? activeDemographics = demo
                    .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Active)
                    .Where(d => d.ReHireDate >= req.BeginningDate && d.ReHireDate <= req.EndingDate);

                // PERFORMANCE: Pre-filter ProfitDetails to only forfeitures before joining
                IQueryable<ProfitDetail>? forfeitureTransactions = context.ProfitDetails
                    .Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id);

                // PERFORMANCE: Phase 1 - Get main employee data WITHOUT Details (much faster)
                var mainQuery =
                    from pd in forfeitureTransactions
                    join d in activeDemographics on pd.Ssn equals d.Ssn
                    join ppYE in context.PayProfits
                        on new { d.Id, req.ProfitYear } equals new { Id = ppYE.DemographicId, ppYE.ProfitYear }
                    join enrollment in context.Enrollments
                        on ppYE.EnrollmentId equals enrollment.Id
                    join yos in yearsOfServiceQuery on d.Ssn equals yos.Ssn into yosTmp
                    from yos in yosTmp.DefaultIfEmpty()
                    join vest in vestingServiceQuery on d.Ssn equals vest.Ssn into vestTmp
                    from vest in vestTmp.DefaultIfEmpty()
                    where !req.ExcludeZeroBalance || vest != null && (vest.CurrentBalance != 0 || vest.VestedBalance != 0)
                    select new
                    {
                        d.BadgeNumber,
                        d.ContactInfo.FullName,
                        d.Ssn,
                        d.HireDate,
                        d.ReHireDate,
                        d.StoreNumber,
                        d.Id,
                        YearsOfService = yos != null ? yos.Years : (byte)0,
                        NetBalanceLastYear = vest != null ? vest.CurrentBalance ?? 0 : 0,
                        VestedBalanceLastYear = vest != null ? vest.VestedBalance ?? 0 : 0,
                        d.PayFrequencyId,
                        ppYE.EnrollmentId,
                        EnrollmentName = enrollment.Name,
                        HoursProfitYear = ppYE.HoursExecutive + ppYE.CurrentHoursYear,
                        WagesProfitYear = ppYE.IncomeExecutive + ppYE.CurrentIncomeYear
                    };

                // Apply pagination to main query
                var sortBy = (req.SortBy ?? "badgenumber").ToLowerInvariant() switch {
                    "rehireddate" => "RehireDate",
                    "companycontributionyears" => "YearsOfService",
                    _ => (req.SortBy ?? "")
                };

                if (req.IsSortDescending ?? false)
                {
                    sortBy += " DESC";
                }
                

                var paginatedMain = await mainQuery
                    .Distinct() // Remove duplicates from multiple ProfitDetail rows
                    .OrderBy(sortBy)
                    .Skip(req.Skip ?? 0)
                    .Take(req.Take ?? 10)
                    .ToListAsync(cancellationToken);

                if (!paginatedMain.Any())
                {
                    return new PaginatedResponseDto<UnforfeituresResponse>(req)
                    {
                        Results = new List<UnforfeituresResponse>(),
                        Total = 0
                    };
                }

                // Get total count for pagination
                var totalCount = await mainQuery.Select(x => x.Ssn).Distinct().CountAsync(cancellationToken);

                // PERFORMANCE: Phase 2 - Get Details for only the SSNs in the result set
                var ssnList = paginatedMain.Select(x => x.Ssn).Distinct().ToList();

                var detailsQuery =
                    from pd in forfeitureTransactions
                    where ssnList.Contains(pd.Ssn)
                    join d in activeDemographics on pd.Ssn equals d.Ssn
                    join pp in context.PayProfits
                        on new { d.Id, pd.ProfitYear } equals new { Id = pp.DemographicId, pp.ProfitYear } into ppTmp
                    from pp in ppTmp.DefaultIfEmpty()
                    select new
                    {
                        pd.Ssn,
                        pd.Id,
                        pd.ProfitYear,
                        pd.Forfeiture,
                        pd.Remark,
                        pd.ProfitCodeId,
                        pd.CommentType,
                        HoursTransactionYear = pp != null ? (decimal?)pp.CurrentHoursYear : null,
                        WagesTransactionYear = pp != null ? (decimal?)(pp.CurrentIncomeYear + pp.IncomeExecutive) : null
                    };

                var allDetails = await detailsQuery.ToListAsync(cancellationToken);

                // Group details by SSN for efficient lookup
                var detailsBySsn = allDetails
                    .GroupBy(x => x.Ssn)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // PERFORMANCE: Phase 3 - Combine in memory (fast for small result sets)
                var responses = paginatedMain.Select(main =>
                {
                    detailsBySsn.TryGetValue(main.Ssn, out var details);
                    var maxProfitDetailId = (details?.Any() ?? false) ? details.Max(d => (int)d.Id) : 0;

                    return new UnforfeituresResponse
                    {
                        BadgeNumber = main.BadgeNumber,
                        FullName = main.FullName,
                        Ssn = main.Ssn.MaskSsn(),
                        HireDate = main.HireDate,
                        ReHiredDate = main.ReHireDate ?? default,
                        StoreNumber = main.StoreNumber,
                        CompanyContributionYears = main.YearsOfService,
                        NetBalanceLastYear = main.NetBalanceLastYear,
                        VestedBalanceLastYear = main.VestedBalanceLastYear,
                        EnrollmentName = main.EnrollmentName,
                        EnrollmentId = main.EnrollmentId,
                        HoursProfitYear = main.HoursProfitYear,
                        WagesProfitYear = main.WagesProfitYear,
                        IsExecutive = main.PayFrequencyId == PayFrequency.Constants.Monthly,
                        Details = (details ?? Enumerable.Empty<dynamic>())
                            .Select(d => new RehireTransactionDetailResponse
                            {
                                ProfitYear = d.ProfitYear,
                                HoursTransactionYear = d.HoursTransactionYear,
                                Forfeiture = d.Forfeiture,
                                Remark = d.Remark,
                                ProfitCodeId = d.ProfitCodeId,
                                WagesTransactionYear = d.WagesTransactionYear,
                                SuggestedUnforfeiture =
                                    d.Id == maxProfitDetailId &&
                                    d.CommentType != null && d.CommentType == CommentType.Constants.Forfeit &&
                                    (main.YearsOfService == 1 && main.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions
                                     || main.EnrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords
                                     || main.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords)
                                        ? d.Forfeiture
                                        : null,
                                ProfitDetailId = d.Id
                            })
                            .OrderByDescending(x => x.ProfitDetailId)
                            .ToList()
                    };
                }).ToList();

                var results = new PaginatedResponseDto<UnforfeituresResponse>(req)
                {
                    Results = responses,
                    Total = totalCount
                };

                // Record result count metric
                EndpointTelemetry.RecordCountsProcessed.Record(results.Results.Count(),
                    new("record_type", "unforfeiture"),
                    new("service", nameof(UnforfeitService)));

                return results;
            }, cancellationToken);
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
