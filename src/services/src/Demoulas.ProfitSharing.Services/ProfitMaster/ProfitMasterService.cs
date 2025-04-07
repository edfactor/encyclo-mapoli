using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitMaster;

/// <summary>
/// Profit Master Update - inserts or deletes PROFIT_DETAIL rows based on values provided by the user.
///
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
    private readonly IInternalProfitShareEditService _profitShareEditService;
    private readonly IProfitSharingDataContextFactory _dbFactory;
    private readonly IAppUser _appUser;

    public ProfitMasterService(IInternalProfitShareEditService profitShareEditService, IProfitSharingDataContextFactory dbFactory, IAppUser appUser)
    {
        _profitShareEditService = profitShareEditService;
        _dbFactory = dbFactory;
        _appUser = appUser;
    }

// Not clear to me why I have to sprinkle this here
#pragma warning disable AsyncFixer01

    public async Task<ProfitMasterUpdateResponse?> Status(ProfitYearRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        return await _dbFactory.UseReadOnlyContext(async ctx =>
        {
            var yearEndUpdateStatus = await ctx.YearEndUpdateStatuses.Where(status => status.ProfitYear == profitShareUpdateRequest.ProfitYear)
                .FirstOrDefaultAsync(cancellationToken);
            if (yearEndUpdateStatus == null)
            {
                return null;
            }

            return new ProfitMasterUpdateResponse
            {
                BeneficiariesEffected = yearEndUpdateStatus.BeneficiariesEffected,
                EmployeesEffected = yearEndUpdateStatus.EmployeesEffected,
                EtvasEffected = yearEndUpdateStatus.EtvasEffected,
                UpdatedTime = yearEndUpdateStatus.UpdatedTime,
                UpdatedBy = yearEndUpdateStatus.UpdatedBy,
                ContributionPercent = yearEndUpdateStatus.ContributionPercent,
                IncomingForfeitPercent = yearEndUpdateStatus.IncomingForfeitPercent,
                EarningsPercent = yearEndUpdateStatus.EarningsPercent,
                SecondaryEarningsPercent = yearEndUpdateStatus.SecondaryEarningsPercent,
                MaxAllowedContributions = yearEndUpdateStatus.MaxAllowedContributions,
                BadgeAdjusted = yearEndUpdateStatus.BadgeAdjusted,
                BadgeAdjusted2 = yearEndUpdateStatus.BadgeAdjusted2,
                AdjustContributionAmount = yearEndUpdateStatus.AdjustContributionAmount,
                AdjustEarningsAmount = yearEndUpdateStatus.AdjustEarningsAmount,
                AdjustIncomingForfeitAmount = yearEndUpdateStatus.AdjustIncomingForfeitAmount,
                AdjustEarningsSecondaryAmount = yearEndUpdateStatus.AdjustEarningsSecondaryAmount
            };
        });
    }
#pragma warning restore AsyncFixer01

    public async Task<ProfitMasterUpdateResponse> Update(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        int etvasUpdated = 0;
        var (records, _, _, _, _) = await _profitShareEditService.ProfitShareEditRecords(profitShareUpdateRequest, cancellationToken);

        return await _dbFactory.UseWritableContext(async ctx =>
        {
            var yearEndUpdateStatus = await ctx.YearEndUpdateStatuses.Where(yeStatus => yeStatus.ProfitYear == profitShareUpdateRequest.ProfitYear)
                .FirstOrDefaultAsync(cancellationToken);
            if (yearEndUpdateStatus != null)
            {
                throw new BadHttpRequestException($"Can not add new profit detail records for year {profitShareUpdateRequest.ProfitYear} until existing ones are removed.");
            }

            var code2ProfitCode = await ctx.ProfitCodes.ToDictionaryAsync(pc => pc.Id, pc => pc, cancellationToken);

            // Create records and batch insert them.
            var profitDetailRecords = CreateProfitDetailRecords(code2ProfitCode, profitShareUpdateRequest.ProfitYear, records);
            ctx.ProfitDetails.AddRange(profitDetailRecords);

            // You could say, why do we even ask for the profitYear parameter? - profitYear is essentially locked to be last year.
            // TBD: will probably remove the profitYear parameter from the request soon.    
            if (profitShareUpdateRequest.ProfitYear != DateTime.Now.Year - 1)
            {
                throw new BadHttpRequestException($"The Profit year must be last year. {DateTime.Now.Year - 1}");
            }

            // This code only runs when the system is "FROZEN" which means in the beginning of a new year, and processing
            // last year's profit sharing.   We currently grab most profit sharing data from PayProfit using the prior year (aka profit year), but
            // the hot ETVA is located in the PayProfit for the wall-clock year (aka profitYear+1) 
            
            // Vocabulary;  "Now Year" or "wall clock year" or "profit year + 1" they are the same.  
            //              "Profit Year" is simply the profit year. 
            
            // This selection of which columns are from "profit year" vs "now year" is could change. 

            // This gist of why we care about two PAY_PROFIT rows, is that we need information from both,
            // EnrolledId               -> (either row?) which type of plan are they in 1,2 or are the out 3,4   (do we want now or at Fiscal Freeze?) 
            // PointsEarned             -> (profit year) set in PAY443/YearEndService.cs
            // ZeroContributionReasonId -> (profit year) set in PAY426/PayProfitUpdateService.cs
            // ETVA                     -> (profit year - write only!) - it becomes "last years ETVA" aka the ETVA when YE (this program) is run.
            // ETVA                     -> (Now Year aka "profit year + 1", tho "hot" ETVA for an employee

            // When the Profit Share Update is run, both the profitYear and profitYear+1 ETVAs need to be updated.
            // profitYear + 1 <--- because it is the hot "ETVA" which gets adjusted on demand.
            // profitYear     <--- because it is about to become the new "Last Years" ETVA amount - the amount used in this profit share run.
            // I believe they should both hold identical values when this is completed.   Note that doing a revert will restore the profitYear+1 ETVA to the value it had before the update.
            // and will zero out the ETVA for the profitYear.

            var nowYear = DateTime.Now.Year;
            var profitYear = profitShareUpdateRequest.ProfitYear;

            var employeeSsns = records.Select(r => r.Ssn).ToHashSet();
            var payProfits = await ctx.PayProfits.Include(pp => pp.Demographic)
                .Where(pp => (pp.ProfitYear == profitYear || pp.ProfitYear == nowYear) && employeeSsns.Contains(pp.Demographic!.Ssn))
                .ToListAsync(cancellationToken);

            var ssnByPayProfitNow = payProfits.Where(pp => pp.ProfitYear == nowYear).ToDictionary(pp => pp.Demographic!.Ssn, pp => pp);
            var ssnByPayProfitYearEnd = payProfits.Where(pp => pp.ProfitYear == profitYear).ToDictionary(pp => pp.Demographic!.Ssn, pp => pp);

            foreach (var earningRecord in records.Where(r => r.Code == /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings.Id && r.IsEmployee))
            {
                PayProfit ppNow = ssnByPayProfitNow[earningRecord.Ssn];
                ppNow.Etva += earningRecord.EarningAmount; // we add "100% interest amount" to the ETVA

                PayProfit ppProfitYear = ssnByPayProfitYearEnd[earningRecord.Ssn];
                ppProfitYear.Etva = ppNow.Etva; // At this moment, both "Now" and the "Profit Year" ETVAs are identical.

                etvasUpdated++;
            }

            var employeesEffected = records.Where(m => m.IsEmployee).Select(m => m.Ssn).ToHashSet().Count;
            var beneficiariesEffected = records.Where(m => !m.IsEmployee).Select(m => m.Ssn).ToHashSet().Count;

            ctx.YearEndUpdateStatuses.Add(new YearEndUpdateStatus
            {
                ProfitYear = profitShareUpdateRequest.ProfitYear,
                UpdatedTime = DateTime.Now,
                UpdatedBy = _appUser.UserName ?? "Unknown",
                BeneficiariesEffected = beneficiariesEffected,
                EmployeesEffected = employeesEffected,
                EtvasEffected = etvasUpdated,
                ContributionPercent = profitShareUpdateRequest.ContributionPercent,
                IncomingForfeitPercent = profitShareUpdateRequest.IncomingForfeitPercent,
                EarningsPercent = profitShareUpdateRequest.EarningsPercent,
                SecondaryEarningsPercent = profitShareUpdateRequest.SecondaryEarningsPercent,
                MaxAllowedContributions = profitShareUpdateRequest.MaxAllowedContributions,
                BadgeAdjusted = profitShareUpdateRequest.BadgeToAdjust,
                BadgeAdjusted2 = profitShareUpdateRequest.BadgeToAdjust2,
                AdjustContributionAmount = profitShareUpdateRequest.AdjustContributionAmount,
                AdjustEarningsAmount = profitShareUpdateRequest.AdjustEarningsAmount,
                AdjustIncomingForfeitAmount = profitShareUpdateRequest.AdjustIncomingForfeitAmount,
                AdjustEarningsSecondaryAmount = profitShareUpdateRequest.AdjustEarningsSecondaryAmount
            });

            await ctx.SaveChangesAsync(cancellationToken);

            return new ProfitMasterUpdateResponse
            {
                BeneficiariesEffected = beneficiariesEffected,
                EmployeesEffected = employeesEffected,
                EtvasEffected = etvasUpdated,
                UpdatedTime = DateTime.Now,
                UpdatedBy = _appUser.UserName ?? "Unknown",
                ContributionPercent = profitShareUpdateRequest.ContributionPercent,
                IncomingForfeitPercent = profitShareUpdateRequest.IncomingForfeitPercent,
                EarningsPercent = profitShareUpdateRequest.EarningsPercent,
                SecondaryEarningsPercent = profitShareUpdateRequest.SecondaryEarningsPercent,
                MaxAllowedContributions = profitShareUpdateRequest.MaxAllowedContributions,
                BadgeAdjusted = profitShareUpdateRequest.BadgeToAdjust,
                BadgeAdjusted2 = profitShareUpdateRequest.BadgeToAdjust2,
                AdjustContributionAmount = profitShareUpdateRequest.AdjustContributionAmount,
                AdjustEarningsAmount = profitShareUpdateRequest.AdjustEarningsAmount,
                AdjustIncomingForfeitAmount = profitShareUpdateRequest.AdjustIncomingForfeitAmount,
                AdjustEarningsSecondaryAmount = profitShareUpdateRequest.AdjustEarningsSecondaryAmount
            }!;
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
    public async Task<ProfitMasterRevertResponse> Revert(ProfitYearRequest profitYearRequest, CancellationToken cancellationToken)
    {
        return await _dbFactory.UseWritableContext(async ctx =>
        {
            // See the longer explaination above in the "Update" method for why we lock this parameter
            // TBD: the "profit Year" parameter will likely be removed in the future
            if (profitYearRequest.ProfitYear != DateTime.Now.Year - 1)
            {
                throw new BadHttpRequestException($"The Profit year must be last year. {DateTime.Now.Year - 1}");
            }

            var yearEndUpdateStatus = await ctx.YearEndUpdateStatuses.Where(yeStatus => yeStatus.ProfitYear == profitYearRequest.ProfitYear).FirstOrDefaultAsync(cancellationToken);
            if (yearEndUpdateStatus == null)
            {
                throw new BadHttpRequestException($"Can not Revert. There is no year end update for profit year {profitYearRequest.ProfitYear}");
            }

            // read this year's contribution/vestingEarnings profit_detail rows
            var pds = await ctx.ProfitDetails
                .Where(pd => pd.ProfitYear == profitYearRequest.ProfitYear &&
                             ((pd.ProfitCodeId == /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings.Id &&
                               pd.CommentTypeId == CommentType.Constants.OneHundredPercentEarnings.Id)
                              || pd.ProfitCodeId == /*0*/ ProfitCode.Constants.IncomingContributions.Id))
                .ToListAsync(cancellationToken);

            // --- Adjust ETVA

            // When the revert is run, both the profitYear and profitYear+1 ETVAs need to be updated.
            // profitYear + 1 ---> because it is the hot "ETVA" which gets adjusted on demand, and we need to subtract our adjustment
            // profitYear ---> because it is about to become the new "Last Years" ETVA amount - we no longer have a valid value for last years ETVA.
            // so PayProfit[profitYear].ETVA will be set it to 0.  last years ETVA is in limbo util the YE is completed.

            var nowYear = DateTime.Now.Year;
            var profitYear = profitYearRequest.ProfitYear;

            var memberSsns = pds.Select(p => p.Ssn).ToHashSet();
            var payProfitsBothYears = await ctx.PayProfits.Include(pp => pp.Demographic)
                .Where(pp => (pp.ProfitYear == nowYear || pp.ProfitYear == profitYear) && memberSsns.Contains(pp.Demographic!.Ssn))
                .ToListAsync(cancellationToken);

            var ssnByPayProfitNow = payProfitsBothYears.Where(pp => pp.ProfitYear == nowYear).ToDictionary(pp => pp.Demographic!.Ssn, pp => pp);
            var ssnByPayProfitYearEnd = payProfitsBothYears.Where(pp => pp.ProfitYear == profitYear).ToDictionary(pp => pp.Demographic!.Ssn, pp => pp);

            var etvasReverted = 0;
            foreach (var etvaReccords in pds
                         .Where(pd => pd.ProfitCodeId == /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings.Id && ssnByPayProfitYearEnd.ContainsKey(pd.Ssn))
                         .Select(pd => new { pd, ppNow = ssnByPayProfitNow[pd.Ssn], ppProfitYear = ssnByPayProfitYearEnd[pd.Ssn] }))
            {
                // Here we back out the change to the now ETVA (as we are reverting the transaction.)
                etvaReccords.ppNow.Etva -= etvaReccords.pd.Earnings;
                // We cant unwind this because we stomped on it during the Update.  It will get corrected when YE completes.  At this moment it is in limbo.
                etvaReccords.ppProfitYear.Etva = 0;
                etvasReverted++;
            }

            ctx.ProfitDetails.RemoveRange(pds);
            ctx.YearEndUpdateStatuses.Remove(yearEndUpdateStatus);

            await ctx.SaveChangesAsync(cancellationToken);
            return new ProfitMasterRevertResponse
            {
                BeneficiariesEffected = memberSsns.Count - ssnByPayProfitNow.Count,
                EmployeesEffected = ssnByPayProfitNow.Count,
                EtvasEffected = etvasReverted,
                UdpatedTime = DateTime.Now,
                UpdatedBy = _appUser.UserName ?? "Unknown",
            };
        }, cancellationToken);
    }
}
