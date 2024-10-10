using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary.TerminatedEmployeeAndBeneficiaryDataEndpoint;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;


public class TerminatedEmployeeAndBeneficiaryDataEndpoint
    : EndpointWithCsvTotalsBase<TerminatedEmployeeAndBeneficiaryDataRequest,
        ReportResponseBase<TerminatedEmployeeAndBeneficiaryDataResponseDto>,
        TerminatedEmployeeAndBeneficiaryDataResponseDto,
        TerminatedEmployeeAndBeneficiaryDataResponseMap>
{
    private readonly ITerminatedEmployeeAndBeneficiaryReportService _terminatedEmployeeAndBeneficiaryReportService;

    public TerminatedEmployeeAndBeneficiaryDataEndpoint(
        ITerminatedEmployeeAndBeneficiaryReportService terminatedEmployeeAndBeneficiaryReportService)
    {
        ReportFileName = "TerminatedEmployeeAndBeneficiaryReport.csv";
        _terminatedEmployeeAndBeneficiaryReportService = terminatedEmployeeAndBeneficiaryReportService;
    }

    public override void Configure()
    {
        Get("/terminated-employee-and-beneficiary");
        Summary(s =>
        {
            s.Summary = "Provide the Terminated Employee and Beneficiary Report (QPAY066) report.";
            s.Description =
                "Reports on beneficiaries with a non-zero balance and employees who were terminated (and not retired) in the specified date range.";
            s.ExampleRequest = new TerminatedEmployeeAndBeneficiaryDataRequest()
            {
                StartDate = new DateOnly(2023, 1, 7),
                EndDate = new DateOnly(2024, 1, 2),
                ProfitYear = 2023
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new TerminatedEmployeeAndBeneficiaryResponse
                    {
                        ReportName= "Terminated Employee and Beneficiary Report",
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
        base.Configure();
    }

    public override async Task<ReportResponseBase<TerminatedEmployeeAndBeneficiaryDataResponseDto>> GetResponse(
        TerminatedEmployeeAndBeneficiaryDataRequest req, CancellationToken ct)
    {
        TerminatedEmployeeAndBeneficiaryResponse report =
            await _terminatedEmployeeAndBeneficiaryReportService.GetReport(req, ct);
        return report;
    }

    public override string ReportFileName { get; }


    protected internal override Task GenerateCsvContent(CsvWriter csvWriter, ReportResponseBase<TerminatedEmployeeAndBeneficiaryDataResponseDto> report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<TerminatedEmployeeAndBeneficiaryDataResponseMap>();

        TerminatedEmployeeAndBeneficiaryResponse responseWithTotals = (report as TerminatedEmployeeAndBeneficiaryResponse)!;

        // Write out totals
        csvWriter.NextRecord();
        csvWriter.WriteField("Amount In Profit Sharing");
        csvWriter.WriteField(responseWithTotals.TotalEndingBalance);
        csvWriter.WriteField("Vested Amount");
        csvWriter.WriteField(responseWithTotals.TotalVested);
        csvWriter.WriteField("Total Forfeitures");
        csvWriter.WriteField(responseWithTotals.TotalForfeit);
        csvWriter.WriteField("Total Beneficiary Alloctions");
        csvWriter.WriteField(responseWithTotals.TotalBeneficiaryAllocation);

        csvWriter.NextRecord();

        // Move to the next record to separate the headers from the data
        csvWriter.NextRecord();

        // Write the headers using the registered class map
        csvWriter.WriteHeader<TerminatedEmployeeAndBeneficiaryDataResponseDto>();

        csvWriter.NextRecord();

        return base.GenerateCsvContent(csvWriter, report, cancellationToken);
    }



    public sealed class
        TerminatedEmployeeAndBeneficiaryDataResponseMap : ClassMap<TerminatedEmployeeAndBeneficiaryDataResponseDto>
    {
        public TerminatedEmployeeAndBeneficiaryDataResponseMap()
        {
            Map(m => m.BadgePSn).Index(0).Name("BADGE_PSN");
            Map(m => m.Name).Index(1).Name("NAME");
            Map(m => m.BeginningBalance).Index(2).Name("BEGINNING_BALANCE");
            Map(m => m.BeneficiaryAllocation).Index(3).Name("BENEFICIARY_ALLOCATION");
            Map(m => m.DistributionAmount).Index(4).Name("DISTRIBUTION_AMOUNT");
            Map(m => m.Forfeit).Index(5).Name("FORFEIT");
            Map(m => m.EndingBalance).Index(6).Name("ENDING_BALANCE");
            Map(m => m.VestedBalance).Index(7).Name("VESTED_BALANCE");
            Map(m => m.DateTerm).Index(8).Name("DATE_TERM");
            Map(m => m.YtdPsHours).Index(9).Name("YTD_PS_HOURS");
            Map(m => m.VestedPercent).Index(10).Name("VESTED_PERCENT");
            Map(m => m.Age).Index(11).Name("AGE");
            Map(m => m.EnrollmentCode).Index(12).Name("ENROLLMENT_CODE");
        }

    }
}
