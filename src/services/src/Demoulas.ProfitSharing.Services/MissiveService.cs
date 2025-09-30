using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
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
                var employeeList = await demographics.Join(ctx.PayProfits, d=>d.Id, pp=>pp.DemographicId, (d,pp)=>new {d,pp})
                    .Where(empl => ssnSet.Contains(empl.d.Ssn))
                    .Where(emp=>emp.pp.ProfitYear == profitYear)
                    .ToListAsync(cancellation);
                var demoIds = employeeList.Select(empl => empl.d.Id).ToList();

                // Pre-fetch balances for all SSNs
                var balances = await _totalService.GetVestingBalanceForMembersAsync(Common.Contracts.Request.SearchBy.Ssn, ssnSet, profitYear, cancellation);
                Dictionary<int, BalanceEndpointResponse> balanceMap = new ();
                try
                {
                    balanceMap = balances.ToDictionary(b => b.Id, b => b);
                }
                catch (ArgumentException)
                {
                    //Swallow duplicate key exception
                }

                // Pre-fetch BeneficiaryContacts for all SSNs
                var beneficiaryContacts = await ctx.BeneficiaryContacts.Where(bc => ssnSet.Contains(bc.Ssn)).Select(bc => bc.Ssn).Distinct().ToListAsync(cancellation);

                var mayBe100Percent = employeeList
                    .Where(empl=>empl.pp.ZeroContributionReasonId == /*7*/ ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay)
                    .Select(empl=>empl.d.Ssn).ToHashSet();

                var vestingNow100 = employeeList
                    .Where(empl=>empl.pp.ZeroContributionReasonId ==  /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
                    .Select(empl=>empl.d.Ssn).ToHashSet();

                var minHours = ReferenceData.MinimumHoursForContribution();
                var vestingIncreased = new HashSet<int>();
                
                foreach (var empl in employeeList)
                {
                    if (balanceMap.TryGetValue(empl.d.Ssn, out var memberBalance) && memberBalance is { YearsInPlan: >= 2 and <= 7 })
                    {
                        var hasVesting =  empl.pp.CurrentHoursYear + empl.pp.HoursExecutive > minHours &&
                                          empl.pp.EnrollmentId == /*2*/ Enrollment.Constants.NewVestingPlanHasContributions;
                        if (hasVesting)
                        {
                            vestingIncreased.Add(empl.d.Ssn);
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

                    if (vestingNow100.Contains(ssn) && balanceMap.ContainsKey(ssn) && balanceMap[ssn].CurrentBalance > 0)
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
