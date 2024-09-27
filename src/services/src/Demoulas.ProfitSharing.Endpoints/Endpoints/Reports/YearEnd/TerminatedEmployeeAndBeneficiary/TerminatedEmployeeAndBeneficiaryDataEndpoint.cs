using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;


public class TerminatedEmployeeAndBeneficiaryDataEndpoint
    : Endpoint<TerminatedEmployeeAndBeneficiaryDataRequest, TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>>
{
    private readonly ITerminatedEmployeeAndBeneficiaryReportService _terminatedEmployeeAndBeneficiaryReportService;

    public TerminatedEmployeeAndBeneficiaryDataEndpoint(ITerminatedEmployeeAndBeneficiaryReportService terminatedEmployeeAndBeneficiaryReportService)
    {
        _terminatedEmployeeAndBeneficiaryReportService = terminatedEmployeeAndBeneficiaryReportService;
    }

    public override void Configure()
    {
        Get("/terminated-employee-and-beneficiary-data/");
        Summary(s =>
        {
            s.Summary = "Provide the Terminated Employee and Beneficiary Report (QPAY066) report.";
            s.Description =
                "Reports on beneficiaries with a non-zero balance and employees who were terminated (and not retired) in the specified date range.";
            s.ExampleRequest = new TerminatedEmployeeAndBeneficiaryDataRequest()
            {
                StartDate = new DateOnly(2023, 1, 7),
                EndDate = new DateOnly(2024, 1, 2),
                ProfitShareYear = ReferenceData.AutoSelectYear
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>
                    {
                        ReportDate = default,
                        TotalVested = 1000,
                        TotalForfeit = 2000,
                        TotalEndingBalance = 3000,
                        TotalBeneficiaryAllocation = 4000,
                        Response = new PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>
                        {
                            Total = 1,
                            Results = new List<TerminatedEmployeeAndBeneficiaryDataResponseDto>
                            {
                                TerminatedEmployeeAndBeneficiaryDataResponseDto.Example
                            }
                        }
                    }
                }
            };


            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

    }

    public override async Task HandleAsync(TerminatedEmployeeAndBeneficiaryDataRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto> report = await _terminatedEmployeeAndBeneficiaryReportService.GetReport(req, ct);
        await SendAsync(report, 200, ct);
    }

}
