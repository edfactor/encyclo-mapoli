using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;
internal sealed class MissiveService : IMissiveService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ITotalService _totalService;
    private readonly ILogger<MissiveService> _logger;

    public MissiveService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory
    ) 
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _logger = loggerFactory.CreateLogger<MissiveService>();
    }
    public async Task<List<int>> DetermineMissivesForSsn(int ssn, short profitYear, CancellationToken cancellation)
    {
        using (_logger.BeginScope("Searching for missives for ssn {0}", ssn.MaskSsn()))
        {
            var rslt = new List<int>();
            _ = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {

                if (await HasNewVestingPlanHasContributions(ctx, ssn, profitYear, cancellation))
                {
                    rslt.Add(Missive.Constants.VestingIncreasedOnCurrentBalance);
                }

                return Task.FromResult(true);
            }).Unwrap();

            return rslt;
        }
    }

    public Task<List<MissiveResponse>> GetAllMissives(CancellationToken token)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return ctx.Missives.Select(x => new MissiveResponse() { Id = x.Id, Message = x.Message }).ToListAsync(token);
        });
    }

    private async Task<bool> HasNewVestingPlanHasContributions(ProfitSharingReadOnlyDbContext ctx,int ssn, short profitYear, CancellationToken cancellation)
    {
        var memberBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(Common.Contracts.Request.SearchBy.Ssn, ssn, profitYear, cancellation);
        if (memberBalance != null && memberBalance.YearsInPlan >= 2 && memberBalance.YearsInPlan <= 7)
        {
            var vestingIncreased = await (
                from d in ctx.Demographics
                join pp in ctx.PayProfits.Where(x => x.ProfitYear == profitYear) on d.Id equals pp.DemographicId
                where d.Ssn == ssn
                  && pp.CurrentHoursYear + pp.HoursExecutive > ReferenceData.MinimumHoursForContribution()
                  && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions
                select d.Id

            ).AnyAsync(cancellation);
            return true;
        }

        return false;
    }
}
