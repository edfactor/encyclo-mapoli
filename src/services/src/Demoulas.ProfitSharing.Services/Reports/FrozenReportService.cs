using System.Diagnostics;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class FrozenReportService : IFrozenReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ContributionService _contributionService;
    private readonly TotalService _totalService;
    private readonly ILogger _logger;

    public FrozenReportService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory loggerFactory,
        ContributionService contributionService,
        TotalService totalService
    )
    {
        _dataContextFactory = dataContextFactory;
        _contributionService = contributionService;
        _totalService = totalService;
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
                    select new ForfeituresAndPointsForYearResponse()
                    {
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

    public async Task<DistributionsByAge> GetDistributionsByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        List<byte> codes =
        [
            ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal,
            ProfitCode.Constants.OutgoingDirectPayments,
            ProfitCode.Constants.Outgoing100PercentVestedPayment
        ];

        const string FT = "FullTime";
        const string PT = "PartTime";

        var queryResult = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = (from pd in ctx.ProfitDetails
                join d in ctx.Demographics on pd.Ssn equals d.Ssn
                where pd.ProfitYear == req.ProfitYear && codes.Contains(pd.ProfitCodeId)
                select new
                {
                    d.DateOfBirth,
                    EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT,
                    d.EmployeeId,
                    Amount = pd.Forfeiture,
                    pd.CommentTypeId
                });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == PT),
                _ => query
            };

            return query.ToListAsync(cancellationToken: cancellationToken);
        });


        static ProfitSharingAggregates ComputeAggregates(List<DistributionsByAgeDetail> details)
        {
            return new ProfitSharingAggregates
            {
                RegularTotalEmployees = (short)details.Where(d => d.RegularAmount > 0).Sum(d => d.EmployeeCount),
                RegularAmount = details.Sum(d => d.RegularAmount),
                HardshipTotalEmployees = (short)details.Where(d => d.HardshipAmount > 0).Sum(d => d.EmployeeCount),
                HardshipTotalAmount = details.Sum(d => d.HardshipAmount),
            };
        }

        var details = queryResult.Select(x => new
            {
                Age = x.DateOfBirth.Age(),
                x.EmploymentType,
                x.EmployeeId,
                x.Amount,
                x.CommentTypeId
            })
            .GroupBy(x => new { x.Age, x.EmploymentType })
            .Select(g => new DistributionsByAgeDetail
            {
                Age = g.Key.Age,
                EmploymentType = g.Key.EmploymentType,
                EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count(),
                Amount = g.Sum(x => x.Amount),
                // Compute the total hardship amount within the group
                HardshipAmount = g
                    .Where(x => x.CommentTypeId == CommentType.Constants.Hardship)
                    .Sum(x => x.Amount),
                // Compute the total regular amount within the group
                RegularAmount = g
                    .Where(x => x.CommentTypeId != CommentType.Constants.Hardship)
                    .Sum(x => x.Amount)
            })
            .OrderBy(x => x.Age)
            .ToList();

        req = req with { Take = details.Count + 1 };
        // Compute aggregates using helper method
        var totalAggregates = ComputeAggregates(details);

        return new DistributionsByAge
        {
            ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            RegularTotalEmployees = totalAggregates.RegularTotalEmployees,
            RegularTotalAmount = totalAggregates.RegularAmount,
            HardshipTotalEmployees = totalAggregates.HardshipTotalEmployees,
            HardshipTotalAmount = totalAggregates.HardshipTotalAmount,
            Response = new PaginatedResponseDto<DistributionsByAgeDetail>(req) { Results = details, Total = details.Count }
        };
    }


    public async Task<ContributionsByAge> GetContributionsByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        const string FT = "FullTime";
        const string PT = "PartTime";

        var queryResult = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = (from pd in ctx.ProfitDetails
                join d in ctx.Demographics on pd.Ssn equals d.Ssn
                where pd.ProfitYear == req.ProfitYear
                      && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions
                      && pd.Contribution > 0
                select new
                {
                    d.DateOfBirth,
                    EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT,
                    d.EmployeeId,
                    Amount = pd.Contribution
                });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == PT),
                _ => query
            };


            return query.ToListAsync(cancellationToken: cancellationToken);
        });


        var details = queryResult.Select(x => new
            {
                Age = x.DateOfBirth.Age(),
                x.EmployeeId,
                x.Amount
            })
            .GroupBy(x => new { x.Age })
            .Select(g => new ContributionsByAgeDetail
            {
                Age = g.Key.Age, 
                EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count(),
                Amount = g.Sum(x => x.Amount),
            })
            .OrderBy(x => x.Age)
            .ToList();

        req = req with { Take = details.Count + 1 };


        return new ContributionsByAge
        {
            ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            DistributionTotalAmount = details.Sum(d => d.Amount),
            TotalEmployees = (short)details.Sum(d => d.EmployeeCount),
            Response = new PaginatedResponseDto<ContributionsByAgeDetail>(req) { Results = details, Total = details.Count }
        };
    }

    public async Task<ForfeituresByAge> GetForfeituresByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        const string FT = "FullTime";
        const string PT = "PartTime";

        var queryResult = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = (from pd in ctx.ProfitDetails
                         join d in ctx.Demographics on pd.Ssn equals d.Ssn
                         where pd.ProfitYear == req.ProfitYear
                               && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions
                               && pd.Forfeiture > 0
                         select new
                         {
                             d.DateOfBirth,
                             EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT,
                             d.EmployeeId,
                             Amount = pd.Forfeiture
                         });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == PT),
                _ => query
            };


            return query.ToListAsync(cancellationToken: cancellationToken);
        });


        var details = queryResult.Select(x => new
        {
            Age = x.DateOfBirth.Age(),
            x.EmployeeId,
            x.Amount
        })
            .GroupBy(x => new { x.Age })
            .Select(g => new ForfeituresByAgeDetail
            {
                Age = g.Key.Age,
                EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count(),
                Amount = g.Sum(x => x.Amount),
            })
            .OrderBy(x => x.Age)
            .ToList();

        req = req with { Take = details.Count + 1 };


        return new ForfeituresByAge
        {
            ReportName = "PROFIT SHARING FORFEITURES BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            DistributionTotalAmount = details.Sum(d => d.Amount),
            TotalEmployees = (short)details.Sum(d => d.EmployeeCount),
            Response = new PaginatedResponseDto<ForfeituresByAgeDetail>(req) { Results = details, Total = details.Count }
        };
    }

    public async Task<BalanceByAge> GetBalanceByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        var rawResult = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = _totalService.GetTotalBalanceSet(ctx, req.ProfitYear);

            var joinedQuery = from q in query
                              join d in ctx.Demographics on q.Ssn equals d.Ssn into demographics
                              from demographic in demographics.DefaultIfEmpty()
                              join b in ctx.BeneficiaryContacts on q.Ssn equals b.Ssn into beneficiaries
                              from beneficiary in beneficiaries.DefaultIfEmpty()
                              where demographic != null || beneficiary != null
                              select new
                              {
                                  q,
                                  Demographic = demographic,
                                  BeneficiaryContact = beneficiary,
                                  EmploymentType = demographic != null && demographic.EmploymentTypeId == EmploymentType.Constants.PartTime
                                      ? "PartTime"
                                      : "FullTime",
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  DateOfBirth = demographic != null
                                      ? demographic.DateOfBirth
                                      : (beneficiary!.DateOfBirth)
                              };

            return await joinedQuery.ToListAsync(cancellationToken);
        });

        // Client-side processing for grouping and filtering
        var groupedResult = rawResult
            .GroupBy(item => item.DateOfBirth)
            .Select(g => new
            {
                DateOfBirth = g.Key,
                Entries = g.ToList()
            })
            .Where(g => req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => g.Entries.All(e => e.EmploymentType == "FullTime"),
                FrozenReportsByAgeRequest.Report.PartTime => g.Entries.All(e => e.EmploymentType == "PartTime"),
                _ => true
            })
            .ToList();

        // Final transformation to BalanceByAgeDetail
        var details = groupedResult
            .Select(group => new BalanceByAgeDetail
            {
                Age = group.DateOfBirth.Age(),
                Amount = group.Entries.Sum(e => e.q.Total),
                BeneficiaryCount = group.Entries.Count(e => e.IsBeneficiary),
                EmployeeCount = group.Entries.Count(e => !e.IsBeneficiary)
            })
            .Where(detail => detail.Amount > 0)
            .ToList();

        // Build the final response
        req = req with { Take = details.Count + 1 };

        return new BalanceByAge
        {
            ReportName = "PROFIT SHARING BALANCE BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            BalanceTotalAmount = details.Sum(d => d.Amount),
            TotalMembers = (short)details.Sum(d => d.EmployeeCount + d.BeneficiaryCount),
            TotalBeneficiaries = (short)details.Sum(d => d.BeneficiaryCount),
            TotalNonBeneficiaries = (short)details.Sum(d => d.EmployeeCount),
            Response = new PaginatedResponseDto<BalanceByAgeDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
        };

    }
}
