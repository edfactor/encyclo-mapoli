using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;
using CsvHelper.Configuration;
using CsvHelper;
using System.Text;
using System.Threading;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;


public class TerminatedEmployeeAndBeneficiaryReportEndpoint(
    ITerminatedEmployeeAndBeneficiaryReportService terminatedEmployeeAndBeneficiaryReportService)
        : Endpoint<TerminatedEmployeeAndBeneficiaryReportRequestDto, string>
{
    public override void Configure()
    {
        Get("/terminated-employee-and-beneficiary-report/");
        Summary(s =>
        {
            s.Summary = "Provide the Terminated Employee and Beneficiary Report (QPAY066) report.";
            s.Description =
                "Reports on beneficiaries with a non-zero balance and employees who were terminated (and not retired) in the specified date range.";
        });
        Group<YearEndGroup>();
        Description(d => d.Produces<string>(200, "text/plain"));
    }

    public override async Task HandleAsync(TerminatedEmployeeAndBeneficiaryReportRequestDto req, CancellationToken ct)
    {
        string report = await terminatedEmployeeAndBeneficiaryReportService.GetReport(req.StartDate, req.EndDate, req.ProfitShareYear, ct);
        HttpContext.Response.ContentType = "text/plain";
        await HttpContext.Response.WriteAsync(report, ct);
        await Task.CompletedTask;
    }
}
