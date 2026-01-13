using Demoulas.ProfitSharing.Common.Contracts; // Result, Error, ListResponseDto
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class TaxCodeEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<TaxCodeResponse>>
{
    private readonly ITaxCodeLookupService _taxCodeLookupService;
    private readonly ILogger<TaxCodeEndpoint> _logger;

    public TaxCodeEndpoint(ITaxCodeLookupService taxCodeLookupService, ILogger<TaxCodeEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _taxCodeLookupService = taxCodeLookupService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("tax-codes");
        Summary(s =>
        {
            s.Summary = "Gets all available tax codes";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, ListResponseDto<TaxCodeResponse>.From(new List<TaxCodeResponse>
                {
                    new() { Id = 'A', Name = "A - Married Filing Jointly or Qualifying Widow(er)" },
                    new() { Id = 'B', Name = "B - Single or Married Filing Separately" },
                    new() { Id = 'C', Name = "C - Head of Household" },
                    new() { Id = 'D', Name = "D - Married Filing Jointly or Qualifying Widow(er) with Two Incomes" },
                    new() { Id = 'E', Name = "E - Single or Married Filing Separately with One Income" },
                    new() { Id = 'F', Name = "F - Head of Household with One Income" },
                    new() { Id = 'G', Name = "G - Married Filing Jointly or Qualifying Widow(er) with Three or More Incomes" },
                    new() { Id = 'H', Name = "H - Single or Married Filing Separately with Two or More Incomes" },
                    new() { Id = 'I', Name = "I - Head of Household with Two or More Incomes" },
                    new() { Id = 'J', Name = "J - Married Filing Jointly or Qualifying Widow(er) with Four or More Incomes" },
                    new() { Id = 'K', Name = "K - Single or Married Filing Separately with Three or More Incomes" },
                    new() { Id = 'L', Name = "L - Head of Household with Three or More Incomes" },
                    new() { Id = 'M', Name = "M - Exempt" }
                })
            } };
        });
        Group<LookupGroup>();
        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<TaxCodeResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var dto = await _taxCodeLookupService.GetTaxCodesAsync(ct);

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "tax-code-lookup"),
                new("endpoint", "TaxCodeEndpoint"));

            EndpointTelemetry.RecordCountsProcessed.Record(dto.Items.Count,
                new("record_type", "tax-codes"),
                new("endpoint", "TaxCodeEndpoint"));

            _logger.LogInformation("Tax code lookup completed, returned {TaxCodeCount} tax codes (correlation: {CorrelationId})",
                dto.Items.Count, HttpContext.TraceIdentifier);

            var result = Result<ListResponseDto<TaxCodeResponse>>.Success(dto);
            var httpResult = result.ToHttpResult();

            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            return Result<ListResponseDto<TaxCodeResponse>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
