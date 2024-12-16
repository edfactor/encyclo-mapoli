using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;

public class ExecutiveHoursAndDollarsEndpoint :
    EndpointWithCsvBase<ExecutiveHoursAndDollarsRequest, ExecutiveHoursAndDollarsResponse, ExecutiveHoursAndDollarsEndpoint.ExecutiveHoursAndDollarsMap
    >
{
    private readonly IExecutiveHoursAndDollarsService _reportService;

    public  ExecutiveHoursAndDollarsEndpoint(IExecutiveHoursAndDollarsService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("executive-hours-and-dollars");
        Summary(s =>
        {
            s.Summary = "Executive Hours and Dollars Endpoint";
            s.Description =
                "The Executive Hours and Dollars Endpoint endpoint produces a list of executives with hours and dollars.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    204, "Operation completed successfully."
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "Executive Hours and Dollars";

    public override Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetResponse(ExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        return  _reportService.GetExecutiveHoursAndDollarsReportAsync(req, ct);
    }

    public sealed class ExecutiveHoursAndDollarsMap : ClassMap<ExecutiveHoursAndDollarsResponse>
    {
        public ExecutiveHoursAndDollarsMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE");
            Map(m => m.FullName).Index(1).Name("NAME");
            Map(m => m.StoreNumber).Index(2).Name("STR");
            Map(m => m.HoursExecutive).Index(3).Name("EXEC HRS");
            Map(m => m.IncomeExecutive).Index(4).Name("EXEC DOLS");
            Map(m => m.CurrentHoursYear).Index(5).Name("ORA HRS CUR");
            Map(m => m.CurrentIncomeYear).Index(6).Name("ORA DOLS CUR");
            Map(m => m.PayFrequencyId).Index(7).Name("FREQ");
            Map(m => m.EmploymentStatusId).Index(8).Name("STATUS");
        }
    }
}

