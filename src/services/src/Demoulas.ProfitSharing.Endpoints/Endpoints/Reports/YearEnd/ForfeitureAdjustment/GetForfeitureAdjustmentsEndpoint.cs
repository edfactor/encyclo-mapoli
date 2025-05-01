using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Request;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;
public class GetForfeitureAdjustmentsEndpoint : EndpointWithCsvTotalsBase<ForfeitureAdjustmentRequest, 
    ForfeitureAdjustmentReportResponse, 
    ForfeitureAdjustmentReportDetail, 
    GetForfeitureAdjustmentsEndpoint.ForfeitureAdjustmentResponseMap>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;

    public GetForfeitureAdjustmentsEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
    }

    public override void Configure()
    {
        Get("forfeiture-adjustments");
        Summary(s =>
        {
            s.Summary = "Get forfeiture adjustments for a given year and badge number/ssn.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Description = "This endpoint returns a list of forfeiture adjustments for a given year and badge number/ssn.";
            s.ExampleRequest = new ForfeitureAdjustmentRequest() { ProfitYear = 2024, Badge = 1234567890 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ForfeitureAdjustmentReportResponse
                    {
                        ReportName = "Forfeiture Adjustments",
                        ReportDate = DateTimeOffset.Now,
                        TotatNetBalance = 1000,
                        TotatNetVested = 1000,
                        Response = new PaginatedResponseDto<ForfeitureAdjustmentReportDetail>()
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "ForfeitureAdjustments";

    public override Task<ForfeitureAdjustmentReportResponse> GetResponse(ForfeitureAdjustmentRequest req, CancellationToken ct)
    {
        return _forfeitureAdjustmentService.GetForfeitureAdjustmentReportAsync(req, ct);
    }

    // Define the map class for CSV generation
    public sealed class ForfeitureAdjustmentResponseMap : ClassMap<ForfeitureAdjustmentReportDetail>
    {
        public ForfeitureAdjustmentResponseMap()
        {
            // Map properties from the detail record for CSV generation
            Map(m => m.ClientNumber).Name("Client Number");
            Map(m => m.BadgeNumber).Name("Badge Number");
            Map(m => m.StartingBalance).Name("Starting Balance");
            Map(m => m.ForfeitureAmount).Name("Forfeiture Amount");
            Map(m => m.NetBalance).Name("Net Balance");
            Map(m => m.NetVested).Name("Net Vested");
        }
    }
}
