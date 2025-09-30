using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.MergeProfitDetails;
public class MergeProfitDetailsService : IMergeProfitDetailsService
{

    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public MergeProfitDetailsService(IProfitSharingDataContextFactory dataContextFactory, IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
    }

    public Task MergeProfitDetailsToDemographic(int sourceSsn, int destinationSsn, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            try
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
                HashSet<int> ssns = new() { sourceSsn, destinationSsn };

                List<Demographic> source = await demographicQuery
                     .Where(d => ssns.Contains(d.Ssn))
                     .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                // confirm we have both source and destination
                Demographic? sourceDemographic = source.FirstOrDefault(d => d.Ssn == sourceSsn);
                Demographic? destinationDemographic = source.FirstOrDefault(e => e.Ssn == destinationSsn);

                if (sourceDemographic == null || destinationDemographic == null)
                {
                    throw new KeyNotFoundException($"Could not find both source SSN {sourceSsn.MaskSsn()} and destination SSN {destinationSsn.MaskSsn()} in demographics.");
                }

                await ctx.ProfitDetails
                    .Where(p => p.Ssn == destinationSsn)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(p => p.Ssn, sourceSsn),
                        cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error merging profit details from SSN {sourceSsn.MaskSsn()} to SSN {destinationSsn.MaskSsn()}: {ex.Message}", ex);
            }


        }, cancellationToken);
    }
}
