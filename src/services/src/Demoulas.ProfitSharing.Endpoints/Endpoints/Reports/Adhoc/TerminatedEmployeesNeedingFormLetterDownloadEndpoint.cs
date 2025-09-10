using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;
public sealed class TerminatedEmployeesNeedingFormLetterDownloadEndpoint : ProfitSharingEndpoint<TerminatedLettersRequest, string>
{
    private readonly IAdhocTerminatedEmployeesService _adhocTerminatedEmployeesService;

    public TerminatedEmployeesNeedingFormLetterDownloadEndpoint(IAdhocTerminatedEmployeesService adhocTerminatedEmployeesService) : base(Navigation.Constants.Unknown) //TBD
    {
        _adhocTerminatedEmployeesService = adhocTerminatedEmployeesService;
    }

    public override void Configure()
    {
        Get("adhoc-terminated-employees-report-needing-letter/download");
        Summary(s =>
        {
            s.Summary = "Returns a text file containing a form letter to be sent to terminated employees who aren't fully vested";
            s.ExampleRequest = TerminatedLettersRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(TerminatedLettersRequest req, CancellationToken ct)
    {
        var response = await _adhocTerminatedEmployeesService.GetFormLetterForTerminatedEmployees(req, ct);
        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream))
        {

            await writer.WriteAsync(response);

            await writer.FlushAsync(ct);

            memoryStream.Position = 0;

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "QPROF003.txt",
                Inline = false
            };
            HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

            await Send.StreamAsync(memoryStream, "QPROF003.txt", contentType: "text/plain", cancellation: ct);
        }
    }
}
