using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;


sealed internal class TerminatedEmployeeAndBeneficiaryReportEndpoint(
    ITerminatedEmployeeAndBeneficiaryReportService terminatedEmployeeAndBeneficiaryReportService)
    : Endpoint<TerminatedEmployeeAndBeneficiaryReportRequestDto>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/terminatedEmployeeAndBeneficiaryReport");
        AllowAnonymous(); // for now.
        Summary(s =>
        {
            s.Summary = "Provide the Terminated Employee and Beneficiary Report (QPAY066) report.";
            s.Description =
                @"Reports on beneficiaries with a non-zero balance and employees who were terminated (and not retired) in the specified date range.";
        });
        Group<YearEndGroup>();
        Description(d => d.Produces<string>(200, "text/plain"));
    }

    public override async Task HandleAsync(TerminatedEmployeeAndBeneficiaryReportRequestDto req, CancellationToken ct)
    {
        string report = await terminatedEmployeeAndBeneficiaryReportService.GetReport(req.startDate, req.endDate, req.profitShareYear, ct);
        HttpContext.Response.ContentType = "text/plain";
        await HttpContext.Response.WriteAsync(report, ct);
    }
}
