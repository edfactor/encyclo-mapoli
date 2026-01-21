using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Services.Distributions.MergeProfitDetails;

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
            return await _dataContextFactory.UseWritableContext((Func<Data.Contexts.ProfitSharingDbContext, Task<Result<bool>>>)(async ctx =>
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(ctx, false);
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

                // Get distinct years from source profit details that have years of service credit > 0
                var sourceYearsWithCredit = await ctx.ProfitDetails
                    .Where(p => p.Ssn == sourceSsn && p.YearsOfServiceCredit > 0)
                    .Select(p => p.ProfitYear)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                // Find which years the destination already has service credit for
                var destinationYearsWithCredit = await ctx.ProfitDetails
                    .Where(p => p.Ssn == destinationSsn && p.YearsOfServiceCredit > 0)
                    .Select(p => p.ProfitYear)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                // Find overlapping years where both source and destination have service credit
                var overlappingYears = sourceYearsWithCredit.Intersect(destinationYearsWithCredit).ToHashSet();

                // Perform the merge operation for ProfitDetails
                // If the year overlaps (destination already has credit), set YearsOfServiceCredit to 0
                // Otherwise, keep the source's YearsOfServiceCredit value
                var profitDetailsRowsAffected = await ctx.ProfitDetails
                    .Where(p => p.Ssn == sourceSsn)
                    .ExecuteUpdateAsync(
                        s => s
                            .SetProperty(p => p.Ssn, destinationSsn)
                            .SetProperty(p => p.YearsOfServiceCredit,
                                p => overlappingYears.Contains(p.ProfitYear) ? (sbyte)0 : p.YearsOfServiceCredit)
                            .SetProperty(p => p.ModifiedAtUtc, DateTimeOffset.UtcNow),
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
