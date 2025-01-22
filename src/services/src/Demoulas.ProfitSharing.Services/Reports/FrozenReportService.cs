using Demoulas.Common.Contracts.Contracts.Response;
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

    public async Task<ReportResponseBase<ForfeituresAndPointsForYearResponse>> GetForfeituresAndPointsForYearAsync(FrozenProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request FORFEITURES AND POINTS FOR YEAR"))
        {
            var hoursWorkedRequirement = ContributionService.MinimumHoursForContribution();

            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var demographicExpression = ctx.Demographics.Include(d => d.ContactInfo).Select(x=>x);
                if (req.UseFrozenData)
                {
                    demographicExpression = FrozenService.GetDemographicSnapshot(ctx, req.ProfitYear);
                }

                var recs =
                    from d in demographicExpression
                    join pp in ctx.PayProfits on d.Id equals pp.DemographicId
                    where pp.ProfitYear == req.ProfitYear
                    orderby d.BadgeNumber
                    select new ForfeituresAndPointsForYearResponse()
                    {
                        BadgeNumber = d.BadgeNumber,
                        EmployeeName = d.ContactInfo.FullName,
                        EmployeeSsn = d.Ssn.ToString(),
                        Forfeitures = 0,
                        ForfeitPoints = 0,
                        EarningPoints = 0
                    };

                var query = await recs.ToListAsync(cancellationToken);

                var badges = query.Select(x => (int)x.BadgeNumber).ToHashSet();
                var totals = await _contributionService.GetNetBalance((req.ProfitYear), badges, cancellationToken);
                var forfeitures = ctx.ProfitDetails
                    .Join(ctx.Demographics, x => x.Ssn, x => x.Ssn, (pd, d) => new { pd, d })
                    .Where(x => x.pd.ProfitYear == req.ProfitYear)
                    .Where(x => x.pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id)
                    .GroupBy(pd => pd.d.BadgeNumber)
                    .Select(g => new { DemographicId = g.Key, Forfeitures = g.Sum(x => x.pd.Forfeiture) > 0 ? g.Sum(x => x.pd.Forfeiture) : 0 });
                
                var forfeituresFiltered = await forfeitures.Where(f => badges.Contains(f.DemographicId)).ToHashSetAsync(cancellationToken);

                var currentYear = await (from pd in ctx.ProfitDetails
                    join d in ctx.Demographics on pd.Ssn equals d.Ssn
                    where pd.ProfitYear == req.ProfitYear
                          && badges.Contains(d.BadgeNumber)
                    group pd by new { pd.Ssn, BadgeNumber = d.BadgeNumber }
                    into pd_g
                    select new
                    {
                        pd_g.Key.BadgeNumber,
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
                        where pp.ProfitYear == req.ProfitYear - 1 && badges.Contains(d.BadgeNumber)
                                                                  && (pp.HoursExecutive + pp.CurrentHoursYear) >= hoursWorkedRequirement
                        select new { BadgeNumber = d.BadgeNumber, pp.CurrentIncomeYear }
                    ).ToListAsync(cancellationToken);

                foreach (var rec in query.Where(rec => totals.ContainsKey((int)rec.BadgeNumber)))
                {
                    var cy = currentYear.Find(x => x.BadgeNumber == rec.BadgeNumber);
                    if (cy != default)
                    {
                        decimal points = (totals[(int)rec.BadgeNumber].TotalContributions +
                                          totals[(int)rec.BadgeNumber].TotalEarnings +
                                          totals[(int)rec.BadgeNumber].TotalForfeitures -
                                          totals[(int)rec.BadgeNumber].TotalPayments) -
                                         (cy.loan1Total - cy.loan2Total - cy.forfeitTotal);

                        rec.EarningPoints = Convert.ToInt16(Math.Round(points / 100, 0, MidpointRounding.AwayFromZero));
                    }

                    var lypp = lastYearPayProfits.Find(x => x.BadgeNumber == rec.BadgeNumber);
                    if (lypp != null)
                    {
                        rec.ForfeitPoints = Convert.ToInt16(Math.Round((lypp.CurrentIncomeYear) / 100, 0, MidpointRounding.AwayFromZero));
                    }

                    var forfeitRec = forfeituresFiltered.FirstOrDefault(x => x.DemographicId == rec.BadgeNumber);
                    if (forfeitRec != default)
                    {
                        rec.Forfeitures = forfeitRec.Forfeitures;
                    }
                }

                var rowsWithData = query.Where(x => x.EarningPoints != 0 || x.ForfeitPoints != 0 || x.Forfeitures != 0);

                return new PaginatedResponseDto<ForfeituresAndPointsForYearResponse>(req)
                {
                    Results = rowsWithData.Skip(req.Skip ?? 0).Take(req.Take ?? int.MaxValue),
                    Total = rowsWithData.Count()
                };
            });

            _logger.LogInformation("Returned {Results} records", rslt.Results.Count());

            return new ReportResponseBase<ForfeituresAndPointsForYearResponse>
            {
                ReportDate = DateTimeOffset.Now, ReportName = $"PROFIT  SHARING  FORFEITURES  AND  POINTS  FOR  {req.ProfitYear}", Response = rslt
            };
        }
    }

    /// <summary>
    /// Retrieves the profit-sharing distributions grouped by age and year.
    /// </summary>
    /// <param name="req">
    /// The request object containing the parameters for the report, such as the year and as-of date.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the 
    /// <see cref="DistributionsByAge"/> object, which includes the report details and aggregated data.
    /// </returns>
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
        DateTime asOfDate = await GetAsOfDate(req, cancellationToken);

        var queryResult = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = (from pd in ctx.ProfitDetails
                         join d in ctx.Demographics on pd.Ssn equals d.Ssn
                         where pd.ProfitYear == req.ProfitYear && codes.Contains(pd.ProfitCodeId)
                         select new
                         {
                             d.DateOfBirth,
                             EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT,
                             BadgeNumber = d.BadgeNumber,
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
            Age = x.DateOfBirth.Age(asOfDate),
            x.EmploymentType,
            x.BadgeNumber,
            x.Amount,
            x.CommentTypeId
        })
            .GroupBy(x => new { x.Age })
            .Select(g => new DistributionsByAgeDetail
            {
                Age = g.Key.Age,
                EmploymentType = req.ReportType.ToString(),
                EmployeeCount = g.Select(x => x.BadgeNumber).Distinct().Count(),
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

    /// <summary>
    /// Retrieves the contributions grouped by age for a specific profit year, based on the provided request.
    /// </summary>
    /// <param name="req">
    /// The request containing the parameters for generating the contributions report, 
    /// including the profit year and report type (e.g., FullTime, PartTime, or Total).
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="ContributionsByAge"/> object, 
    /// which includes details such as the report name, report date, total employees, distribution total amount, 
    /// and a paginated response of contributions grouped by age.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="req"/> parameter is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there is an issue with retrieving or processing the data.
    /// </exception>
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
                select new { d.DateOfBirth, EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT, BadgeNumber = d.BadgeNumber, Amount = pd.Contribution });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == PT),
                _ => query
            };


            return query.ToListAsync(cancellationToken: cancellationToken);
        });

        var asOfDate = await GetAsOfDate(req, cancellationToken);
        var details = queryResult.Select(x => new { Age = x.DateOfBirth.Age(asOfDate), x.BadgeNumber, x.Amount })
            .GroupBy(x => new { x.Age })
            .Select(g => new ContributionsByAgeDetail { Age = g.Key.Age, EmployeeCount = g.Select(x => x.BadgeNumber).Distinct().Count(), Amount = g.Sum(x => x.Amount), })
            .OrderBy(x => x.Age)
            .ToList();

        req = req with { Take = details.Count + 1 };


        return new ContributionsByAge
        {
            ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            TotalAmount = details.Sum(d => d.Amount),
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
                select new { d.DateOfBirth, EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? PT : FT, BadgeNumber = d.BadgeNumber, Amount = pd.Forfeiture });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == PT),
                _ => query
            };


            return query.ToListAsync(cancellationToken: cancellationToken);
        });

        var asOfDate = await GetAsOfDate(req, cancellationToken);
        var details = queryResult.Select(x => new { Age = x.DateOfBirth.Age(asOfDate), x.BadgeNumber, x.Amount })
            .GroupBy(x => new { x.Age })
            .Select(g => new ForfeituresByAgeDetail { Age = g.Key.Age, EmployeeCount = g.Select(x => x.BadgeNumber).Distinct().Count(), Amount = g.Sum(x => x.Amount), })
            .OrderBy(x => x.Age)
            .ToList();

        req = req with { Take = details.Count + 1 };


        return new ForfeituresByAge
        {
            ReportName = "PROFIT SHARING FORFEITURES BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            TotalAmount = details.Sum(d => d.Amount),
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
        var asOfDate = await GetAsOfDate(req, cancellationToken);
        var groupedResult = rawResult
            .GroupBy(item => item.DateOfBirth.Age(asOfDate))
            .Select(g => new { Age = g.Key, Entries = g.ToList() })
            .ToList();

        // Final transformation to BalanceByAgeDetail
        var details = groupedResult
            .Select(group => new BalanceByAgeDetail
            {
                Age = (byte)group.Age,
                CurrentBalance = group.Entries.Sum(e => e.CurrentBalance),
                CurrentBeneficiaryBalance = group.Entries.Sum(e => e.IsBeneficiary ? e.CurrentBalance : 0),
                CurrentBeneficiaryVestedBalance = group.Entries.Sum(e => e.IsBeneficiary ? e.VestedBalance : 0),
                VestedBalance = group.Entries.Sum(e => e.VestedBalance),
                BeneficiaryCount = group.Entries.Count(e => e.IsBeneficiary),
                EmployeeCount = group.Entries.Count(e => !e.IsBeneficiary),
                FullTimeCount = group.Entries.Count(e => e.EmploymentType == FT),
                PartTimeCount = group.Entries.Count(e => e.EmploymentType == PT)
            })
            .OrderBy(e => e.Age)
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
            Response = new PaginatedResponseDto<BalanceByAgeDetail>(req) { Results = details, Total = details.Count }
        };
    }

    public async Task<VestedAmountsByAge> GetVestedAmountsByAgeYearAsync(ProfitYearAndAsOfDateRequest req, CancellationToken cancellationToken = default)
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

            return joinedQuery
                .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
                .ToListAsync(cancellationToken);
        });

        // Client-side grouping and aggregation
        var asOfDate = await GetAsOfDate(req, cancellationToken);
        var groupedResult = rawResult
            .GroupBy(item => item.DateOfBirth.Age(asOfDate))
            .Select(g => new { Age = g.Key, Entries = g.ToList() })
            .ToList();

        // Transform into detailed report format
        var details = groupedResult
            .Select(group => new VestedAmountsByAgeDetail
            {
                Age = (byte)(group.Age),
                FullTimeCount = (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance),
                NotVestedCount = (short)group.Entries.Count(e => e.VestedBalance == 0),
                PartialVestedCount = (short)group.Entries.Count(e => e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                FullTime100PercentCount = (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance),
                FullTime100PercentAmount = group.Entries.Where(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance).Sum(e => e.CurrentBalance),
                FullTimePartialCount = (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                FullTimePartialAmount = group.Entries.Where(e => e.EmploymentType == FT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance).Sum(e => e.VestedBalance),
                FullTimeNotVestedCount = (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance == 0),
                FullTimeNotVestedAmount = group.Entries.Where(e => e.EmploymentType == FT && e.VestedBalance == 0).Sum(e => e.CurrentBalance),
                PartTime100PercentCount = (short)group.Entries.Count(e => e.EmploymentType == PT && e.VestedBalance == e.CurrentBalance),
                PartTime100PercentAmount = group.Entries.Where(e => e.EmploymentType == PT && e.VestedBalance == e.CurrentBalance).Sum(e => e.CurrentBalance),
                PartTimePartialCount = (short)group.Entries.Count(e => e.EmploymentType == PT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                PartTimePartialAmount = group.Entries.Where(e => e.EmploymentType == PT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance).Sum(e => e.VestedBalance),
                PartTimeNotVestedCount = (short)group.Entries.Count(e => e.EmploymentType == PT && e.VestedBalance == 0),
                PartTimeNotVestedAmount = group.Entries.Where(e => e.EmploymentType == PT && e.VestedBalance == 0).Sum(e => e.CurrentBalance),
                BeneficiaryCount = (short)group.Entries.Count(e => e.IsBeneficiary),
                BeneficiaryAmount = group.Entries.Where(e => e.IsBeneficiary).Sum(e => e.CurrentBalance),
            })
            .OrderBy(e => e.Age)
            .ToList();

        // Calculate totals for all categories
        short totalFullTimeCount = (short)details.Sum(d => d.FullTimeCount);
        short totalNotVestedCount = (short)details.Sum(d => d.NotVestedCount);
        short totalPartialVestedCount = (short)details.Sum(d => d.PartialVestedCount);
        short totalBeneficiaryCount = (short)details.Sum(d => d.BeneficiaryCount);

        decimal totalFullTime100PercentAmount = details.Sum(d => d.FullTime100PercentAmount);
        decimal totalFullTimePartialAmount = details.Sum(d => d.FullTimePartialAmount);
        decimal totalFullTimeNotVestedAmount = details.Sum(d => d.FullTimeNotVestedAmount);

        decimal totalPartTime100PercentAmount = details.Sum(d => d.PartTime100PercentAmount);
        decimal totalPartTimePartialAmount = details.Sum(d => d.PartTimePartialAmount);
        decimal totalPartTimeNotVestedAmount = details.Sum(d => d.PartTimeNotVestedAmount);

        decimal totalBeneficiaryAmount = details.Sum(d => d.BeneficiaryAmount);

        // Build the final response
        req = req with { Take = details.Count + 1 };

        return new VestedAmountsByAge
        {
            ReportName = "PROFIT SHARING VESTED AMOUNTS BY AGE",
            ReportDate = DateTimeOffset.Now,
            TotalFullTimeCount = totalFullTimeCount,
            TotalNotVestedCount = totalNotVestedCount,
            TotalPartialVestedCount = totalPartialVestedCount,
            TotalFullTime100PercentAmount = totalFullTime100PercentAmount,
            TotalFullTimePartialAmount = totalFullTimePartialAmount,
            TotalFullTimeNotVestedAmount = totalFullTimeNotVestedAmount,
            TotalPartTime100PercentAmount = totalPartTime100PercentAmount,
            TotalPartTimePartialAmount = totalPartTimePartialAmount,
            TotalPartTimeNotVestedAmount = totalPartTimeNotVestedAmount,
            TotalBeneficiaryCount = totalBeneficiaryCount,
            TotalBeneficiaryAmount = totalBeneficiaryAmount,
            Response = new PaginatedResponseDto<VestedAmountsByAgeDetail>(req) { Results = details, Total = details.Count }
        };
    }

    public async Task<BalanceByYears> GetBalanceByYearsAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default)
    {
        const string FT = "FullTime";
        const string PT = "PartTime";

        var startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var details = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = _totalService.TotalVestingBalance(ctx, req.ProfitYear, startEnd.FiscalEndDate);
            var yearsInPlanQuery = _totalService.GetYearsOfService(ctx, req.ProfitYear);

            var joinedQuery = from q in query
                join yip in yearsInPlanQuery on q.Ssn equals yip.Ssn
                join d in ctx.Demographics.Include(d => d.PayProfits.Where(p => p.YearsInPlan > 0)) on q.Ssn equals d.Ssn into demographics
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
                    YearsInPlan = (byte)yip.Years
                };

            joinedQuery = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => joinedQuery.Where(g => !g.IsBeneficiary && g.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => joinedQuery.Where(g => !g.IsBeneficiary && g.EmploymentType == PT),
                _ => joinedQuery
            };

            return joinedQuery
                .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
                .GroupBy(item => item.YearsInPlan)
                .Select(group => new BalanceByYearsDetail
                {
                    Years = group.Key,
                    CurrentBalance = group.Sum(e => e.CurrentBalance),
                    CurrentBeneficiaryBalance = group.Sum(e => e.IsBeneficiary ? e.CurrentBalance : 0),
                    CurrentBeneficiaryVestedBalance = group.Sum(e => e.IsBeneficiary ? e.VestedBalance : 0),
                    VestedBalance = group.Sum(e => e.VestedBalance),
                    BeneficiaryCount = group.Count(e => e.IsBeneficiary),
                    EmployeeCount = group.Count(e => !e.IsBeneficiary),
                    FullTimeCount = group.Count(e => e.EmploymentType == FT),
                    PartTimeCount = group.Count(e => e.EmploymentType == PT)
                })
                .OrderByDescending(e => e.Years)
                .ToListAsync(cancellationToken);
        });

        // Build the final response
#pragma warning disable S2971
        req = req with { Take = (details.Count() + 1) };
#pragma warning restore S2971

        return new BalanceByYears
        {
            ReportName = "PROFIT SHARING BALANCE BY YEARS",
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
            Response = new PaginatedResponseDto<BalanceByYearsDetail>(req) { Results = details, Total = details.Count }
        };
    }

    private async Task<DateTime> GetAsOfDate(ProfitYearAndAsOfDateRequest req, CancellationToken cancellationToken)
    {
        DateTime asOfDate;
        if (req.AsOfDate.HasValue)
        {
            asOfDate = req.AsOfDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }
        else
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
            asOfDate = calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }

        return asOfDate;
    }
}
