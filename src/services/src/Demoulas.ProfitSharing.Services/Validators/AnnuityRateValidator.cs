using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Validators;

/// <summary>
/// Validates annuity rate data completeness for profit sharing year-end processing.
/// Checks that all required ages have rates defined for a given year.
/// </summary>
public sealed class AnnuityRateValidator : IAnnuityRateValidator
{
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public AnnuityRateValidator(IProfitSharingDataContextFactory contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    /// <inheritdoc />
    public Task<Result<bool>> ValidateYearCompletenessAsync(short year, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get configuration for the year
            var config = await ctx.AnnuityRateConfigs
                .FirstOrDefaultAsync(c => c.Year == year, cancellationToken);

            if (config is null)
            {
                return Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
                {
                    ["Year"] = [$"Annuity rate configuration not found for year {year}. Please configure age ranges before generating certificates."]
                }));
            }

            // Get all ages that have rates defined for this year
            var existingAges = await ctx.AnnuityRates
                .Where(r => r.Year == year)
                .Select(r => r.Age)
                .ToListAsync(cancellationToken);

            // Generate expected age range
            var expectedAges = Enumerable.Range(config.MinimumAge, config.MaximumAge - config.MinimumAge + 1)
                .Select(age => (byte)age)
                .ToHashSet();

            // Find missing ages
            var missingAges = expectedAges.Except(existingAges).OrderBy(age => age).ToList();

            if (missingAges.Count > 0)
            {
                var missingRanges = FormatMissingAgeRanges(missingAges);
                return Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
                {
                    ["AnnuityRates"] = [$"Annuity rates incomplete for year {year}. Missing ages: {missingRanges}. Please populate all required rates before generating certificates."]
                }));
            }

            // Check for gaps (ages exist but not in sequence)
            var sortedExistingAges = existingAges.OrderBy(a => a).ToList();
            var gaps = new List<byte>();

            for (int i = 0; i < sortedExistingAges.Count - 1; i++)
            {
                var currentAge = sortedExistingAges[i];
                var nextAge = sortedExistingAges[i + 1];

                if (nextAge - currentAge > 1)
                {
                    // Gap detected
                    for (byte gapAge = (byte)(currentAge + 1); gapAge < nextAge; gapAge++)
                    {
                        if (expectedAges.Contains(gapAge))
                        {
                            gaps.Add(gapAge);
                        }
                    }
                }
            }

            if (gaps.Count > 0)
            {
                var gapRanges = FormatMissingAgeRanges(gaps);
                return Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
                {
                    ["AnnuityRates"] = [$"Annuity rates have gaps for year {year}. Missing ages: {gapRanges}. Please fill gaps before generating certificates."]
                }));
            }

            return Result<bool>.Success(true);
        }, cancellationToken);
    }

    /// <inheritdoc />
    public Task<byte?> GetMinimumAgeForYearAsync(short year, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.AnnuityRateConfigs
                .Where(c => c.Year == year)
                .Select(c => (byte?)c.MinimumAge)
                .FirstOrDefaultAsync(cancellationToken);
        }, cancellationToken);
    }

    /// <inheritdoc />
    public Task<byte?> GetMaximumAgeForYearAsync(short year, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.AnnuityRateConfigs
                .Where(c => c.Year == year)
                .Select(c => (byte?)c.MaximumAge)
                .FirstOrDefaultAsync(cancellationToken);
        }, cancellationToken);
    }

    /// <summary>
    /// Formats a list of missing ages into readable ranges.
    /// Example: [67, 68, 69, 85, 86, 100] => "67-69, 85-86, 100"
    /// </summary>
    private static string FormatMissingAgeRanges(List<byte> missingAges)
    {
        if (missingAges.Count == 0)
        {
            return string.Empty;
        }

        var ranges = new List<string>();
        var rangeStart = missingAges[0];
        var rangeEnd = missingAges[0];

        for (int i = 1; i < missingAges.Count; i++)
        {
            if (missingAges[i] == rangeEnd + 1)
            {
                // Continue current range
                rangeEnd = missingAges[i];
            }
            else
            {
                // End current range and start new one
                ranges.Add(rangeStart == rangeEnd ? $"{rangeStart}" : $"{rangeStart}-{rangeEnd}");
                rangeStart = missingAges[i];
                rangeEnd = missingAges[i];
            }
        }

        // Add final range
        ranges.Add(rangeStart == rangeEnd ? $"{rangeStart}" : $"{rangeStart}-{rangeEnd}");

        return string.Join(", ", ranges);
    }
}
