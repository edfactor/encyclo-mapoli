using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

internal sealed class MissiveService : IMissiveService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ITotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger<MissiveService> _logger;

    public MissiveService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory,
        IDemographicReaderService demographicReaderService
    )
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
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
            await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // Pre-fetch demographics for all SSNs
                var demographics = await _demographicReaderService.BuildDemographicQueryAsync(ctx);
                var dobList = await demographics
                    .Where(d => ssnSet.Contains(d.Ssn))
                    .Select(d => new { d.Ssn, d.DateOfBirth })
                    .ToListAsync(cancellation);

                var dobBySsn = dobList
                    .GroupBy(d => d.Ssn)
                    .ToDictionary(g => g.Key, g => g.First().DateOfBirth);

                var employeeList = await demographics.Join(ctx.PayProfits, d => d.Id, pp => pp.DemographicId, (d, pp) => new { d, pp })
                    .Where(empl => ssnSet.Contains(empl.d.Ssn))
                    .Where(emp => emp.pp.ProfitYear == profitYear)
                    .ToListAsync(cancellation);

                // Pre-fetch balances for all SSNs
                var balances = await _totalService.GetVestingBalanceForMembersAsync(Common.Contracts.Request.SearchBy.Ssn, ssnSet, profitYear, cancellation);
                Dictionary<int, BalanceEndpointResponse> balanceMap = new();
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
                    .Where(empl => empl.pp.ZeroContributionReasonId == /*7*/ ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay)
                    .Select(empl => empl.d.Ssn).ToHashSet();

                var vestingNow100 = employeeList
                    .Where(empl => empl.pp.ZeroContributionReasonId ==  /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
                    .Select(empl => empl.d.Ssn).ToHashSet();

                var minHours = ReferenceData.MinimumHoursForContribution;
                var vestingIncreased = new HashSet<int>();

                foreach (var empl in employeeList)
                {
                    if (balanceMap.TryGetValue(empl.d.Ssn, out var memberBalance) && memberBalance is { YearsInPlan: >= 2 and <= 7 })
                    {
                        // Check if using New Vesting Plan (VestingScheduleId == 2) and has contributions (no forfeiture)
                        var hasVesting = empl.pp.TotalHours > minHours &&
                                          empl.pp.VestingScheduleId == VestingSchedule.Constants.NewPlan &&
                                          !empl.pp.HasForfeited;
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

                    if (vestingNow100.Contains(ssn) && balanceMap.TryGetValue(ssn, out BalanceEndpointResponse? value) && value.CurrentBalance > 0)
                    {
                        missives.Add(Missive.Constants.VestingIsNow100Percent);
                    }

                    if (dobBySsn.TryGetValue(ssn, out var dob) &&
                        balanceMap.TryGetValue(ssn, out var balance) &&
                        IsUnderAgeAtDate(dob, DateOnly.FromDateTime(DateTime.Today), underAgeThreshold: 21) &&
                        (balance.CurrentBalance > 0 || balance.VestedBalance > 0))
                    {
                        missives.Add(Missive.Constants.EmployeeUnder21WithBalance);
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
            }, cancellation);
            return result;
        }
    }

    public Task<List<MissiveResponse>> GetAllMissives(CancellationToken token)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx => ctx.Missives.Select(x => new MissiveResponse { Id = x.Id, Message = x.Message, Description = x.Description, Severity = x.Severity }).ToListAsync(token), token);
    }

    private static bool IsUnderAgeAtDate(DateOnly dateOfBirth, DateOnly asOf, int underAgeThreshold)
    {
        var age = dateOfBirth.Age(asOf.ToDateTime(TimeOnly.MinValue));
        return age < underAgeThreshold;
    }
}
