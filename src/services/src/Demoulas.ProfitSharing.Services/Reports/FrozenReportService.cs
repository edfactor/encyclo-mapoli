using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class FrozenReportService : IFrozenReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger _logger;

    public FrozenReportService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory loggerFactory,
        TotalService totalService,
        ICalendarService calendarService,
        IDemographicReaderService demographicReaderService
    )
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
        _demographicReaderService = demographicReaderService;
        _logger = loggerFactory.CreateLogger<FrozenReportService>();
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

        const string ft = "FullTime";
        const string pt = "PartTime";
        DateTime asOfDate = await GetAsOfDate(req, cancellationToken);

        var queryResult = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var query = (from pd in ctx.ProfitDetails
                         join d in demographics on pd.Ssn equals d.Ssn
                         where pd.ProfitYear == req.ProfitYear && codes.Contains(pd.ProfitCodeId)
                         select new
                         {
                             d.DateOfBirth,
                             EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? pt : ft,
                             BadgeNumber = d.BadgeNumber,
                             Amount = pd.Forfeiture,
                             pd.CommentTypeId
                         });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == ft),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == pt),
                _ => query
            };

            return await query.ToListAsync(cancellationToken: cancellationToken);
        }, cancellationToken);

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
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        return new DistributionsByAge
        {
            ReportName = "PROFIT SHARING DISTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
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
        const string ft = "FullTime";
        const string pt = "PartTime";

        var queryResult = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var query = (from pd in ctx.ProfitDetails
                         join d in demographics on pd.Ssn equals d.Ssn
                         where pd.ProfitYear == req.ProfitYear
                               && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions
                               && pd.Contribution > 0
                         select new
                         {
                             d.DateOfBirth,
                             EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? pt : ft,
                             BadgeNumber = d.BadgeNumber,
                             Amount = pd.Contribution
                         });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == ft),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == pt),
                _ => query
            };


            return await query.ToListAsync(cancellationToken: cancellationToken);
        }, cancellationToken);

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

        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        return new ContributionsByAge
        {
            ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
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
        const string ft = "FullTime";
        const string pt = "PartTime";

        var queryResult = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var query = (from pd in ctx.ProfitDetails
                         join d in demographics on pd.Ssn equals d.Ssn
                         where pd.ProfitYear == req.ProfitYear
                               && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                               && pd.Forfeiture > 0
                         select new
                         {
                             d.DateOfBirth,
                             EmploymentType = d.EmploymentTypeId == EmploymentType.Constants.PartTime ? pt : ft,
                             BadgeNumber = d.BadgeNumber,
                             Amount = pd.Forfeiture
                         });

            query = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => query.Where(q => q.EmploymentType == ft),
                FrozenReportsByAgeRequest.Report.PartTime => query.Where(q => q.EmploymentType == pt),
                _ => query
            };


            return await query.ToListAsync(cancellationToken: cancellationToken);
        }, cancellationToken);

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

        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        return new ForfeituresByAge
        {
            ReportName = "PROFIT SHARING FORFEITURES BY AGE",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
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
        const string ft = "FullTime";
        const string pt = "PartTime";

        var startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var rawResult = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = _totalService.TotalVestingBalance(ctx, req.ProfitYear, startEnd.FiscalEndDate);
            var demo = await _demographicReaderService.BuildDemographicQuery(ctx);

            var joinedQuery = from q in query
                              join d in demo on q.Ssn equals d.Ssn into demographics
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
                                          ? pt
                                          : ft,
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  DateOfBirth = demographic != null
                                      ? demographic.DateOfBirth
                                      : (beneficiary!.DateOfBirth),
                              };

            joinedQuery = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == ft),
                FrozenReportsByAgeRequest.Report.PartTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == pt),
                _ => joinedQuery
            };

            return await joinedQuery.ToListAsync(cancellationToken);
        }, cancellationToken);

        // Client-side processing for grouping and filtering
        var asOfDate = await GetAsOfDate(req, cancellationToken);
        var groupedResult = rawResult
            .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
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
                FullTimeCount = group.Entries.Count(e => e.EmploymentType == ft),
                PartTimeCount = group.Entries.Count(e => e.EmploymentType == pt)
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
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = startEnd.FiscalBeginDate,
            EndDate = startEnd.FiscalEndDate,
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
        const string ft = "FullTime";
        const string pt = "PartTime";

        var startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var rawResult = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = _totalService.TotalVestingBalance(ctx, req.ProfitYear, startEnd.FiscalEndDate);
            var demo = await _demographicReaderService.BuildDemographicQuery(ctx);

            var joinedQuery = from q in query
                              join d in demo on q.Ssn equals d.Ssn into demographics
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
                                          ? pt
                                          : ft,
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  DateOfBirth = demographic != null
                                      ? demographic.DateOfBirth
                                      : (beneficiary!.DateOfBirth),
                              };

            return await joinedQuery
                .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
                .ToListAsync(cancellationToken);
        }, cancellationToken);

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
                    (short)group.Entries.Count(e => e.EmploymentType == ft && e.VestedBalance == e.CurrentBalance),
                NotVestedCount = (short)group.Entries.Count(e => e.VestedBalance == 0),
                PartialVestedCount =
                    (short)group.Entries.Count(e => e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                FullTime100PercentCount =
                    (short)group.Entries.Count(e => e.EmploymentType == ft && e.VestedBalance == e.CurrentBalance),
                FullTime100PercentAmount =
                    group.Entries.Where(e => e.EmploymentType == ft && e.VestedBalance == e.CurrentBalance)
                        .Sum(e => (e.CurrentBalance ?? 0)),
                FullTimePartialCount =
                    (short)group.Entries.Count(e =>
                        e.EmploymentType == ft && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                FullTimePartialAmount =
                    group.Entries
                        .Where(e => e.EmploymentType == ft && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance)
                        .Sum(e => (e.VestedBalance ?? 0)),
                FullTimeNotVestedCount =
                    (short)group.Entries.Count(e => e.EmploymentType == ft && e.VestedBalance == 0),
                FullTimeNotVestedAmount =
                    group.Entries.Where(e => e.EmploymentType == ft && e.VestedBalance == 0).Sum(e => (e.CurrentBalance ?? 0)),
                PartTime100PercentCount =
                    (short)group.Entries.Count(e => e.EmploymentType == pt && e.VestedBalance == e.CurrentBalance),
                PartTime100PercentAmount =
                    group.Entries.Where(e => e.EmploymentType == pt && e.VestedBalance == e.CurrentBalance)
                        .Sum(e => (e.CurrentBalance ?? 0)),
                PartTimePartialCount =
                    (short)group.Entries.Count(e =>
                        e.EmploymentType == pt && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance),
                PartTimePartialAmount =
                    group.Entries
                        .Where(e => e.EmploymentType == pt && e.VestedBalance > 0 && e.VestedBalance < e.CurrentBalance)
                        .Sum(e => (e.VestedBalance ?? 0)),
                PartTimeNotVestedCount =
                    (short)group.Entries.Count(e => e.EmploymentType == pt && e.VestedBalance == 0),
                PartTimeNotVestedAmount =
                    group.Entries.Where(e => e.EmploymentType == pt && e.VestedBalance == 0).Sum(e => (e.CurrentBalance ?? 0)),
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
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = startEnd.FiscalBeginDate,
            EndDate = startEnd.FiscalEndDate,
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
        const string ft = "FullTime";
        const string pt = "PartTime";

        var startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var detailList = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = _totalService.TotalVestingBalance(ctx, req.ProfitYear, startEnd.FiscalEndDate);
            var yearsInPlanQuery = _totalService.GetYearsOfService(ctx, req.ProfitYear, startEnd.FiscalEndDate);
            var demo = await _demographicReaderService.BuildDemographicQuery(ctx);

            var joinedQuery = from q in query
                              join yip in yearsInPlanQuery on q.Ssn equals yip.Ssn
                              join d in demo
                                  .Include(d => d.PayProfits) on q.Ssn equals d.Ssn into demographics
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
                                          ? pt
                                          : ft,
                                  IsBeneficiary = demographic == null && beneficiary != null,
                                  YearsInPlan = yip.Years
                              };

            joinedQuery = req.ReportType switch
            {
                FrozenReportsByAgeRequest.Report.FullTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == ft),
                FrozenReportsByAgeRequest.Report.PartTime => joinedQuery.Where(g =>
                    !g.IsBeneficiary && g.EmploymentType == pt),
                _ => joinedQuery
            };

            return await joinedQuery.ToListAsync(cancellationToken);

        }, cancellationToken);

        var details = detailList
            .Where(detail => (detail.CurrentBalance > 0 || detail.VestedBalance > 0))
            .GroupBy(item => item.YearsInPlan)
            .Select(group => new BalanceByYearsDetail
            {
                Years = group.Key,
                CurrentBalance = group.Sum(e => (e.CurrentBalance ?? 0)),
                CurrentBeneficiaryBalance = group.Sum(e => e.IsBeneficiary ? (e.CurrentBalance ?? 0) : 0),
                CurrentBeneficiaryVestedBalance = group.Sum(e => e.IsBeneficiary ? (e.VestedBalance ?? 0) : 0),
                VestedBalance = group.Sum(e => (e.VestedBalance ?? 0)),
                BeneficiaryCount = group.Count(e => e.IsBeneficiary),
                EmployeeCount = group.Count(e => !e.IsBeneficiary),
                FullTimeCount = group.Count(e => e.EmploymentType == ft),
                PartTimeCount = group.Count(e => e.EmploymentType == pt)
            })
            .OrderByDescending(e => e.Years)
            .ToList();

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
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = startEnd.FiscalBeginDate,
            EndDate = startEnd.FiscalEndDate,
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
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx, true);
            var demoBase = demographics.Select(x =>
                new
                {
                    x.Ssn,
                    x.ContactInfo.FirstName,
                    x.ContactInfo.LastName,
                    BadgeNumber = (long)x.BadgeNumber,
                    DemographicId = x.Id,
                    IsEmployee = true,
                    x.StoreNumber,
                    x.PayFrequencyId,
                });
            var beneficiaryBase = ctx.BeneficiaryContacts
#pragma warning disable DSMPS001
                .Where(x => !ctx.Demographics.Any(d => d.Ssn == x.Ssn))
#pragma warning restore DSMPS001
                .Join(ctx.Beneficiaries, bc => bc.Id, b => b.BeneficiaryContactId, (bc, b) => new { bc, b })
                .Select(x => new
                {
                    x.bc.Ssn,
                    FirstName = x.bc.ContactInfo.FirstName.Trim(),
                    x.bc.ContactInfo.LastName,
                    BadgeNumber = (long)(x.b.BadgeNumber * 10000 + x.b.PsnSuffix),
                    DemographicId = 0,
                    IsEmployee = false,
                    StoreNumber = (short)0,
                    PayFrequencyId = (byte)0,
                });
            var
                members = demoBase.Union(
                    beneficiaryBase); //UnionBy throws an error, so beneficiaries that are also employees are filtered out, and the regular Union can be used since we've filtered out possible duplicates.

            var baseQuery = await (
                from m in members

                join bal in _totalService.TotalVestingBalance(ctx, req.ProfitYear /*Employee*/, startEnd.FiscalEndDate)
                    on m.Ssn equals bal.Ssn into balTmp
                from bal in balTmp.DefaultIfEmpty()

                    // ToBeDone: This should be using the frozen for year "lastYear"
                join lyBalTbl in _totalService.TotalVestingBalance(ctx, lastYear /*Transactions Up To*/, lyStartEnd.FiscalBeginDate)
                    on m.Ssn equals lyBalTbl.Ssn into lyBalTmp
                from lyBal in lyBalTmp.DefaultIfEmpty()

                join lyPpTbl in ctx.PayProfits.Where(x => x.ProfitYear == lastYear)
                    on m.DemographicId equals lyPpTbl.DemographicId into lyPpTmp
                from lyPp in lyPpTmp.DefaultIfEmpty()

                join ppTbl in ctx.PayProfits.Where(x => x.ProfitYear == req.ProfitYear)
                    on m.DemographicId equals ppTbl.DemographicId into ppTmp
                from pp in ppTmp.DefaultIfEmpty()

                select new
                {
                    m.BadgeNumber,
                    m.FirstName,
                    m.LastName,
                    m.StoreNumber,
                    m.IsEmployee,
                    IsExecutive = m.PayFrequencyId == PayFrequency.Constants.Monthly,

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
                IsExecutive = x.IsExecutive,
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
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = lyStartEnd.FiscalBeginDate,
                EndDate = startEnd.FiscalEndDate,
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
        }, cancellationToken);


        return rawResult;

    }

    public async Task<GrossWagesReportResponse> GetGrossWagesReport(GrossWagesReportRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("REQUEST GROSS WAGES REPORT FOR YEAR: {ProfitYear}", req.ProfitYear))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx, true);

                // Query for PayProfit data for the requested year
                var baseQuery = (from d in demographics
                                 join pp in ctx.PayProfits on new { d.Id, Year = req.ProfitYear } equals new
                                 {
                                     Id = pp.DemographicId,
                                     Year = pp.ProfitYear
                                 } into pp_tmp
                                 from pp in pp_tmp.DefaultIfEmpty()
                                 join psBal in _totalService.GetTotalBalanceSet(ctx, req.ProfitYear) on d.Ssn equals psBal.Ssn into
                                     psBal_tmp
                                 from psBal in psBal_tmp.DefaultIfEmpty()
                                 join fBal in _totalService.GetForfeitures(ctx, req.ProfitYear) on d.Ssn equals fBal.Ssn into
                                     fBal_tmp
                                 from fBal_lj in fBal_tmp.DefaultIfEmpty()
                                 join lBal in _totalService.GetQuoteLoansUnQuote(ctx, req.ProfitYear) on d.Ssn equals lBal.Ssn into
                                     lBal_tmp
                                 from lBal_lj in lBal_tmp.DefaultIfEmpty()
                                 where pp != null && (pp.TotalIncome) >= req.MinGrossAmount
                                 orderby d.ContactInfo.FullName
                                 select new
                                 {
                                     d.BadgeNumber,
                                     EmployeeName = d.ContactInfo.FullName ?? "",
                                     d.DateOfBirth,
                                     d.Ssn,
                                     Forfeitures = fBal_lj != null ? fBal_lj.TotalAmount : (decimal?)null,
                                     Loans = lBal_lj != null ? lBal_lj.TotalAmount : (decimal?)null,
                                     ProfitSharingAmount = psBal != null ? psBal.TotalAmount : (decimal?)null,
                                     GrossWages = pp != null ? pp.TotalIncome : 0m,
                                     EnrollmentId = pp != null ? pp.EnrollmentId : (byte?)null,
                                     d.PayFrequencyId,
                                 });

                var totals = await baseQuery.GroupBy(x => true).Select(x => new
                {
                    TotalForfeitures = x.Sum(c => c.Forfeitures),
                    TotalGrossWages = x.Sum(c => c.GrossWages),
                    TotalLoans = x.Sum(c => c.Loans),
                    TotalProfitSharingAmount = x.Sum(c => c.ProfitSharingAmount)
                }).FirstOrDefaultAsync(cancellationToken);
                var pagedData = await baseQuery.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
                var reportDemographics = new PaginatedResponseDto<GrossWagesReportDetail>(req)
                {
                    Results = pagedData.Results.Select(x => new GrossWagesReportDetail
                    {
                        BadgeNumber = x.BadgeNumber,
                        EmployeeName = x.EmployeeName,
                        DateOfBirth = x.DateOfBirth,
                        Ssn = x.Ssn.MaskSsn(),
                        Forfeitures = x.Forfeitures ?? 0,
                        Loans = x.Loans ?? 0,
                        ProfitSharingAmount = x.ProfitSharingAmount ?? 0,
                        GrossWages = x.GrossWages,
                        EnrollmentId = (x.EnrollmentId ?? 0),
                        IsExecutive = x.PayFrequencyId == PayFrequency.Constants.Monthly,
                    }).ToList(),
                    Total = pagedData.Total
                };
                return new { reportDemographics, totals };
            }, cancellationToken);

            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
            return new GrossWagesReportResponse()
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = calInfo.FiscalBeginDate,
                EndDate = calInfo.FiscalEndDate,
                ReportName = GrossWagesReportResponse.REPORT_NAME,
                Response = rslt.reportDemographics,
                TotalForfeitures = rslt?.totals?.TotalForfeitures ?? 0m,
                TotalGrossWages = rslt?.totals?.TotalGrossWages ?? 0m,
                TotalLoans = rslt?.totals?.TotalLoans ?? 0m,
                TotalProfitSharingAmount = rslt?.totals?.TotalProfitSharingAmount ?? 0m
            };
        }

    }

    public async Task<ProfitControlSheetResponse> GetProfitControlSheet(ProfitYearRequest request, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request PROFIT CONTROL SHEET"))
        {
            return await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                DateOnly fiscalEndDate = (await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken)).FiscalEndDate;
                ProfitControlSheetResponse response = new();
                // Build composite key sets (Ssn, Id) to disambiguate potential duplicate SSNs with different Demographic/Beneficiary records.
                var employeeKeys = await (await _demographicReaderService.BuildDemographicQuery(ctx, true))
                    .Select(d => new { d.Ssn, d.Id })
                    .ToListAsync(cancellationToken);
                var employeeKeySet = employeeKeys
                    .Select(x => (x.Ssn, x.Id))
                    .ToHashSet();

                var beneKeys = await ctx.BeneficiaryContacts
                    .Select(bc => new { bc.Ssn, bc.Id })
                    .ToListAsync(cancellationToken);
                var beneKeySet = beneKeys
                    .Select(x => (x.Ssn, x.Id))
                    .ToHashSet();

                // Determine disjoint and overlapping composite keys.
                var pureEmployee = employeeKeySet.Except(beneKeySet).ToHashSet();
                var pureBene = beneKeySet.Except(employeeKeySet).ToHashSet();
                var both = employeeKeySet.Intersect(beneKeySet).ToHashSet();

                // Build a dictionary keyed by composite (Ssn, Id) to avoid duplicate key exceptions when multiple rows share the same SSN.
                // Name tuple elements so we can reference kvp.Key.Ssn clearly below.
                var allMemberCurrentBalance = await _totalService.TotalVestingBalance(ctx, request.ProfitYear, fiscalEndDate)
                    .ToDictionaryAsync(k => (Ssn: k.Ssn, Id: k.Id), v => v.CurrentBalance ?? 0m, cancellationToken);

                // Sum by SSN set membership. Multiple (Ssn, Id) rows for the same Ssn will correctly accumulate.
                response.EmployeeContributionProfitSharingAmount = allMemberCurrentBalance
                    .Where(kvp => pureEmployee.Contains(kvp.Key))
                    .Sum(kvp => kvp.Value);

                response.NonEmployeeProfitSharingAmount = allMemberCurrentBalance
                    .Where(kvp => pureBene.Contains(kvp.Key))
                    .Sum(kvp => kvp.Value);

                response.EmployeeBeneficiaryAmount = allMemberCurrentBalance
                    .Where(kvp => both.Contains(kvp.Key))
                    .Sum(kvp => kvp.Value);
                return response;
            }, cancellationToken);
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

    internal class ForfeitureAndPointsDbResponse
    {
        internal required PaginatedResponseDto<ForfeituresAndPointsForYearResponse> Pagination { get; set; }
        internal required ForfeitureAndPointsTotals Totals { get; set; }
    }

    internal class ForfeitureAndPointsTotals
    {
        internal decimal TotalForfeitures { get; set; }
        [MaskSensitive]
        internal int TotalForfeitPoints { get; set; }
        [MaskSensitive]
        internal int TotalEarningPoints { get; set; }
        internal decimal AllocationsFromTotals { get; set; }
        internal decimal AllocationsToTotals { get; set; }
        internal decimal DistributionTotals { get; set; }
        internal decimal TotalProfitSharingBalance { get; set; }
    }
}
