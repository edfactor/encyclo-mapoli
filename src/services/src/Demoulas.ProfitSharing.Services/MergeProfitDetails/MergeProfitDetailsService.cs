using Demoulas.ProfitSharing.Common.Contracts;
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

    public async Task<Result<bool>> MergeProfitDetailsToDemographic(int sourceSsn, int destinationSsn, CancellationToken cancellationToken = default)
    {
        // Early validation: prevent merging with same SSN
        if (sourceSsn == destinationSsn)
        {
            return Result<bool>.Failure(Error.SameDemographicMerge);
        }

        try
        {
            return await _dataContextFactory.UseWritableContext(async ctx =>
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
                HashSet<int> ssns = new() { sourceSsn, destinationSsn };

                List<Demographic> source = await demographicQuery
                     .Where(d => ssns.Contains(d.Ssn))
                     .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                // Confirm we have both source and destination
                Demographic? sourceDemographic = source.FirstOrDefault(d => d.Ssn == sourceSsn);
                Demographic? destinationDemographic = source.FirstOrDefault(e => e.Ssn == destinationSsn);

                // Validate demographics existence with specific error messages
                if (sourceDemographic == null && destinationDemographic == null)
                {
                    return Result<bool>.Failure(Error.BothDemographicsNotFound);
                }
                if (sourceDemographic == null)
                {
                    return Result<bool>.Failure(Error.SourceDemographicNotFound);
                }
                if (destinationDemographic == null)
                {
                    return Result<bool>.Failure(Error.DestinationDemographicNotFound);
                }

                // Perform the merge operation
                var rowsAffected = await ctx.ProfitDetails
                    .Where(p => p.Ssn == sourceSsn)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(p => p.Ssn, destinationSsn),
                        cancellationToken);

                return Result<bool>.Success(true);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(Error.MergeOperationFailed(ex.Message));
        }
    }
}
