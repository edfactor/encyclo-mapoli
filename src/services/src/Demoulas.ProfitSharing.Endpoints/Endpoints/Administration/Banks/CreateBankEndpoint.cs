using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Validators.Administration;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class CreateBankEndpoint : ProfitSharingEndpoint<CreateBankRequest, Results<Created<BankDto>, BadRequest, ProblemHttpResult>>
{
    private readonly IBankService _bankService;
    private readonly ILogger<CreateBankEndpoint> _logger;

    public CreateBankEndpoint(IBankService bankService, ILogger<CreateBankEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("banks");
        Validator<CreateBankRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Creates a new bank.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Created<BankDto>, BadRequest, ProblemHttpResult>> ExecuteAsync(CreateBankRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _bankService.CreateAsync(req, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-create"),
                    new("endpoint", nameof(CreateBankEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "bank"),
                    new("endpoint", nameof(CreateBankEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.Match<Results<Created<BankDto>, BadRequest, ProblemHttpResult>>(
                success => TypedResults.Created($"/api/administration/banks/{success.Id}", success),
                error => error.Status == StatusCodes.Status400BadRequest
                    ? TypedResults.BadRequest()
                    : TypedResults.Problem(error.Detail)
            );
        });
    }
}
