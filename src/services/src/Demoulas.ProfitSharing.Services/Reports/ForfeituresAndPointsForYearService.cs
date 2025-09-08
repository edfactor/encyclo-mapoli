using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

// Builds the data which is presented in the PAY443 report in READY

public class ForfeituresAndPointsForYearService : IForfeituresAndPointsForYearService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public ForfeituresAndPointsForYearService(
        IProfitSharingDataContextFactory dataContextFactory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService
    )
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
    }


    public async Task<ForfeituresAndPointsForYearResponseWithTotals> GetForfeituresAndPointsForYearAsync(FrozenProfitYearRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            short currentYear = request.ProfitYear;
            short lastYear = (short)(currentYear - 1);

            ForfeituresAndPointsForYearResponseWithTotals response = await ComputeTotals(ctx, currentYear, lastYear, cancellationToken);

            // Employee details
            IEnumerable<ForfeituresAndPointsForYearResponse> members = await GetMembers(ctx, currentYear, cancellationToken);

            // Each employee's value is independently rounded. 
            int earningPoints = members.Sum(r => r.EarningPoints);

            PaginatedResponseDto<ForfeituresAndPointsForYearResponse> paginatedData = await members.AsQueryable().ToPaginationResultsAsync(request, cancellationToken);

            return response with { TotalEarningPoints = earningPoints, Response = paginatedData };
        });
    }


    private async Task<ForfeituresAndPointsForYearResponseWithTotals> ComputeTotals(ProfitSharingReadOnlyDbContext ctx, short currentYear, short lastYear,
        CancellationToken cancellationToken)
    {
        decimal? lastYearTotal = await _totalService.TotalVestingBalance(ctx, lastYear, DateOnly.MaxValue).AsNoTracking()
            .SumAsync(ptvb => ptvb.CurrentBalance, cancellationToken);

        var transactionsInCurrentYear = await _totalService.GetTransactionsBySsnForProfitYearForOracle(ctx, currentYear).ToListAsync(cancellationToken);
        decimal distributionsTotal = transactionsInCurrentYear.Sum(syd => syd.DistributionsTotal);
        decimal paidAllocationsTotal = transactionsInCurrentYear.Sum(syd => syd.PaidAllocationsTotal);
        decimal allocationsTotal = transactionsInCurrentYear.Sum(syd => syd.AllocationsTotal);
        decimal forfeitsTotal = transactionsInCurrentYear.Sum(syd => syd.ForfeitsTotal);

        int totalContForfeitPoints = (int)(await ctx.PayProfits.Where(p => p.ProfitYear == currentYear).SumAsync(p => p.PointsEarned, cancellationToken))!;

        return new ForfeituresAndPointsForYearResponseWithTotals
            {
                DistributionTotals = distributionsTotal,
                AllocationsFromTotals = allocationsTotal,
                AllocationToTotals = paidAllocationsTotal,
                TotalForfeitures = forfeitsTotal,
                TotalEarningPoints = 0,
                TotalForfeitPoints = totalContForfeitPoints,
                TotalProfitSharingBalance = lastYearTotal,
                ReportDate = DateTimeOffset.UtcNow,
                ReportName = $"PROFIT SHARING FORFEITURES AND POINTS FOR {currentYear}",
                StartDate = new DateOnly(currentYear,
                    1,
                    1),
                EndDate = new DateOnly(currentYear,
                    12,
                    31),
                Response = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse> { Total = 0, Results = [] }
            }
            ;
    }

    private async Task<IEnumerable<ForfeituresAndPointsForYearResponse>> GetMembers(ProfitSharingReadOnlyDbContext ctx, short currentYear, CancellationToken cancellationToken)
    {
        // Get current balances for all members (some members could have no balance)
        Dictionary<int, ParticipantTotalVestingBalance> memberAmountsBySsn = await _totalService
            .TotalVestingBalance(ctx, currentYear, DateOnly.MaxValue)
            .AsNoTracking()
            .ToDictionaryAsync(ptvb => ptvb.Ssn, ptvb => ptvb, cancellationToken);

        // Get this year's transactions for all members (some members could have no transactions)
        var transactionsInCurrentYearBySsn =
            await _totalService.GetTransactionsBySsnForProfitYearForOracle(ctx, currentYear)
                .AsNoTracking()
                .ToDictionaryAsync(ptvb => ptvb.Ssn, ptvb => ptvb, cancellationToken);

        // Gather all the employees
        IQueryable<Demographic> demographicExpression = await _demographicReaderService.BuildDemographicQuery(ctx, true);
        var employeesRaw = await demographicExpression
            .Join(ctx.PayProfits, d => d.Id, pp => pp.DemographicId, (d, pp) => new { d, pp })
            .AsNoTracking()
            .Where(pp => pp.pp.ProfitYear == currentYear).ToListAsync(cancellationToken);

        Dictionary<int, ForfeituresAndPointsForYearResponse> employeeMembersBySsn = employeesRaw
            .ToDictionary(d => d.d.Ssn, v => ToMemberDetails(v.d, memberAmountsBySsn.ContainsKey(v.d.Ssn) ? memberAmountsBySsn[v.d.Ssn] : null, v.pp,
                transactionsInCurrentYearBySsn.ContainsKey(v.d.Ssn) ? transactionsInCurrentYearBySsn[v.d.Ssn] : null));

        // Gather Bene's
        Dictionary<int, ForfeituresAndPointsForYearResponse> beneMembers = await ctx.Beneficiaries
            .Include(b => b.Contact)
            .AsNoTracking()
            .Where(b => !employeeMembersBySsn.Keys.Contains(b.Contact!.Ssn)) // omit employees
            .ToDictionaryAsync(b => b.Contact!.Ssn, v => ToMemberDetails(v, memberAmountsBySsn.ContainsKey(v.Contact!.Ssn) ? memberAmountsBySsn[v.Contact!.Ssn] : null),
                cancellationToken);

        List<ForfeituresAndPointsForYearResponse> members = employeeMembersBySsn.Values.Concat(beneMembers.Values)
            .OrderBy(m => m.EmployeeName, StringComparer.Ordinal)
            .ThenByDescending(m => m.BadgeNumber)
            .ToList();

        // Filter out members with nothing going on
        members = members.Where(m => m.ContForfeitPoints != 0 || m.EarningPoints != 0 || m.Forfeitures != 0).ToList();

        return members;
    }


    private static ForfeituresAndPointsForYearResponse ToMemberDetails(Demographic d, ParticipantTotalVestingBalance? ptvb, PayProfit pp,
        ProfitDetailRollup? singleYearNumbers
    )
    {
        decimal balanceConsideredForEarnings = (ptvb?.CurrentBalance ?? 0) - (singleYearNumbers?.MilitaryTotal ?? 0) - (singleYearNumbers?.ClassActionFundTotal ?? 0);
        int earningsPoints = (int) Math.Round(balanceConsideredForEarnings / 100, MidpointRounding.AwayFromZero);
        decimal forfeitures = singleYearNumbers == null ? 0.00m : -1 * singleYearNumbers.TotalForfeitures;

        return new ForfeituresAndPointsForYearResponse
        {
            EmployeeName = d.ContactInfo.FullName!,
            BadgeNumber = d.BadgeNumber,
            Ssn = d.Ssn.MaskSsn(),
            Forfeitures = forfeitures,
            ContForfeitPoints = (short)(pp.PointsEarned ?? 0), // Yea, PointsEarned is not EarningPoints
            EarningPoints = earningsPoints,
            IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
        };
    }

    private static ForfeituresAndPointsForYearResponse ToMemberDetails(Beneficiary b, ParticipantTotalVestingBalance? ptvb)
    {
        short earningsPoints = ptvb != null ? (short)Math.Round((ptvb.CurrentBalance ?? 0) / 100, MidpointRounding.AwayFromZero) : (short)0;
        return new ForfeituresAndPointsForYearResponse
        {
            EmployeeName = b.Contact!.ContactInfo.FullName!,
            BadgeNumber = 0,
            Ssn = b.Contact!.Ssn.MaskSsn(),
            Forfeitures = 0.00m,
            ContForfeitPoints = 0,
            EarningPoints = earningsPoints,
            BeneficiaryPsn = "0" + b.Psn,
            IsExecutive = false,
        };
    }
}
