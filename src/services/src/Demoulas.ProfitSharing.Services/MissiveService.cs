using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;
internal sealed class MissiveService : IMissiveService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ITotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger<MissiveService> _logger;

    public MissiveService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory,
        ICalendarService calendarService,
        IDemographicReaderService demographicReaderService
    ) 
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
        _demographicReaderService = demographicReaderService;
        _logger = loggerFactory.CreateLogger<MissiveService>();
    }

    public async Task<Dictionary<int, List<int>>> DetermineMissivesForSsns(IEnumerable<int> ssns, short profitYear, CancellationToken cancellation)
    {
        var ssnSet = ssns.ToHashSet();
        var result = new Dictionary<int, List<int>>();
        if (ssnSet.Count == 0)
        {
            return result;
        }

        using (_logger.BeginScope("Searching for missives for ssns {0}", string.Join(",", ssnSet.Select(x => x.MaskSsn()))))
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, cancellation);
            await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // Pre-fetch demographics for all SSNs
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                var demoList = await demographics.Where(d => ssnSet.Contains(d.Ssn)).ToListAsync(cancellation);
                var demoIds = demoList.Select(d => d.Id).ToList();

                // Pre-fetch balances for all SSNs
                var balances = await _totalService.GetVestingBalanceForMembersAsync(Common.Contracts.Request.SearchBy.Ssn, ssnSet, profitYear, cancellation);
                var balanceMap = balances.ToDictionary(b => b.Id, b => b);

                // Pre-fetch PayProfits for all demographics in profitYear
                var payProfits = await ctx.PayProfits.Where(pp => pp.ProfitYear == profitYear && demoIds.Contains(pp.DemographicId)).ToListAsync(cancellation);

                // Pre-fetch BeneficiaryContacts for all SSNs
                var beneficiaryContacts = await ctx.BeneficiaryContacts.Where(bc => ssnSet.Contains(bc.Ssn)).Select(bc => bc.Ssn).Distinct().ToListAsync(cancellation);

                // Pre-fetch for EmployeeMayBe100Percent
                var zeroContributionReasonId = ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay;
                var mayBe100Percent = await (from pp in ctx.PayProfits
                    where pp.ProfitYear == profitYear && pp.ZeroContributionReasonId == zeroContributionReasonId && demoIds.Contains(pp.DemographicId)
                    join d in demographics on pp.DemographicId equals d.Id
                    select d.Ssn).Distinct().ToListAsync(cancellation);

                // Pre-calculate 65th birthday cutoff
                var sixtyFiveBirthDate = DateOnly.FromDateTime(DateTime.Today).AddYears(-65);

                // Pre-fetch for VestingIsNow100Percent
                var vestingNow100 = demoList.Where(d =>
                    d.DateOfBirth <= sixtyFiveBirthDate &&
                    (!d.TerminationDate.HasValue || d.TerminationDate > calInfo.FiscalEndDate) &&
                    d.TerminationCodeId != TerminationCode.Constants.Deceased
                ).Select(d => d.Ssn).ToHashSet();

                // Pre-fetch for HasNewVestingPlanHasContributions
                var minHours = ReferenceData.MinimumHoursForContribution();
                var newVestingPlanId = Enrollment.Constants.NewVestingPlanHasContributions;
                var vestingIncreased = new HashSet<int>();
                foreach (var d in demoList)
                {
                    if (balanceMap.TryGetValue(d.Ssn, out var memberBalance) && memberBalance is { YearsInPlan: >= 2 and <= 7 })
                    {
                        var hasVesting = payProfits.Any(pp => pp.DemographicId == d.Id &&
                                                              pp.CurrentHoursYear + pp.HoursExecutive > minHours &&
                                                              pp.EnrollmentId == newVestingPlanId);
                        if (hasVesting)
                        {
                            vestingIncreased.Add(d.Ssn);
                        }
                    }
                }

                foreach (var ssn in ssnSet)
                {
                    var missives = new List<int>();
                    if (vestingIncreased.Contains(ssn))
                    {
                        missives.Add(Missive.Constants.VestingIncreasedOnCurrentBalance);
                    }

                    if (vestingNow100.Contains(ssn))
                    {
                        missives.Add(Missive.Constants.VestingIsNow100Percent);
                    }

                    if (beneficiaryContacts.Contains(ssn))
                    {
                        missives.Add(Missive.Constants.EmployeeIsAlsoABeneficiary);
                    }

                    if (mayBe100Percent.Contains(ssn))
                    {
                        missives.Add(Missive.Constants.EmployeeMayBe100Percent);
                    }

                    result[ssn] = missives;
                }

                return Task.FromResult(true);
            });
            return result;
        }
    }

    public Task<List<MissiveResponse>> GetAllMissives(CancellationToken token)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return ctx.Missives.Select(x => new MissiveResponse() { Id = x.Id, Message = x.Message, Description = x.Description, Severity = x.Severity}).ToListAsync(token);
        });
    }
}
