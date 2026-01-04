using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

/// <summary>
/// Endpoint to check which years have complete annuity rate data.
/// Returns completeness status for a range of years (default: current year and previous 5 years).
/// </summary>
public sealed class GetMissingAnnuityYearsEndpoint : ProfitSharingEndpoint<GetMissingAnnuityYearsRequest, Results<Ok<MissingAnnuityYearsResponse>, ProblemHttpResult>>
{
    private readonly IAnnuityRateValidator _annuityRateValidator;
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly ILogger<GetMissingAnnuityYearsEndpoint> _logger;

    public GetMissingAnnuityYearsEndpoint(
        IAnnuityRateValidator annuityRateValidator,
        IProfitSharingDataContextFactory contextFactory,
        ILogger<GetMissingAnnuityYearsEndpoint> logger)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRateValidator = annuityRateValidator;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("annuity-rates/missing-years");
        Summary(s =>
        {
            s.Summary = "Checks which years have complete annuity rate data (all required ages defined).";
            s.Description = "Returns completeness status for a range of years. Defaults to current year and previous 5 years if no range specified.";
            s.ExampleRequest = GetMissingAnnuityYearsRequest.RequestExample();
        });
        Group<AdministrationGroup>();
    }

    public override async Task<Results<Ok<MissingAnnuityYearsResponse>, ProblemHttpResult>> ExecuteAsync(GetMissingAnnuityYearsRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, req);

            // Default to current year and previous 5 years if not specified
            var currentYear = (short)DateTime.Today.Year;
            var startYear = req.StartYear ?? (short)(currentYear - 5);
            var endYear = req.EndYear ?? currentYear;

            // Validate year range
            if (startYear > endYear)
            {
                var error = Error.Validation(new Dictionary<string, string[]>
                {
                    [nameof(req.StartYear)] = ["StartYear must be less than or equal to EndYear."]
                });
                return TypedResults.Problem(
                    detail: error.Description,
                    statusCode: 400,
                    title: "Validation Failed",
                    extensions: new Dictionary<string, object?> { ["errors"] = error.ValidationErrors });
            }

            var yearStatuses = new List<AnnuityYearStatus>();

            for (short year = startYear; year <= endYear; year++)
            {
                // Check if year has any annuity rates at all
                var hasAnyRates = await _contextFactory.UseReadOnlyContext(async ctx =>
                    await ctx.AnnuityRates.AnyAsync(r => r.Year == year, ct), ct);

                if (!hasAnyRates)
                {
                    // Year has no rates - mark as incomplete with all ages missing
                    var config = await _contextFactory.UseReadOnlyContext(async ctx =>
                        await ctx.AnnuityRateConfigs.FirstOrDefaultAsync(c => c.Year == year, ct), ct);

                    var allAges = config != null
                        ? Enumerable.Range(config.MinimumAge, config.MaximumAge - config.MinimumAge + 1)
                            .Select(age => (byte)age)
                            .ToArray()
                        : Enumerable.Range(67, 54).Select(age => (byte)age).ToArray(); // Default 67-120

                    yearStatuses.Add(new AnnuityYearStatus
                    {
                        Year = year,
                        IsComplete = false,
                        MissingAges = allAges
                    });
                    continue;
                }

                // Validate completeness
                var validationResult = await _annuityRateValidator.ValidateYearCompletenessAsync(year, ct);

                if (validationResult.IsSuccess)
                {
                    yearStatuses.Add(new AnnuityYearStatus
                    {
                        Year = year,
                        IsComplete = true,
                        MissingAges = Array.Empty<byte>()
                    });
                }
                else
                {
                    // Extract missing ages from validation error message
                    var missingAges = await GetMissingAgesForYear(year, ct);
                    yearStatuses.Add(new AnnuityYearStatus
                    {
                        Year = year,
                        IsComplete = false,
                        MissingAges = missingAges
                    });
                }
            }

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "check-missing-annuity-years"),
                    new("endpoint", nameof(GetMissingAnnuityYearsEndpoint)));

                var incompleteCount = yearStatuses.Count(y => !y.IsComplete);
                EndpointTelemetry.RecordCountsProcessed?.Record(incompleteCount,
                    new("record_type", "incomplete-years"),
                    new("endpoint", nameof(GetMissingAnnuityYearsEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            var response = new MissingAnnuityYearsResponse
            {
                Years = yearStatuses
            };

            // Record successful response metrics
            this.RecordResponseMetrics(HttpContext, _logger, response, isSuccess: true);

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            // Record exception and error metrics
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

    /// <summary>
    /// Gets the list of missing ages for a given year.
    /// </summary>
    private Task<byte[]> GetMissingAgesForYear(short year, CancellationToken ct)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var config = await ctx.AnnuityRateConfigs
                .FirstOrDefaultAsync(c => c.Year == year, ct);

            if (config == null)
            {
                return Array.Empty<byte>();
            }

            var existingAges = await ctx.AnnuityRates
                .Where(r => r.Year == year)
                .Select(r => r.Age)
                .ToListAsync(ct);

            var expectedAges = Enumerable.Range(config.MinimumAge, config.MaximumAge - config.MinimumAge + 1)
                .Select(age => (byte)age)
                .ToHashSet();

            return expectedAges.Except(existingAges).OrderBy(age => age).ToArray();
        }, ct);
    }
}
