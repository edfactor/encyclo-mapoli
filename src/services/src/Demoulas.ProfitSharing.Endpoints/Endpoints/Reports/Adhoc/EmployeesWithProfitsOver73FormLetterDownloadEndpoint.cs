using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

/// <summary>
/// Endpoint for downloading form letters for employees over age 73 with profit sharing balances.
/// Returns a text file containing form letters for employees who must take required minimum distributions (RMDs).
/// </summary>
public sealed class EmployeesWithProfitsOver73FormLetterDownloadEndpoint : ProfitSharingEndpoint<EmployeesWithProfitsOver73Request, string>
{
    private readonly IEmployeesWithProfitsOver73Service _employeesWithProfitsOver73Service;

    public EmployeesWithProfitsOver73FormLetterDownloadEndpoint(
        IEmployeesWithProfitsOver73Service employeesWithProfitsOver73Service)
        : base(Navigation.Constants.AdhocProfLetter73)
    {
        _employeesWithProfitsOver73Service = employeesWithProfitsOver73Service;
    }

    public override void Configure()
    {
        Get("prof-letter73/download-form-letter");

        // Add role-based authorization
        Roles(Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.ITDEVOPS);

        Summary(s =>
        {
            s.Summary = "Returns a text file containing form letters for employees over age 73 who must take required minimum distributions";
            s.Description = "Generates form letters (QPROF_OVER73.txt) for employees with profit sharing accounts who have reached age 73 and must comply with IRS required minimum distribution (RMD) rules. " +
                           "Optionally accepts a list of badge numbers to generate letters for specific employees only. If no badge numbers are provided, letters will be generated for all eligible employees.";
            s.ExampleRequest = new EmployeesWithProfitsOver73Request
            {
                ProfitYear = 2023,
                BadgeNumbers = new List<int> { 12345, 67890 } // Optional: specify employees, or omit for all
            };
            s.Responses[200] = "Text file (QPROF_OVER73.txt) containing form letters";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
    }

    protected override async Task<string> HandleRequestAsync(EmployeesWithProfitsOver73Request req, CancellationToken ct)
    {
        var formLetterContent = await _employeesWithProfitsOver73Service.GetEmployeesWithProfitsOver73FormLetterAsync(req, ct);

        // Convert string to stream and send
        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: false))
        {
            await writer.WriteAsync(formLetterContent);
            await writer.FlushAsync(ct);
            memoryStream.Position = 0;

            // Send stream as file download (FastEndpoints handles Content-Disposition)
            await Send.StreamAsync(
                stream: memoryStream,
                fileName: "QPROF_OVER73.txt",
                contentType: "text/plain",
                cancellation: ct);
        }

        return string.Empty;
    }
}
