using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitMaster;

/// <summary>
/// <para>
/// Profit Master Update - inserts or deletes PROFIT_DETAIL rows based on values provided by the user.
/// </para>
/// <para>
/// Invokes the Profit Share Edit Service to compute and return the transactions (PROFIT_DETAIL rows) based on user input.
/// The resulting records are inserted or deleted in the PROFIT_DETAILS database.
///
/// The employees ETVA field will be adjusted when inserting and records with an "100% Earnings" (profit_code=8) records.
/// The ETVA field will be decremented when performing deletes.
///
/// The class name follows the name of the step in the Ready YE flow.
/// </summary>
public class ProfitMasterService : IProfitMasterService
{
    private readonly IProfitShareEditService _profitShareEditService;
    private readonly IProfitSharingDataContextFactory _dbFactory;

    public ProfitMasterService(IProfitShareEditService profitShareEditService, IProfitSharingDataContextFactory dbFactory)
    {
        _profitShareEditService = profitShareEditService;
        _dbFactory = dbFactory;
    }

    public async Task<ProfitMasterResponse> Update(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        int etvasUpdated = 0;
        var records = await _profitShareEditService.ProfitShareEditRecords(profitShareUpdateRequest, cancellationToken);

        return await _dbFactory.UseWritableContext(async ctx =>
        {
            var code2ProfitCode = await ctx.ProfitCodes.ToDictionaryAsync(pc => pc.Id, pc => pc, cancellationToken);

            // Create records and batch insert them.
            var profitDetailRecords = CreateProfitDetailRecords(code2ProfitCode, profitShareUpdateRequest.ProfitYear, records);
            ctx.ProfitDetails.AddRange(profitDetailRecords);

            // create ETVA updates 
            var employeeSsns = records.Select(r => r.Ssn).ToHashSet();
            var payProfits = await ctx.PayProfits.Include(pp => pp.Demographic)
                .Where(pp => pp.ProfitYear == profitShareUpdateRequest.ProfitYear && employeeSsns.Contains(pp.Demographic!.Ssn))
                .ToListAsync(cancellationToken);
            var ssn2PayProfit = payProfits.ToDictionary(pp => pp.Demographic!.Ssn, pp => pp);
            foreach (var earningRecord in records.Where(r => r.Code == /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings.Id && r.IsEmployee))
            {
                var pp = ssn2PayProfit[earningRecord.Ssn];
                pp.HoursExecutive = pp.HoursExecutive + 0; // This is NOP until Etva is added back.
                // pp.Etva = rec8.Earnings
                etvasUpdated++;
            }

            await ctx.SaveChangesAsync(cancellationToken);
            var employeesEffected = records.Where(m => m.IsEmployee).Select(m => m.Ssn).ToHashSet().Count;
            var beneficiariesEffected = records.Where(m => !m.IsEmployee).Select(m => m.Ssn).ToHashSet().Count;

            return new ProfitMasterResponse { BeneficiariesEffected = beneficiariesEffected, EmployeesEffected = employeesEffected, EtvasEffected = etvasUpdated }!;
        }, cancellationToken);
    }

    private static List<ProfitDetail> CreateProfitDetailRecords(Dictionary<byte, ProfitCode> id2ProfitCode, short profitYear,
        IEnumerable<ProfitShareEditMemberRecord> rec)
    {
        return rec.Select(r => new ProfitDetail
        {
            ProfitCode = id2ProfitCode[r.Code],
            Ssn = r.Ssn,
            ProfitYear = profitYear,
            ProfitYearIteration = r.YearExtension,
            ProfitCodeId = r.Code,
            Contribution = r.ContributionAmount,
            Earnings = r.EarningAmount,
            Forfeiture = r.ForfeitureAmount,
            Remark = r.Remark,
            ZeroContributionReasonId = r.ZeroContStatus,
            CommentTypeId = r.CommentTypeId,
        }).ToList();
    }

#pragma warning disable AsyncFixer01
    public async Task<ProfitMasterResponse> Revert(ProfitYearRequest profitYearRequest, CancellationToken cancellationToken)
    {
        return await _dbFactory.UseWritableContext(async ctx =>
        {
            // read this years contribution/vestingEarnings profit_detail rows
            var pds = await ctx.ProfitDetails
                .Where(pd => pd.ProfitYear == profitYearRequest.ProfitYear &&
                             (pd.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id
                              || pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id))
                .ToListAsync(cancellationToken);

            var memberSsns = pds.Select(p => p.Ssn).ToHashSet();
            var ssn2PayProfit = await ctx.PayProfits.Where(pp => pp.ProfitYear == profitYearRequest.ProfitYear && memberSsns.Contains(pp.Demographic!.Ssn))
                .ToDictionaryAsync(pp => pp.Demographic!.Ssn, pp => pp, cancellationToken);

            var etvaReset = new HashSet<int>();
            // Adjust ETVA
            foreach (var etvaRec in pds.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id && ssn2PayProfit.ContainsKey(pd.Ssn))
                         .Select(pd => new { pd, pp = ssn2PayProfit[pd.Ssn] }))
            {
                //pp.Etva -= pd.Earnings
                Console.WriteLine(etvaRec.pd + " " + etvaRec.pp);
                etvaReset.Add(etvaRec.pd.Ssn);
            }

            ctx.ProfitDetails.RemoveRange(pds);
            await ctx.SaveChangesAsync(cancellationToken);
            return new ProfitMasterResponse
            {
                BeneficiariesEffected = memberSsns.Count - ssn2PayProfit.Count, EmployeesEffected = ssn2PayProfit.Count, EtvasEffected = etvaReset.Count
            };
        }, cancellationToken);
    }
}
