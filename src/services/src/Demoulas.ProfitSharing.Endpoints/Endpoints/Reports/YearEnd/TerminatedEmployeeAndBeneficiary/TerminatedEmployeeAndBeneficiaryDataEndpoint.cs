using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;


public class TerminatedEmployeeAndBeneficiaryDataEndpoint(
    ITerminatedEmployeeAndBeneficiaryReportService _terminatedEmployeeAndBeneficiaryReportService)
    : Endpoint<TerminatedEmployeeAndBeneficiaryDataRequest, TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>>

{
    public override void Configure()
    {
        Get("/terminated-employee-and-beneficiary-data/");
        Summary(s =>
        {
            s.Summary = "Provide the Terminated Employee and Beneficiary Report (QPAY066) report.";
            s.Description =
                "Reports on beneficiaries with a non-zero balance and employees who were terminated (and not retired) in the specified date range.";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(TerminatedEmployeeAndBeneficiaryDataRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto> report = await _terminatedEmployeeAndBeneficiaryReportService.GetReport(req, ct);
        await SendAsync(report, 200, ct);
    }

}
