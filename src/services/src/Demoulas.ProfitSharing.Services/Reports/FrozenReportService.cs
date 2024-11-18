using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class FrozenReportService : IFrozenReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ContributionService _contributionService;
    private readonly ILogger _logger;

    public FrozenReportService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory loggerFactory,
        ContributionService contributionService
    )
    {
        _dataContextFactory = dataContextFactory;
        _contributionService = contributionService;
        _logger = loggerFactory.CreateLogger<FrozenReportService>();
    }

    public async Task<ReportResponseBase<ForfeituresAndPointsForYearResponse>> GetForfeituresAndPointsForYear(ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request FORFEITURES AND POINTS FOR YEAR"))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {

                var forfeitures = ctx.ProfitDetails
                    .Where(pd => pd.ProfitYear == req.ProfitYear)
                    .Where(pd =>
                        (pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
                    )
                    .Join(ctx.Demographics, x => x.Ssn, x => x.Ssn, (pd, d) => new { pd, d })
                    .GroupBy(pd => pd.d.Id)
                    .Select(g => new { DemographicId = g.Key, Forfeitures = g.Sum(x => x.pd.Forfeiture) > 0 ? g.Sum(x => x.pd.Forfeiture) : 0 });

                var recs = await (
                    from d in ctx.Demographics
                    join pp in ctx.PayProfits on d.Id equals pp.DemographicId
                    join fLj in forfeitures on d.Id equals fLj.DemographicId into fTmp
                    from f in fTmp.DefaultIfEmpty()
                    where pp.ProfitYear == req.ProfitYear
                    orderby d.EmployeeId
                    select new ForfeituresAndPointsForYearResponse() {
                        EmployeeBadgeNumber = d.EmployeeId,
                        EmployeeName = d.ContactInfo.FullName,
                        EmployeeSsn = d.Ssn.ToString(),
                        Forfeitures = f.Forfeitures,
                        ForfeitPoints = 0,
                        EarningPoints = 0
                    }
                ).ToPaginationResultsAsync(req, cancellationToken);

                var badges = recs.Results.Select(x => (int)x.EmployeeBadgeNumber).ToHashSet();
                var totals = await _contributionService.GetNetBalance((req.ProfitYear), badges, cancellationToken);

                var currentYear = await (from pd in ctx.ProfitDetails
                    join d in ctx.Demographics on pd.Ssn equals d.Ssn
                    where pd.ProfitYear == req.ProfitYear
                          && badges.Contains(d.EmployeeId)
                    group pd by new { pd.Ssn, d.EmployeeId }
                    into pd_g
                    select new
                    {
                        pd_g.Key.EmployeeId,
                        pd_g.Key.Ssn,
                        loan1Total =
                            pd_g.Where(x =>
                                new[] { ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id, ProfitCode.Constants.OutgoingDirectPayments.Id }
                                    .Contains(x.ProfitCodeId) ||
                                (x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment) &&
                                x.CommentTypeId != CommentType.Constants.TransferOut).Sum(x => x.Forfeiture),
                        forfeitTotal = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures).Sum(x => x.Forfeiture),
                        loan2Total = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary).Sum(x => x.Forfeiture),
                        allocationTotal = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary).Sum(x => x.Contribution)
                    }).ToListAsync(cancellationToken);

                var lastYearPayProfits = await (from pp in ctx.PayProfits
                        join d in ctx.Demographics on pp.DemographicId equals d.Id
                        where pp.ProfitYear == req.ProfitYear - 1 && badges.Contains(d.EmployeeId)
                                                                  && (pp.HoursExecutive + pp.CurrentHoursYear) >= 1000
                        select new { d.EmployeeId, pp.CurrentIncomeYear }
                    ).ToListAsync(cancellationToken);

                foreach (var rec in recs.Results.Where(rec => totals.ContainsKey((int)rec.EmployeeBadgeNumber)))
                {
                    var cy = currentYear.Find(x => x.EmployeeId == rec.EmployeeBadgeNumber);
                    if (cy != default)
                    {
                        var points = (totals[(int)rec.EmployeeBadgeNumber].TotalContributions +
                                      totals[(int)rec.EmployeeBadgeNumber].TotalEarnings +
                                      totals[(int)rec.EmployeeBadgeNumber].TotalForfeitures -
                                      totals[(int)rec.EmployeeBadgeNumber].TotalPayments) -
                                     (cy.loan1Total - cy.loan2Total - cy.forfeitTotal);

                        rec.EarningPoints = Convert.ToInt16(Math.Round(points / 100, 0, MidpointRounding.AwayFromZero));
                    }

                    var lypp = lastYearPayProfits.Find(x => x.EmployeeId == rec.EmployeeBadgeNumber);
                    if (lypp != null)
                    {
                        rec.ForfeitPoints = Convert.ToInt16(Math.Round((lypp.CurrentIncomeYear) / 100, 0, MidpointRounding.AwayFromZero));
                    }
                }

                return recs;
            });

            _logger.LogInformation("Returned {Results} records", rslt.Results.Count());

            return new ReportResponseBase<ForfeituresAndPointsForYearResponse>
            {
                ReportDate = DateTimeOffset.Now, ReportName = $"PROFIT  SHARING  FORFEITURES  AND  POINTS  FOR  {req.ProfitYear}", Response = rslt
            };
        }
    }

    public async Task<ProfitSharingDistributionsByAge> GetDistributionsByAgeYear(ProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        List<byte> codes =
        [
            ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal,
            ProfitCode.Constants.OutgoingDirectPayments,
            ProfitCode.Constants.Outgoing100PercentVestedPayment
        ];

        const string FT = "FullTime";
        const string PT = "PartTime";

        var details = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = await (from pd in ctx.ProfitDetails
                    join d in ctx.Demographics on pd.Ssn equals d.Ssn
                    where pd.ProfitYear == req.ProfitYear && codes.Contains(pd.ProfitCodeId)
                    select new
                    {
                        d.DateOfBirth,
                        EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? "PartTime" : "FullTime",
                        d.EmployeeId,
                        Amount = pd.Forfeiture,
                        pd.CommentTypeId
                    })
                .ToListAsync(cancellationToken: cancellationToken);

            return query.Select(x => new
                {
                    Age = x.DateOfBirth.Age(),
                    x.EmploymentType,
                    x.EmployeeId,
                    x.Amount,
                    x.CommentTypeId
                })
                .GroupBy(x => new { x.Age, x.EmploymentType, x.CommentTypeId })
                .Select(g => new ProfitSharingDistributionsByAgeDetail()
                {
                    Age = g.Key.Age,
                    EmploymentType = g.Key.EmploymentType,
                    CommentTypeId = g.Key.CommentTypeId,
                    EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count(),
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Age)
                .ToList();
        });

        // Separate details by employment type
        var fullTimeDetails = details.Where(d => d.EmploymentType == FT).ToList();
        var partTimeDetails = details.Where(d => d.EmploymentType == PT).ToList();

        // Compute aggregates using helper method
        var totalAggregates = ComputeAggregates(details);
        var fullTimeAggregates = ComputeAggregates(fullTimeDetails);
        var partTimeAggregates = ComputeAggregates(partTimeDetails);

        return new ProfitSharingDistributionsByAge
        {
            ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,

            TotalResults = details,
            FullTimeResults = fullTimeDetails,
            PartTimeResults = partTimeDetails,

            RegularTotalEmployees = totalAggregates.RegularTotalEmployees,
            RegularTotalAmount = totalAggregates.RegularAmount,
            HardshipTotalEmployees = totalAggregates.HardshipTotalEmployees,
            HardshipTotalAmount = totalAggregates.HardshipTotalAmount,


            FullTimeRegularEmployees = fullTimeAggregates.RegularTotalEmployees,
            FullTimeRegularAmount = fullTimeAggregates.RegularAmount,
            FullTimeHardshipTotalEmployees = fullTimeAggregates.HardshipTotalEmployees,
            FullTimeHardshipTotalAmount = fullTimeAggregates.HardshipTotalAmount,



            PartTimeRegularEmployees = partTimeAggregates.RegularTotalEmployees,
            PartTimeRegularAmount = partTimeAggregates.RegularAmount,
            PartTimeHardshipTotalEmployees = partTimeAggregates.HardshipTotalEmployees,
            PartTimeHardshipTotalAmount = partTimeAggregates.HardshipTotalAmount,

            // Not a paginated report
            Response = new PaginatedResponseDto<ProfitSharingDistributionsByAgeDetail>(req)
        };
    }

    private ProfitSharingAggregates ComputeAggregates(List<ProfitSharingDistributionsByAgeDetail> details)
    {
        var hardshipDetails = details.Where(d => d.CommentTypeId == CommentType.Constants.Hardship).ToList();
        var regularDetails = details.Where(d => d.CommentTypeId != CommentType.Constants.Hardship).ToList();

        return new ProfitSharingAggregates
        {
            RegularAmount = regularDetails.Sum(d => d.Amount),
            RegularTotalEmployees = (short)regularDetails.Sum(d => d.EmployeeCount),

            HardshipTotalEmployees = (short)hardshipDetails.Sum(d => d.EmployeeCount),
            HardshipTotalAmount = hardshipDetails.Sum(d => d.Amount),
        };
    }
}
