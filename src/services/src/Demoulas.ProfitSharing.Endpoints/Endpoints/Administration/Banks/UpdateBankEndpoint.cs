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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class UpdateBankEndpoint : ProfitSharingEndpoint<UpdateBankRequest, Results<Ok<BankDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBankService _bankService;
    private readonly ILogger<UpdateBankEndpoint> _logger;

    public UpdateBankEndpoint(IBankService bankService, ILogger<UpdateBankEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("banks");
        Validator<UpdateBankRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Updates an existing bank.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<BankDto>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UpdateBankRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _bankService.UpdateAsync(req, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-update"),
                    new("endpoint", nameof(UpdateBankEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "bank"),
                    new("endpoint", nameof(UpdateBankEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResultWithValidation(Error.BankNotFound);
        });
    }
}
