using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class FrozenReportService : IFrozenReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ContributionService _contributionService;
    private readonly TotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly ILogger _logger;

    public FrozenReportService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory loggerFactory,
        ContributionService contributionService,
        TotalService totalService,
        ICalendarService calendarService
    )
    {
        _dataContextFactory = dataContextFactory;
        _contributionService = contributionService;
        _totalService = totalService;
        _calendarService = calendarService;
        _logger = loggerFactory.CreateLogger<FrozenReportService>();
    }

    public async Task<ReportResponseBase<ForfeituresAndPointsForYearResponse>> GetForfeituresAndPointsForYearAsync(ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request FORFEITURES AND POINTS FOR YEAR"))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {

                var forfeitures = ctx.ProfitDetails
                    .Where(pd => pd.ProfitYear == req.ProfitYear)
                    .Where(pd =>pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id)
                    .Join(ctx.Demographics, x => x.Ssn, x => x.Ssn, (pd, d) => new { pd, d })
                    .GroupBy(pd => pd.d.Id)
                    .Select(g => new { DemographicId = g.Key, Forfeitures = g.Sum(x => x.pd.Forfeiture) > 0 ? g.Sum(x => x.pd.Forfeiture) : 0 });

                var recs = 
                    from d in ctx.Demographics.Include(d=> d.ContactInfo)
                    join pp in ctx.PayProfits on d.Id equals pp.DemographicId
                    join fLj in forfeitures on d.Id equals fLj.DemographicId into fTmp
                    from f in fTmp.DefaultIfEmpty()
                    where pp.ProfitYear == req.ProfitYear
                    orderby d.EmployeeId
                    select new ForfeituresAndPointsForYearResponse()
                    {
                        EmployeeId = d.EmployeeId,
                        EmployeeName = d.ContactInfo.FullName,
                        EmployeeSsn = d.Ssn.ToString(),
                        Forfeitures = f != null ? f.Forfeitures : 0,
                        ForfeitPoints = 0,
                        EarningPoints = 0
                    };

                var query = await recs.ToPaginationResultsAsync(req, cancellationToken);

                var badges = query.Results.Select(x => (int)x.EmployeeId).ToHashSet();
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

                foreach (var rec in query.Results.Where(rec => totals.ContainsKey((int)rec.EmployeeId)))
                {
                    var cy = currentYear.Find(x => x.EmployeeId == rec.EmployeeId);
                    if (cy != default)
                    {
                        var points = (totals[(int)rec.EmployeeId].TotalContributions +
                                      totals[(int)rec.EmployeeId].TotalEarnings +
                                      totals[(int)rec.EmployeeId].TotalForfeitures -
                                      totals[(int)rec.EmployeeId].TotalPayments) -
                                     (cy.loan1Total - cy.loan2Total - cy.forfeitTotal);

                        rec.EarningPoints = Convert.ToInt16(Math.Round(points / 100, 0, MidpointRounding.AwayFromZero));
                    }

                    var lypp = lastYearPayProfits.Find(x => x.EmployeeId == rec.EmployeeId);
                    if (lypp != null)
                    {
                        rec.ForfeitPoints = Convert.ToInt16(Math.Round((lypp.CurrentIncomeYear) / 100, 0, MidpointRounding.AwayFromZero));
                    }
                }
                return query;
            });

            _logger.LogInformation("Returned {Results} records", rslt.Results.Count());

            return new ReportResponseBase<ForfeituresAndPointsForYearResponse>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = $"PROFIT  SHARING  FORFEITURES  AND  POINTS  FOR  {req.ProfitYear}",
                Response = rslt
            };
        }
    }

    public async Task<DistributionsByAge> GetDistributionsByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
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


    public async Task<ContributionsByAge> GetContributionsByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
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

    public async Task<ForfeituresByAge> GetForfeituresByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        const string FT = "FullTime";
        const string PT = "PartTime";

        var queryResult = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = (from pd in ctx.ProfitDetails
                         join d in ctx.Demographics on pd.Ssn equals d.Ssn
                         where pd.ProfitYear == req.ProfitYear
                               && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
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

    public async Task<BalanceByAge> GetBalanceByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        const string FT = "FullTime";
        const string PT = "PartTime";

        var startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var rawResult = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = _totalService.TotalVestingBalance(ctx, req.ProfitYear, startEnd.FiscalEndDate);

            var joinedQuery = from q in query
                              join d in ctx.Demographics on q.Ssn equals d.Ssn into demographics
                              from demographic in demographics.DefaultIfEmpty()
                              join b in ctx.BeneficiaryContacts on q.Ssn equals b.Ssn into beneficiaries
                              from beneficiary in beneficiaries.DefaultIfEmpty()
                              where demographic != null || beneficiary != null
                              select new
                              {
                                  q.CurrentBalance,
                                  q.VestedBalance,
                                  EmploymentType = 
                                      demographic != null && demographic.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT,
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  DateOfBirth = demographic != null
                                      ? demographic.DateOfBirth
                                      : (beneficiary!.DateOfBirth),
                              };

            joinedQuery = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => joinedQuery.Where(g => !g.IsBeneficiary && g.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => joinedQuery.Where(g => !g.IsBeneficiary && g.EmploymentType == PT),
                _ => joinedQuery
            };

            return joinedQuery
                .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
                .ToListAsync(cancellationToken);
        });

        // Client-side processing for grouping and filtering
        var groupedResult = rawResult
            .GroupBy(item => item.DateOfBirth.Age())
            .Select(g => new
            {
                Age = g.Key,
                Entries = g.ToList()
            })
            .ToList();

        // Final transformation to BalanceByAgeDetail
        var details = groupedResult
            .Select(group => new BalanceByAgeDetail
            {
                Age = group.Age,
                CurrentBalance = group.Entries.Sum(e => e.CurrentBalance),
                CurrentBeneficiaryBalance = group.Entries.Sum(e => e.IsBeneficiary ? e.CurrentBalance : 0),
                CurrentBeneficiaryVestedBalance = group.Entries.Sum(e => e.IsBeneficiary ? e.VestedBalance : 0),
                VestedBalance = group.Entries.Sum(e => e.VestedBalance),
                BeneficiaryCount = group.Entries.Count(e => e.IsBeneficiary),
                EmployeeCount = group.Entries.Count(e => !e.IsBeneficiary),
                FullTimeCount = group.Entries.Count(e => e.EmploymentType == FT),
                PartTimeCount = group.Entries.Count(e => e.EmploymentType == PT)
            })            
            .OrderBy(e=> e.Age)
            .ToList();

        // Build the final response
        req = req with { Take = details.Count + 1 };

        return new BalanceByAge
        {
            ReportName = "PROFIT SHARING BALANCE BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            BalanceTotalAmount = details.Sum(d => d.CurrentBalance),
            VestedTotalAmount = details.Sum(d => d.VestedBalance),
            TotalMembers = (short)details.Sum(d => d.EmployeeCount + d.BeneficiaryCount),
            TotalBeneficiaries = (short)details.Sum(d => d.BeneficiaryCount),
            TotalBeneficiariesAmount = details.Sum(d => d.CurrentBeneficiaryBalance),
            TotalBeneficiariesVestedAmount = details.Sum(d => d.CurrentBeneficiaryVestedBalance),
            TotalFullTimeCount = details.Sum(d => d.FullTimeCount),
            TotalPartTimeCount = details.Sum(d => d.PartTimeCount),
            Response = new PaginatedResponseDto<BalanceByAgeDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
        };

    }

    public async Task<VestedAmountsByAge> GetVestedAmountsByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        const string FT = "FullTime";
        const string PT = "PartTime";

        var startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var rawResult = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = _totalService.TotalVestingBalance(ctx, req.ProfitYear, startEnd.FiscalEndDate);

            var joinedQuery = from q in query
                              join d in ctx.Demographics on q.Ssn equals d.Ssn into demographics
                              from demographic in demographics.DefaultIfEmpty()
                              join b in ctx.BeneficiaryContacts on q.Ssn equals b.Ssn into beneficiaries
                              from beneficiary in beneficiaries.DefaultIfEmpty()
                              where demographic != null || beneficiary != null
                              select new
                              {
                                  q.CurrentBalance,
                                  q.VestedBalance,
                                  EmploymentType =
                                      demographic != null && demographic.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT,
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  DateOfBirth = demographic != null
                                      ? demographic.DateOfBirth
                                      : (beneficiary!.DateOfBirth),
                              };

            joinedQuery = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => joinedQuery.Where(g => !g.IsBeneficiary && g.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => joinedQuery.Where(g => !g.IsBeneficiary && g.EmploymentType == PT),
                _ => joinedQuery
            };

            return joinedQuery
                .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
                .ToListAsync(cancellationToken);
        });

        // Client-side processing for grouping and filtering
        var groupedResult = rawResult
            .GroupBy(item => item.DateOfBirth.Age())
            .Select(g => new
            {
                Age = g.Key,
                Entries = g.ToList()
            })
            .ToList();

        // Final transformation to VestedAmountsByAgeDetail
        var details = groupedResult
            .Select(group => new VestedAmountsByAgeDetail
            {
                Age = (byte)group.Age,
                FullTimeCount = (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance),
                FullTimeAmount = group.Entries.Where(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance).Sum(e => e.CurrentBalance),
                NotVestedCount = (short)group.Entries.Count(e => e.VestedBalance == 0),
                NotVestedAmount = group.Entries.Where(e => e.VestedBalance == 0).Sum(e => e.CurrentBalance),
                PartialVestedCount = (short)group.Entries.Count(e => e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                PartialVestedAmount = group.Entries.Where(e => e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance).Sum(e => e.VestedBalance),
                BeneficiaryCount = (short)group.Entries.Count(e => e.IsBeneficiary),
                BeneficiaryAmount = group.Entries.Where(e => e.IsBeneficiary).Sum(e => e.CurrentBalance)
            })
            .OrderBy(e => e.Age)
            .ToList();

        // Build the final response
        req = req with { Take = details.Count + 1 };

        return new VestedAmountsByAge
        {
            ReportName = "PROFIT SHARING VESTED AMOUNTS BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            TotalFullTimeAmount = details.Sum(d => d.FullTimeAmount),
            TotalNotVestedAmount = details.Sum(d => d.NotVestedAmount),
            TotalPartialVestedAmount = details.Sum(d => d.PartialVestedAmount),
            TotalBeneficiaryAmount = details.Sum(d => d.BeneficiaryAmount),
            TotalFullTimeCount = (short)details.Sum(d => d.FullTimeCount),
            TotalNotVestedCount = (short)details.Sum(d => d.NotVestedCount),
            TotalPartialVestedCount = (short)details.Sum(d => d.PartialVestedCount),
            TotalBeneficiaryCount = (short)details.Sum(d => d.BeneficiaryCount),
            Response = new PaginatedResponseDto<VestedAmountsByAgeDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
        };
    }
}
