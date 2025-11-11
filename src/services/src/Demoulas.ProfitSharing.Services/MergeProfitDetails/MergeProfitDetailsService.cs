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

    public async Task<Result<bool>> MergeProfitDetailsToDemographic(int sourceDemographic, int targetDemographic, CancellationToken cancellationToken = default)
    {
        // Early validation: prevent merging with same SSN
        if (sourceDemographic == targetDemographic)
        {
            return Result<bool>.Failure(Error.SameDemographicMerge);
        }

        try
        {
            return await _dataContextFactory.UseWritableContext((Func<Data.Contexts.ProfitSharingDbContext, Task<Result<bool>>>)(async ctx =>
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
                HashSet<int> ssns = new() { sourceDemographic, targetDemographic };

                List<Demographic> source = await demographicQuery
                     .Where(d => ssns.Contains(d.Ssn))
                     .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                // Confirm we have both source and destination
                Demographic? sourceDemographic = source.FirstOrDefault((Func<Demographic, bool>)(d => d.Ssn == sourceDemographic));
                Demographic? destinationDemographic = source.FirstOrDefault(e => e.Ssn == targetDemographic);

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
                    .Where((System.Linq.Expressions.Expression<Func<ProfitDetail, bool>>)(p => p.Ssn == sourceDemographic))
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(p => p.Ssn, targetDemographic),
                        cancellationToken);

                return Result<bool>.Success(true);
            }), cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(Error.MergeOperationFailed(ex.Message));
        }
    }
}
