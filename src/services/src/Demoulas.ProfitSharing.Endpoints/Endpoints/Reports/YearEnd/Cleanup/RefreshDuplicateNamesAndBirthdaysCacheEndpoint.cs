using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class RefreshDuplicateNamesAndBirthdaysCacheEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<string>, ProblemHttpResult>>
{
    private readonly IDuplicateNamesAndBirthdaysService _duplicateNamesService;
    private readonly ILogger<RefreshDuplicateNamesAndBirthdaysCacheEndpoint> _logger;

    public RefreshDuplicateNamesAndBirthdaysCacheEndpoint(
        IDuplicateNamesAndBirthdaysService duplicateNamesService,
        ILogger<RefreshDuplicateNamesAndBirthdaysCacheEndpoint> logger)
        : base(Navigation.Constants.DuplicateNamesAndBirthdays)
    {
        _duplicateNamesService = duplicateNamesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("duplicate-names-and-birthdays/refresh-cache");
        Summary(s =>
        {
            s.Summary = "Force a refresh of the duplicate names and birthdays cache";
            s.Description = "Triggers an immediate cache refresh for duplicate names and birthdays data. " +
                          "The cache is automatically refreshed every 6 hours, but this endpoint allows manual refresh when needed.";
            s.ResponseExamples[200] = "Cache refresh completed successfully";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.ITOPERATIONS}";
        });
        Group<YearEndGroup>();
    }

    protected override async Task<Results<Ok<string>, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            _logger.LogInformation("Cache refresh request received for duplicate names and birthdays (correlation: {CorrelationId})",
                HttpContext.TraceIdentifier);

            await _duplicateNamesService.ForceRefreshCacheAsync(ct);

            var response = "Cache refresh completed successfully. The updated data is now available.";

            this.RecordResponseMetrics(HttpContext, _logger, response);

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);

            return TypedResults.Problem(
                title: "Cache Refresh Failed",
                detail: ex.Message,
                statusCode: 500);
        }
    }
}
