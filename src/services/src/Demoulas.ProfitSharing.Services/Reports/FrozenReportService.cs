using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static FastEndpoints.Ep;

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

    public async Task<ForfeituresAndPointsForYearResponseWithTotals> GetForfeituresAndPointsForYearAsync(
    FrozenProfitYearRequest req,
    CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request FORFEITURES AND POINTS FOR YEAR"))
        {
            var hoursWorkedRequirement = ContributionService.MinimumHoursForContribution();

            var result = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // Create base query with appropriate demographics data (frozen or current)
                var demographicExpression = req.UseFrozenData
                    ? FrozenService.GetDemographicSnapshot(ctx, req.ProfitYear)
                    : ctx.Demographics.Include(d => d.ContactInfo);

                var combinedQuery = from d in demographicExpression
                                    join pp in ctx.PayProfits on d.Id equals pp.DemographicId
                                    join lastYearPp in ctx.PayProfits on new { DemographicId = d.Id, ProfitYear = (short)(req.ProfitYear - 1) }
                                        equals new { DemographicId = lastYearPp.DemographicId, ProfitYear = lastYearPp.ProfitYear } into lastYearPpGroup
                                    from lastYearPp in lastYearPpGroup.DefaultIfEmpty()
                                    where pp.ProfitYear == req.ProfitYear
                                    select new
                                    {
                                        d.BadgeNumber,
                                        d.Ssn,
                                        EmployeeName = d.ContactInfo.FullName ?? "",
                                        LastYearIncome = lastYearPp != null ? lastYearPp.CurrentIncomeYear : 0m,
                                        MetHoursRequirement = lastYearPp != null &&
                                            (lastYearPp.HoursExecutive + lastYearPp.CurrentHoursYear) >= hoursWorkedRequirement
                                    };

                var totalCount = await combinedQuery.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    return new
                    {
                        Pagination = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse>(req)
                        {
                            Results = new List<ForfeituresAndPointsForYearResponse>(),
                            Total = 0
                        },
                        Totals = new { TotalForfeitures = 0m, TotalForfeitPoints = 0, TotalEarningPoints = 0 }
                    };
                }

                var allBadgeNumbers = await combinedQuery.Select(x => x.BadgeNumber).ToListAsync(cancellationToken);

                var netBalances = await _contributionService.GetNetBalance(req.ProfitYear, allBadgeNumbers.ToHashSet(), cancellationToken);

                // Get forfeitures for current year
                var forfeitures = await ctx.ProfitDetails
                    .Join(ctx.Demographics, pd => pd.Ssn, d => d.Ssn, (pd, d) => new { pd, d })
                    .Where(x => x.pd.ProfitYear == req.ProfitYear &&
                                x.pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id)
                    .GroupBy(x => x.d.BadgeNumber)
                    .Select(g => new { g.Key, SumValue = g.Sum(x => x.pd.Forfeiture) })
                    .ToDictionaryAsync(x => x.Key, x => x.SumValue, cancellationToken);

                var currentYearDetails = await (from pd in ctx.ProfitDetails
                                                join d in ctx.Demographics on pd.Ssn equals d.Ssn
                                                where pd.ProfitYear == req.ProfitYear && allBadgeNumbers.Contains(d.BadgeNumber)
                                                group pd by new { BadgeNumber = d.BadgeNumber }
                    into pd_g
                                                select new
                                                {
                                                    pd_g.Key.BadgeNumber,
                                                    loan1Total =
                                                        pd_g.Where(x =>
                                                            new[]
                                                                {
                                    ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                                    ProfitCode.Constants.OutgoingDirectPayments.Id
                                                                }
                                                                .Contains(x.ProfitCodeId) ||
                                                            (x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment) &&
                                                            x.CommentTypeId != CommentType.Constants.TransferOut).Sum(x => x.Forfeiture),
                                                    forfeitTotal =
                                                        pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
                                                            .Sum(x => x.Forfeiture),
                                                    loan2Total =
                                                        pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary)
                                                            .Sum(x => x.Forfeiture)
                                                }).ToDictionaryAsync(x => x.BadgeNumber, x => x, cancellationToken);

                decimal totalForfeitures = forfeitures.Values.Sum();

                var eligibleForForfeitPoints = await combinedQuery
                    .Where(x => x.MetHoursRequirement)
                    .Select(x => new { x.BadgeNumber, x.LastYearIncome })
                    .ToListAsync(cancellationToken);

                int totalForfeitPoints = eligibleForForfeitPoints
                    .Sum(x => Convert.ToInt32(Math.Round(x.LastYearIncome / 100, 0, MidpointRounding.AwayFromZero)));

                int totalEarningPoints = 0;
                foreach (var badgeNumber in allBadgeNumbers)
                {
                    if (netBalances.TryGetValue(badgeNumber, out var netBalance) &&
                        currentYearDetails.TryGetValue(badgeNumber, out var cy))
                    {
                        decimal points = (netBalance.TotalContributions +
                                      netBalance.TotalEarnings +
                                      netBalance.TotalForfeitures -
                                      netBalance.TotalPayments) -
                                     (cy.loan1Total - cy.loan2Total - cy.forfeitTotal);

                        totalEarningPoints += Convert.ToInt32(Math.Round(points / 100, 0, MidpointRounding.AwayFromZero));
                    }
                }

                // Get paginated results - @RUSS/Backend team - should totals reflect only paginated dataset or all?
                var paginatedData = await combinedQuery
                    .OrderBy(x => x.BadgeNumber)
                    .Skip(req.Skip ?? 0)
                    .Take(req.Take ?? int.MaxValue)
                    .ToListAsync(cancellationToken);

                var results = new List<ForfeituresAndPointsForYearResponse>();

                foreach (var item in paginatedData)
                {
                    var result = new ForfeituresAndPointsForYearResponse
                    {
                        BadgeNumber = item.BadgeNumber,
                        EmployeeName = item.EmployeeName,
                        Ssn = item.Ssn.MaskSsn(),
                        Forfeitures = 0,
                        ForfeitPoints = 0,
                        EarningPoints = 0
                    };

                    // Calculate earning points
                    if (netBalances.TryGetValue(item.BadgeNumber, out var netBalance) &&
                        currentYearDetails.TryGetValue(item.BadgeNumber, out var cy))
                    {
                        decimal points = (netBalance.TotalContributions +
                                      netBalance.TotalEarnings +
                                      netBalance.TotalForfeitures -
                                      netBalance.TotalPayments) -
                                     (cy.loan1Total - cy.loan2Total - cy.forfeitTotal);

                        result.EarningPoints = Convert.ToInt16(Math.Round(points / 100, 0, MidpointRounding.AwayFromZero));
                    }

                    // Calculate and set forfeit points from last year
                    if (item.MetHoursRequirement)
                    {
                        result.ForfeitPoints = Convert.ToInt16(
                            Math.Round(item.LastYearIncome / 100, 0, MidpointRounding.AwayFromZero));
                    }

                    // Set forfeitures
                    if (forfeitures.TryGetValue(item.BadgeNumber, out var forfeiture))
                    {
                        result.Forfeitures = forfeiture;
                    }

                    // Only include records with non-zero values
                    if (result.EarningPoints != 0 || result.ForfeitPoints != 0 || result.Forfeitures != 0)
                    {
                        results.Add(result);
                    }
                }

                return new
                {
                    Pagination = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse>(req)
                    {
                        Results = results,
                        Total = totalCount
                    },
                    Totals = new
                    {
                        TotalForfeitures = totalForfeitures,
                        TotalForfeitPoints = totalForfeitPoints,
                        TotalEarningPoints = totalEarningPoints
                    }
                };
            });

            _logger.LogInformation("Returned {Results} records", result.Pagination.Results.Count());

            return new ForfeituresAndPointsForYearResponseWithTotals
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = $"PROFIT SHARING FORFEITURES AND POINTS FOR {req.ProfitYear}",
                Response = result.Pagination,
                TotalForfeitures = result.Totals.TotalForfeitures,
                TotalForfeitPoints = result.Totals.TotalForfeitPoints,
                TotalEarningPoints = result.Totals.TotalEarningPoints
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
    public async Task<DistributionsByAge> GetDistributionsByAgeYearAsync(FrozenReportsByAgeRequest req,
        CancellationToken cancellationToken = default)
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
                BadgeNumbers = g.Select(x => x.BadgeNumber).ToHashSet(),
                HardshipEmployeeCount =
                    g.Where(x => x.CommentTypeId == CommentType.Constants.Hardship).Select(x => x.BadgeNumber)
                        .ToHashSet().Count,
                RegularEmployeeCount =
                    g.Where(x => x.CommentTypeId != CommentType.Constants.Hardship).Select(x => x.BadgeNumber)
                        .ToHashSet().Count,
                Amount = g.Sum(x => x.Amount),
                HardshipAmount = g.Where(x => x.CommentTypeId == CommentType.Constants.Hardship).Sum(x => x.Amount),
                RegularAmount = g.Where(x => x.CommentTypeId != CommentType.Constants.Hardship).Sum(x => x.Amount)
            })
            .OrderBy(x => x.Age)
            .ToList();

        if (req.ReportType != FrozenReportsByAgeRequest.Report.Total)
        {
            var totalRequest = req with { ReportType = FrozenReportsByAgeRequest.Report.Total };
            var totalDetails = await GetDistributionsByAgeYearAsync(totalRequest, cancellationToken);
            var totalAges = totalDetails.Response.Results.Select(d => d.Age).ToHashSet();

            foreach (var age in totalAges.Where(age => details.All(d => d.Age != age)))
            {
                details.Add(new DistributionsByAgeDetail
                {
                    Age = age,
                    EmploymentType = req.ReportType.ToString(),
                    BadgeNumbers = [],
                    HardshipEmployeeCount = 0,
                    RegularEmployeeCount = 0,
                    Amount = 0,
                    HardshipAmount = 0,
                    RegularAmount = 0
                });
            }

            details = details.OrderBy(d => d.Age).ToList();
        }

        req = req with { Take = details.Count + 1 };

        static ProfitSharingAggregates ComputeAggregates(List<DistributionsByAgeDetail> details)
        {
            return new ProfitSharingAggregates
            {
                RegularTotalEmployees = (short)details.Where(d => d.RegularAmount > 0).Sum(d => d.EmployeeCount),
                RegularAmount = details.Sum(d => d.RegularAmount),
                HardshipTotalEmployees = (short)details.Where(d => d.HardshipAmount > 0).Sum(d => d.EmployeeCount),
                HardshipTotalAmount = details.Sum(d => d.HardshipAmount),
                TotalEmployees = (short)details.Sum(d => d.EmployeeCount),
                BothHardshipAndRegularEmployees =
                    (short)details.Where(d => d is { RegularAmount: > 0, HardshipAmount: > 0 })
                        .Sum(d => d.EmployeeCount),
                BothHardshipAndRegularAmount = details.Where(d => d is { RegularAmount: > 0, HardshipAmount: > 0 })
                    .Sum(d => d.RegularAmount + d.HardshipAmount)
            };
        }

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
            TotalEmployees = totalAggregates.TotalEmployees,
            BothHardshipAndRegularEmployees = totalAggregates.BothHardshipAndRegularEmployees,
            BothHardshipAndRegularAmount = totalAggregates.BothHardshipAndRegularAmount,
            Response = new PaginatedResponseDto<DistributionsByAgeDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
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
    public async Task<ContributionsByAge> GetContributionsByAgeYearAsync(FrozenReportsByAgeRequest req,
        CancellationToken cancellationToken = default)
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
                             BadgeNumber = d.BadgeNumber,
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

        var asOfDate = await GetAsOfDate(req, cancellationToken);
        var details = queryResult.Select(x => new { Age = x.DateOfBirth.Age(asOfDate), x.BadgeNumber, x.Amount })
            .GroupBy(x => new { x.Age })
            .Select(g => new ContributionsByAgeDetail
            {
                Age = g.Key.Age,
                EmployeeCount = g.Select(x => x.BadgeNumber).Distinct().Count(),
                Amount = g.Sum(x => x.Amount),
            })
            .OrderBy(x => x.Age)
            .ToList();

        if (req.ReportType != FrozenReportsByAgeRequest.Report.Total)
        {
            var totalRequest = req with { ReportType = FrozenReportsByAgeRequest.Report.Total };
            var totalDetails = await GetContributionsByAgeYearAsync(totalRequest, cancellationToken);
            var totalAges = totalDetails.Response.Results.Select(d => d.Age).ToHashSet();

            foreach (var age in totalAges.Where(age => details.All(d => d.Age != age)))
            {
                details.Add(new ContributionsByAgeDetail { Age = age, Amount = 0, EmployeeCount = 0 });
            }

            details = details.OrderBy(d => d.Age).ToList();
        }

        req = req with { Take = details.Count + 1 };


        return new ContributionsByAge
        {
            ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            TotalAmount = details.Sum(d => d.Amount),
            TotalEmployees = (short)details.Sum(d => d.EmployeeCount),
            Response = new PaginatedResponseDto<ContributionsByAgeDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
        };
    }

    public async Task<ForfeituresByAge> GetForfeituresByAgeYearAsync(FrozenReportsByAgeRequest req,
        CancellationToken cancellationToken = default)
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
                             BadgeNumber = d.BadgeNumber,
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

        var asOfDate = await GetAsOfDate(req, cancellationToken);
        var details = queryResult.Select(x => new { Age = x.DateOfBirth.Age(asOfDate), x.BadgeNumber, x.Amount })
            .GroupBy(x => new { x.Age })
            .Select(g => new ForfeituresByAgeDetail
            {
                Age = g.Key.Age,
                EmployeeCount = g.Select(x => x.BadgeNumber).Distinct().Count(),
                Amount = g.Sum(x => x.Amount),
            })
            .OrderBy(x => x.Age)
            .ToList();

        if (req.ReportType != FrozenReportsByAgeRequest.Report.Total)
        {
            var totalRequest = req with { ReportType = FrozenReportsByAgeRequest.Report.Total };
            var totalDetails = await GetForfeituresByAgeYearAsync(totalRequest, cancellationToken);
            var totalAges = totalDetails.Response.Results.Select(d => d.Age).ToHashSet();

            foreach (var age in totalAges.Where(age => details.All(d => d.Age != age)))
            {
                details.Add(new ForfeituresByAgeDetail { Age = age, Amount = 0, EmployeeCount = 0 });
            }

            details = details.OrderBy(d => d.Age).ToList();
        }

        req = req with { Take = details.Count + 1 };


        return new ForfeituresByAge
        {
            ReportName = "PROFIT SHARING FORFEITURES BY AGE",
            ReportDate = DateTimeOffset.Now,
            ReportType = req.ReportType,
            TotalAmount = details.Sum(d => d.Amount),
            TotalEmployees = (short)details.Sum(d => d.EmployeeCount),
            Response = new PaginatedResponseDto<ForfeituresByAgeDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
        };
    }

    /// <summary>
    /// Retrieves a balance report grouped by age for a specific profit year.
    /// </summary>
    /// <param name="req">
    /// The request containing the profit year and additional filtering parameters.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="BalanceByAge"/> object containing the balance details grouped by age, 
    /// including totals and other relevant metrics.
    /// </returns>
    /// <remarks>
    /// This method processes raw data, groups it by age, and performs client-side transformations
    /// to generate a detailed balance report.
    /// </remarks>
    public async Task<BalanceByAge> GetBalanceByAgeYearAsync(FrozenReportsByAgeRequest req,
        CancellationToken cancellationToken = default)
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
                                  CurrentBalance = (q.CurrentBalance ?? 0),
                                  VestedBalance = (q.VestedBalance ?? 0),
                                  EmploymentType =
                                      demographic != null && demographic.EmploymentTypeId == EmploymentType.Constants.PartTime
                                          ? PT
                                          : FT,
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  DateOfBirth = demographic != null
                                      ? demographic.DateOfBirth
                                      : (beneficiary!.DateOfBirth),
                              };

            joinedQuery = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == PT),
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

        if (req.ReportType != FrozenReportsByAgeRequest.Report.Total)
        {
            var totalRequest = req with { ReportType = FrozenReportsByAgeRequest.Report.Total };
            var totalDetails = await GetBalanceByAgeYearAsync(totalRequest, cancellationToken);
            var totalAges = totalDetails.Response.Results.Select(d => d.Age).ToHashSet();

            foreach (var age in totalAges.Where(age => details.All(d => d.Age != age)))
            {
                details.Add(new BalanceByAgeDetail { Age = age, EmployeeCount = 0, CurrentBalance = 0 });
            }

            details = details.OrderBy(d => d.Age).ToList();
        }

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

    public async Task<VestedAmountsByAge> GetVestedAmountsByAgeYearAsync(ProfitYearAndAsOfDateRequest req,
        CancellationToken cancellationToken = default)
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
                                      demographic != null && demographic.EmploymentTypeId == EmploymentType.Constants.PartTime
                                          ? PT
                                          : FT,
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
                FullTimeCount =
                    (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance),
                NotVestedCount = (short)group.Entries.Count(e => e.VestedBalance == 0),
                PartialVestedCount =
                    (short)group.Entries.Count(e => e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                FullTime100PercentCount =
                    (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance),
                FullTime100PercentAmount =
                    group.Entries.Where(e => e.EmploymentType == FT && e.VestedBalance == e.CurrentBalance)
                        .Sum(e => (e.CurrentBalance ?? 0)),
                FullTimePartialCount =
                    (short)group.Entries.Count(e =>
                        e.EmploymentType == FT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                FullTimePartialAmount =
                    group.Entries
                        .Where(e => e.EmploymentType == FT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance)
                        .Sum(e => (e.VestedBalance ?? 0)),
                FullTimeNotVestedCount =
                    (short)group.Entries.Count(e => e.EmploymentType == FT && e.VestedBalance == 0),
                FullTimeNotVestedAmount =
                    group.Entries.Where(e => e.EmploymentType == FT && e.VestedBalance == 0).Sum(e => (e.CurrentBalance ?? 0)),
                PartTime100PercentCount =
                    (short)group.Entries.Count(e => e.EmploymentType == PT && e.VestedBalance == e.CurrentBalance),
                PartTime100PercentAmount =
                    group.Entries.Where(e => e.EmploymentType == PT && e.VestedBalance == e.CurrentBalance)
                        .Sum(e => (e.CurrentBalance ?? 0)),
                PartTimePartialCount =
                    (short)group.Entries.Count(e =>
                        e.EmploymentType == PT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                PartTimePartialAmount =
                    group.Entries
                        .Where(e => e.EmploymentType == PT && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance)
                        .Sum(e => (e.VestedBalance ?? 0)),
                PartTimeNotVestedCount =
                    (short)group.Entries.Count(e => e.EmploymentType == PT && e.VestedBalance == 0),
                PartTimeNotVestedAmount =
                    group.Entries.Where(e => e.EmploymentType == PT && e.VestedBalance == 0).Sum(e => (e.CurrentBalance ?? 0)),
                BeneficiaryCount = (short)group.Entries.Count(e => e.IsBeneficiary),
                BeneficiaryAmount = group.Entries.Where(e => e.IsBeneficiary).Sum(e => (e.CurrentBalance ?? 0)),
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
            Response = new PaginatedResponseDto<VestedAmountsByAgeDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
        };
    }

    public async Task<BalanceByYears> GetBalanceByYearsAsync(FrozenReportsByAgeRequest req,
        CancellationToken cancellationToken = default)
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
                              join d in ctx.Demographics.Include(d => d.PayProfits) on q.Ssn equals d.Ssn into demographics
                              from demographic in demographics.DefaultIfEmpty()
                              join b in ctx.BeneficiaryContacts on q.Ssn equals b.Ssn into beneficiaries
                              from beneficiary in beneficiaries.DefaultIfEmpty()
                              where (demographic != null || beneficiary != null) && yip.Years > 0
                              select new
                              {
                                  q.CurrentBalance,
                                  q.VestedBalance,
                                  EmploymentType =
                                      demographic != null && demographic.EmploymentTypeId == EmploymentType.Constants.PartTime
                                          ? PT
                                          : FT,
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  YearsInPlan = yip.Years
                              };

            joinedQuery = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == FT),
                FrozenReportsByAgeRequest.Report.PartTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == PT),
                _ => joinedQuery
            };

            return joinedQuery
                .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
                .GroupBy(item => item.YearsInPlan)
                .Select(group => new BalanceByYearsDetail
                {
                    Years = group.Key ?? 0,
                    CurrentBalance = group.Sum(e => (e.CurrentBalance ?? 0)),
                    CurrentBeneficiaryBalance = group.Sum(e => e.IsBeneficiary ? (e.CurrentBalance ?? 0) : 0),
                    CurrentBeneficiaryVestedBalance = group.Sum(e => e.IsBeneficiary ? (e.VestedBalance ?? 0) : 0),
                    VestedBalance = group.Sum(e => (e.VestedBalance ?? 0)),
                    BeneficiaryCount = group.Count(e => e.IsBeneficiary),
                    EmployeeCount = group.Count(e => !e.IsBeneficiary),
                    FullTimeCount = group.Count(e => e.EmploymentType == FT),
                    PartTimeCount = group.Count(e => e.EmploymentType == PT)
                })
                .OrderByDescending(e => e.Years)
                .ToListAsync(cancellationToken);
        });

        if (req.ReportType != FrozenReportsByAgeRequest.Report.Total)
        {
            var totalRequest = req with { ReportType = FrozenReportsByAgeRequest.Report.Total };
            var totalDetails = await GetBalanceByYearsAsync(totalRequest, cancellationToken);
            var totalYears = totalDetails.Response.Results.Select(d => d.Years).ToHashSet();

            foreach (var years in totalYears.Where(age => details.All(d => d.Years != age)))
            {
                details.Add(new BalanceByYearsDetail { EmployeeCount = 0, CurrentBalance = 0, Years = years });
            }

            details = details.OrderByDescending(d => d.Years).ToList();
        }

        // Build the final response
        req = req with { Take = (details.Count + 1) };


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
            Response = new PaginatedResponseDto<BalanceByYearsDetail>(req)
            {
                Results = details,
                Total = details.Count
            }
        };
    }

    public async Task<UpdateSummaryReportResponse> GetUpdateSummaryReport(ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        var lastYear = (short)(req.ProfitYear - 1);
        var startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var lyStartEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(lastYear, cancellationToken);
        var rawResult = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            //Get population of both employees and beneficiaries
            var demoBase = FrozenService.GetDemographicSnapshot(ctx, req.ProfitYear).Select(x =>
                new
                {
                    x.Ssn,
                    x.ContactInfo.FirstName,
                    x.ContactInfo.LastName,
                    x.BadgeNumber,
                    DemographicId = x.Id,
                    IsEmployee = true,
                    x.StoreNumber
                });
            var beneficiaryBase = ctx.BeneficiaryContacts
                .Where(x => !ctx.Demographics.Any(d => d.Ssn == x.Ssn))
                .Select(x => new
                {
                    x.Ssn,
                    x.ContactInfo.FirstName,
                    x.ContactInfo.LastName,
                    BadgeNumber = 0,
                    DemographicId = 0,
                    IsEmployee = false,
                    StoreNumber = (short)0
                });
            var
                members = demoBase.Union(
                    beneficiaryBase); //UnionBy throws an error, so beneficiaries that are also employees are filtered out, and the regular Union can be used since we've filtered out possible duplicates.

            var baseQuery = await (
                from m in members
                join bal in _totalService.TotalVestingBalance(ctx, req.ProfitYear, startEnd.FiscalBeginDate)
                    on m.Ssn equals bal.Ssn
                join lyBalTbl in _totalService.TotalVestingBalance(ctx, lastYear, lyStartEnd.FiscalBeginDate)
                    on m.Ssn equals lyBalTbl.Ssn into lyBalTmp
                from lyBal in lyBalTmp.DefaultIfEmpty()
                join lyPpTbl in ctx.PayProfits.Where(x => x.ProfitYear == lastYear)
                    on m.DemographicId equals lyPpTbl.DemographicId into lyPpTmp
                from lyPp in lyPpTmp.DefaultIfEmpty()
                join ppTbl in ctx.PayProfits.Where(x => x.ProfitYear == req.ProfitYear)
                    on m.DemographicId equals ppTbl.DemographicId into ppTmp
                from pp in ppTmp.DefaultIfEmpty()
                where bal.CurrentBalance != 0 && bal.VestedBalance != 0
                select new
                {
                    m.BadgeNumber,
                    m.FirstName,
                    m.LastName,
                    m.StoreNumber,
                    m.IsEmployee,
                    BeforeEnrollmentId = lyPp != null ? lyPp.EnrollmentId : 0,
                    BeforeProfitSharingAmount = lyBal != null ? lyBal.CurrentBalance : 0,
                    BeforeVestedProfitSharingAmount = lyBal != null ? lyBal.VestedBalance : 0,
                    BeforeYearsInPlan = lyBal != null ? lyBal.YearsInPlan : (byte)0,
                    AfterEnrollmentId = pp != null ? pp.EnrollmentId : 0,
                    AfterProfitSharingAmount = bal.CurrentBalance,
                    AfterVestedProfitSharingAmount = bal.VestedBalance,
                    AfterYearsInPlan = bal.YearsInPlan
                }
            ).ToListAsync(
                cancellationToken); //Have to materialize. Something in this query seems to be unable to render as an expression with the current version of the oracle provider.

            var totals = baseQuery.GroupBy(x => true).Select(x => new
            {
                TotalBeforeProfitSharing = x.Sum(c => c.BeforeProfitSharingAmount),
                TotalBeforeVesting = x.Sum(c => c.BeforeVestedProfitSharingAmount),
                TotalAfterProfitSharing = x.Sum(c => c.AfterProfitSharingAmount),
                TotalAfterVesting = x.Sum(c => c.AfterVestedProfitSharingAmount),
                TotalEmployees = x.Count(c => c.IsEmployee),
                TotalBeneficiaries = x.Count(c => !c.IsEmployee)
            }).First();

            var resp = baseQuery.Select(x => new UpdateSummaryReportDetail()
            {
                BadgeNumber = x.BadgeNumber,
                StoreNumber = x.StoreNumber,
                Name = $"{x.LastName}, {x.FirstName}",
                IsEmployee = x.IsEmployee,
                Before = new UpdateSummaryReportPointInTimeDetail()
                {
                    ProfitSharingAmount = (x.BeforeProfitSharingAmount ?? 0),
                    VestedProfitSharingAmount = (x.BeforeVestedProfitSharingAmount ?? 0),
                    YearsInPlan = (x.BeforeYearsInPlan ?? 0),
                    EnrollmentId = (byte)x.BeforeEnrollmentId
                },
                After = new UpdateSummaryReportPointInTimeDetail()
                {
                    ProfitSharingAmount = (x.AfterProfitSharingAmount ?? 0),
                    VestedProfitSharingAmount = (x.AfterVestedProfitSharingAmount ?? 0),
                    YearsInPlan = (x.AfterYearsInPlan ?? 0),
                    EnrollmentId = (byte)x.AfterEnrollmentId
                }
            }).Skip(req.Skip ?? 0).Take(req.Take ?? int.MaxValue);

            return new UpdateSummaryReportResponse()
            {
                ReportName = $"UPDATE SUMMARY FOR PROFIT SHARING :{req.ProfitYear}",
                ReportDate = DateTimeOffset.Now,
                Response =
                    new PaginatedResponseDto<UpdateSummaryReportDetail>(req)
                    {
                        Results = resp,
                        Total = baseQuery.Count
                    },
                TotalAfterProfitSharingAmount = (totals.TotalAfterProfitSharing ?? 0),
                TotalAfterVestedAmount = (totals.TotalAfterVesting ?? 0),
                TotalBeforeProfitSharingAmount = (totals.TotalBeforeProfitSharing ?? 0),
                TotalBeforeVestedAmount = (totals.TotalBeforeVesting ?? 0),
                TotalNumberOfBeneficiaries = totals.TotalBeneficiaries,
                TotalNumberOfEmployees = totals.TotalEmployees
            };
        });


        return rawResult;

    }

    public async Task<GrossWagesReportResponse> GetGrossWagesReport(GrossWagesReportRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request FORFEITURES AND POINTS FOR YEAR"))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                short lastProfitYear = (short)(req.ProfitYear - 1);
                var demographics = FrozenService.GetDemographicSnapshot(ctx, req.ProfitYear);
                var reportDemographics = await (from d in demographics
                    join lyPP in ctx.PayProfits on new { d.Id, Year = lastProfitYear } equals new
                    {
                        Id = lyPP.DemographicId, Year = lyPP.ProfitYear
                    }
                    join pp in ctx.PayProfits on new { d.Id, Year = req.ProfitYear } equals new
                    {
                        Id = pp.DemographicId, Year = pp.ProfitYear
                    }
                    join psBal in _totalService.GetTotalBalanceSet(ctx, req.ProfitYear) on d.Ssn equals psBal.Ssn
                    join fBal in _totalService.GetForfeitures(ctx, req.ProfitYear) on d.Ssn equals fBal.Ssn into
                        fBal_tmp
                    from fBal_lj in fBal_tmp.DefaultIfEmpty()
                    join lBal in _totalService.GetQuoteLoansUnQuote(ctx, req.ProfitYear) on d.Ssn equals lBal.Ssn into
                        lBal_tmp
                    from lBal_lj in lBal_tmp.DefaultIfEmpty()
                    where pp.CurrentIncomeYear + pp.IncomeExecutive > req.MinGrossAmount
                    orderby d.ContactInfo.FullName
                    select new GrossWagesReportDetail()
                    {
                        BadgeNumber = d.BadgeNumber,
                        EmployeeName = d.ContactInfo.FullName ?? "",
                        DateOfBirth = d.DateOfBirth,
                        Ssn = d.Ssn.MaskSsn(),
                        Forfeitures = fBal_lj != null ? (fBal_lj.Total ?? 0) : 0,
                        Loans = lBal_lj != null ? (lBal_lj.Total ?? 0) : 0,
                        ProfitSharingAmount = (psBal.Total ?? 0m),
                        GrossWages = pp.CurrentIncomeYear + pp.IncomeExecutive,
                        EnrollmentId = pp.EnrollmentId,
                    }).ToListAsync(cancellationToken: cancellationToken);

                return reportDemographics;
            });

            return new GrossWagesReportResponse()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = GrossWagesReportResponse.REPORT_NAME,
                Response =
                    new PaginatedResponseDto<GrossWagesReportDetail>(req) { Results = rslt.Skip(req.Skip ?? 0).Take(req.Take ?? int.MaxValue), Total = rslt.Count },
                TotalForfeitures = rslt.Sum(x => x.Forfeitures),
                TotalGrossWages = rslt.Sum(x => x.GrossWages),
                TotalLoans = rslt.Sum(x => x.Loans),
                TotalProfitSharingAmount = rslt.Sum(x => x.ProfitSharingAmount)
            };
        }

    }

    public async Task<ProfitControlSheetResponse> GetProfitControlSheet(ProfitYearRequest request, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request PROFIT CONTROL SHEET"))
        {
            var calInfo =
                await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var rsp = new ProfitControlSheetResponse();

                rsp.EmployeeContributionProfitSharingAmount = (await (
                    from bal in _totalService.GetTotalBalanceSetEmployeePortion(ctx, request.ProfitYear)
                    group bal by true into balGrp
                    select new { Total = balGrp.Sum(x => x.Total) ?? 0 }
                ).FirstOrDefaultAsync(cancellationToken))?.Total ?? 0;

                rsp.NonEmployeeProfitSharingAmount = (await (
                    from bc in ctx.BeneficiaryContacts
                    join b in ctx.Beneficiaries on bc.Id equals b.BeneficiaryContactId
                    where (!ctx.Demographics.Any(d => d.Ssn == bc.Ssn))
                    group b by true into bGrp
                    select new { Total = bGrp.Sum(x => 0) } // Needs to use profit detail rows to get the correct amount
                ).FirstOrDefaultAsync(cancellationToken))?.Total ?? 0;

                rsp.EmployeeBeneficiaryAmount = (await (
                    from bc in ctx.BeneficiaryContacts
                    join b in ctx.Beneficiaries on bc.Id equals b.BeneficiaryContactId
                    where (ctx.Demographics.Any(d => d.Ssn == bc.Ssn))
                    group b by true into bGrp
                    select new { Total = bGrp.Sum(x => 0) } // Needs to use profit detail rows to get the correct amount
                ).FirstOrDefaultAsync(cancellationToken))?.Total ?? 0;

                return rsp;
            });

            return rslt;
        }

    }

    private async Task<DateTime> GetAsOfDate(ProfitYearAndAsOfDateRequest req, CancellationToken cancellationToken)
    {
        DateTime asOfDate;
        if (req.AsOfDate.HasValue)
        {
            asOfDate = req.AsOfDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
        }
        else
        {
            var calInfo =
                await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
            asOfDate = calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
        }

        return asOfDate;
    }
}
