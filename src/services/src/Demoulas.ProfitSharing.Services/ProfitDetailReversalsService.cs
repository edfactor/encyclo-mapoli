using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
public sealed class ProfitDetailReversalsService: IProfitDetailReversalsService
{
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public ProfitDetailReversalsService(IProfitSharingDataContextFactory dbContextFactory, IDemographicReaderService demographicReaderService)
    {
        _dbContextFactory = dbContextFactory;
        _demographicReaderService = demographicReaderService;
    }

    public async Task ReverseProfitDetailsAsync(int[] profitDetailIds, CancellationToken cancellationToken)
    {
        await _dbContextFactory.UseWritableContext(async (ctx) =>
        {
            var profitDetails = await ctx.ProfitDetails
                .Where(pd => profitDetailIds.Contains(pd.Id))
                .ToListAsync(cancellationToken);

            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var employeeSsns = await demographicQuery
                .Where(d => profitDetails.Select(pd => pd.Ssn).Contains(d.Ssn))
                .Select(d => d.Ssn)
                .ToListAsync(cancellationToken);
            
            var currentDate = DateTime.Now;
            
            
            foreach (var pd in profitDetails)
            {
                if (pd.ProfitCodeId is 0 or 2 or 8) {
                    continue;
                }

                var reverseProfitDetail = new Data.Entities.ProfitDetail
                {
                    Ssn = pd.Ssn,
                    ProfitYear = pd.ProfitYear,
                    ProfitYearIteration = pd.ProfitYearIteration,
                    DistributionSequence = pd.DistributionSequence,
                    ProfitCodeId = pd.ProfitCodeId,
                    Contribution = pd.ProfitCodeId == 6 ? -pd.Contribution : 0,
                    Forfeiture = pd.ProfitCodeId is 1 or 3 or 5 or 9 ? -pd.Forfeiture : 0,
                    Earnings = 0,
                    FederalTaxes = -pd.FederalTaxes,
                    StateTaxes = -pd.StateTaxes,
                    MonthToDate = (byte)currentDate.Month,
                    YearToDate = (short)currentDate.Year,
                    YearsOfServiceCredit = (sbyte)-pd.YearsOfServiceCredit,
                    TaxCodeId = pd.TaxCodeId,
                    CommentTypeId = pd.Remark?.StartsWith("REV") == true ? CommentType.Constants.Unrev : CommentType.Constants.Rev,
                    Remark = pd.Remark?.StartsWith("REV") == true 
                        ? $"UN-REV {currentDate:MM/yy}" 
                        : $"REV {currentDate:MM/yy}",
                    
                    ZeroContributionReasonId = null
                };

                ctx.ProfitDetails.Add(reverseProfitDetail);

                // Handle ETVA updates for profit codes 6 and 9
                var employeeMember = employeeSsns.FirstOrDefault(x => x == pd.Ssn);  
                if (pd.ProfitCodeId is 6 or 9 && employeeMember != default)
                {
                    var currentPayProfit = await ctx.PayProfits
                        .FirstOrDefaultAsync(pp=>pp.DemographicId == employeeMember, cancellationToken);
                    if ( currentPayProfit != null)
                    {
                        currentPayProfit.Etva += reverseProfitDetail.Contribution + reverseProfitDetail.Forfeiture;
                    }
                }
            }

            await ctx.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
