using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Services.Reports;

// Builds the data which is presented in the PAY443 report in READY

public class ForfeituresAndPointsForYearService : IForfeituresAndPointsForYearService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IPayrollDuplicateSsnReportService _duplicateSsnReportService;
    private readonly ICrossReferenceValidationService _crossReferenceValidationService;
    private readonly TotalService _totalService;

    public ForfeituresAndPointsForYearService(
        IProfitSharingDataContextFactory dataContextFactory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        IPayrollDuplicateSsnReportService duplicateSsnReportService,
        ICrossReferenceValidationService crossReferenceValidationService
    )
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _duplicateSsnReportService = duplicateSsnReportService;
        _crossReferenceValidationService = crossReferenceValidationService;
    }


    public Task<ForfeituresAndPointsForYearResponseWithTotals> GetForfeituresAndPointsForYearAsync(FrozenProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            short currentYear = req.ProfitYear;
            short lastYear = (short)(currentYear - 1);

            // Validate no duplicate SSNs exist
            var validator = new InlineValidator<short>();
            validator.RuleFor(r => r)
                .MustAsync(async (_, ct) => !await _duplicateSsnReportService.DuplicateSsnExistsAsync(ct))
                .WithMessage("There are presently duplicate SSN's in the system, which will cause this process to fail.");
            await validator.ValidateAndThrowAsync(currentYear, cancellationToken);

            // Execute queries sequentially (DbContext is not thread-safe)
            // Query 1: Last year total balance - aggregate in database
            decimal? lastYearTotal = await _totalService.TotalVestingBalance(ctx, lastYear, DateOnly.MaxValue)

                .TagWith($"ForfeituresReport-LastYearTotal-{lastYear}")
                .SumAsync(ptvb => ptvb.CurrentBalance, cancellationToken);

            // Query 2: Current year transactions (reused for totals and member details)
            var transactionsInCurrentYear = await _totalService.GetTransactionsBySsnForProfitYearForOracle(ctx, currentYear)

                .TagWith($"ForfeituresReport-Transactions-{currentYear}")
                .ToListAsync(cancellationToken);

            // Query 3: Current year balances with projection (only load needed fields)
            var currentBalances = await _totalService.TotalVestingBalance(ctx, currentYear, DateOnly.MaxValue)

                .TagWith($"ForfeituresReport-CurrentBalances-{currentYear}")
                .Select(b => new { b.Ssn, b.Id, b.CurrentBalance })
                .ToListAsync(cancellationToken);

            // Query 4: Demographics joined with PayProfits with projection (only load needed fields)
            IQueryable<Demographic> demographicExpression = await _demographicReaderService.BuildDemographicQueryAsync(ctx, true);
            var employeeData = await (
                from demo in demographicExpression
                join pp in ctx.PayProfits on demo.Id equals pp.DemographicId
                where pp.ProfitYear == currentYear
                select new
                {
                    // Demographic fields
                    DemographicId = demo.Id,
                    demo.Ssn,
                    demo.BadgeNumber,
                    FullName = demo.ContactInfo.FullName,
                    demo.PayFrequencyId,
                    // PayProfit fields
                    pp.PointsEarned
                })
                .TagWith($"ForfeituresReport-EmployeeData-{currentYear}")
                .ToListAsync(cancellationToken);

            // Query 5: Beneficiaries with projection (only load needed fields)
            var beneficiaries = await ctx.Beneficiaries
                .Where(b => !b.IsDeleted)
                .TagWith($"ForfeituresReport-Beneficiaries-{currentYear}")
                .Where(b => b.Contact != null)
                .Select(b => new
                {
                    ContactSsn = b.Contact!.Ssn,
                    ContactId = b.Contact!.Id,
                    ContactFullName = b.Contact.ContactInfo.FullName,
                    b.BadgeNumber,
                    b.PsnSuffix
                })
                .ToListAsync(cancellationToken);

            // Build fast lookup dictionaries (in-memory, very fast)
            var transactionsBySsn = transactionsInCurrentYear.ToLookup(t => t.Ssn);
            var balancesByKey = currentBalances.ToDictionary(b => (b.Ssn, b.Id));

            // Compute totals (in-memory aggregation)
            decimal distributionsTotal = transactionsInCurrentYear.Sum(syd => syd.DistributionsTotal);
            decimal paidAllocationsTotal = transactionsInCurrentYear.Sum(syd => syd.PaidAllocationsTotal);
            decimal allocationsTotal = transactionsInCurrentYear.Sum(syd => syd.AllocationsTotal);
            decimal forfeitsTotal = transactionsInCurrentYear.Sum(syd => syd.ForfeitsTotal);
            int totalContForfeitPoints = (int)employeeData.Sum(e => e.PointsEarned ?? 0);

            // Build employee members
            var employeeSsns = new HashSet<int>();
            var members = new List<ForfeituresAndPointsForYearResponse>(employeeData.Count + 100);

            foreach (var emp in employeeData)
            {
                employeeSsns.Add(emp.Ssn);
                var balance = balancesByKey.GetValueOrDefault((emp.Ssn, emp.DemographicId));
                var transaction = transactionsBySsn[emp.Ssn].FirstOrDefault();

                var member = ToMemberDetailsFromProjection(
                    emp.FullName!,
                    emp.BadgeNumber,
                    emp.Ssn,
                    emp.PayFrequencyId,
                    balance?.CurrentBalance,
                    emp.PointsEarned,
                    transaction);

                // Filter immediately
                if (member.ContForfeitPoints != 0 || member.EarningPoints != 0 || member.Forfeitures != 0)
                {
                    members.Add(member);
                }
            }

            // Add beneficiaries (excluding employees)
            foreach (var bene in beneficiaries.Where(b => !employeeSsns.Contains(b.ContactSsn)))
            {
                var balance = balancesByKey.GetValueOrDefault((bene.ContactSsn, bene.ContactId));

                var member = ToBeneficiaryMemberDetails(
                    bene.ContactFullName!,
                    bene.ContactSsn,
                    bene.BadgeNumber,
                    bene.PsnSuffix,
                    balance?.CurrentBalance);

                // Filter immediately
                if (member.EarningPoints != 0)
                {
                    members.Add(member);
                }
            }

            // Default sort if no SortBy specified
            if (string.IsNullOrWhiteSpace(req.SortBy))
            {
                members = members
                    .OrderBy(m => m.EmployeeName, StringComparer.Ordinal)
                    .ThenByDescending(m => m.BadgeNumber)
                    .ToList();
            }

            // Calculate total earning points
            int earningPoints = members.Sum(r => r.EarningPoints);

            // Apply pagination
            PaginatedResponseDto<ForfeituresAndPointsForYearResponse> paginatedData =
                await members.AsQueryable().ToPaginationResultsAsync(req, cancellationToken);

            var crossRefValidation = await _crossReferenceValidationService.ValidateForfeitureAndPointsReportAsync(currentYear, distributionsTotal, forfeitsTotal, cancellationToken);

            return new ForfeituresAndPointsForYearResponseWithTotals
            {
                DistributionTotals = distributionsTotal,
                AllocationsFromTotals = allocationsTotal,
                AllocationToTotals = paidAllocationsTotal,
                TotalForfeitures = forfeitsTotal,
                TotalEarningPoints = earningPoints,
                TotalForfeitPoints = totalContForfeitPoints,
                TotalProfitSharingBalance = lastYearTotal,
                ReportDate = DateTimeOffset.UtcNow,
                ReportName = $"PROFIT SHARING FORFEITURES AND POINTS FOR {currentYear}",
                StartDate = new DateOnly(currentYear, 1, 1),
                EndDate = new DateOnly(currentYear, 12, 31),
                Response = paginatedData,
                CrossReferenceValidation = crossRefValidation

            };
        }, cancellationToken);
    }

    // Optimized helper for employees when using projection
    private static ForfeituresAndPointsForYearResponse ToMemberDetailsFromProjection(
        string fullName,
        int badgeNumber,
        int ssn,
        int payFrequencyId,
        decimal? currentBalance,
        decimal? pointsEarned,
        ProfitDetailRollup? singleYearNumbers)
    {
        decimal balanceConsideredForEarnings = (currentBalance ?? 0) - (singleYearNumbers?.MilitaryTotal ?? 0) - (singleYearNumbers?.ClassActionFundTotal ?? 0);
        int earningsPoints = (int)Math.Round(balanceConsideredForEarnings / 100, 0, MidpointRounding.AwayFromZero);
        decimal forfeitures = singleYearNumbers == null ? 0.00m : -1 * singleYearNumbers.TotalForfeitures;

        return new ForfeituresAndPointsForYearResponse
        {
            EmployeeName = fullName,
            BadgeNumber = badgeNumber,
            Ssn = ssn.MaskSsn(),
            Forfeitures = forfeitures,
            ContForfeitPoints = (short)(pointsEarned ?? 0),
            EarningPoints = earningsPoints,
            IsExecutive = payFrequencyId == PayFrequency.Constants.Monthly,
        };
    }


    // Optimized helper for beneficiaries when using projection
    private static ForfeituresAndPointsForYearResponse ToBeneficiaryMemberDetails(string fullName, int ssn, int badgeNumber, short psnSuffix, decimal? currentBalance)
    {
        short earningsPoints = currentBalance != null ? (short)Math.Round(currentBalance.Value / 100, 0, MidpointRounding.AwayFromZero) : (short)0;
        string psn = $"{badgeNumber}{psnSuffix:D4}";

        return new ForfeituresAndPointsForYearResponse
        {
            EmployeeName = fullName,
            BadgeNumber = 0,
            Ssn = ssn.MaskSsn(),
            Forfeitures = 0.00m,
            ContForfeitPoints = 0,
            EarningPoints = earningsPoints,
            BeneficiaryPsn = "0" + psn,
            IsExecutive = false,
        };
    }
}
