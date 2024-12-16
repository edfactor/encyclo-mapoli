using System.Diagnostics;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary.TerminatedEmployeeAndBeneficiaryDataEndpoint;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;


public class TerminatedEmployeeAndBeneficiaryDataEndpoint
    : EndpointWithCsvTotalsBase<ProfitYearRequest,
        TerminatedEmployeeAndBeneficiaryResponse,
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
            s.ExampleRequest = new ProfitYearRequest()
            {
                ProfitYear = 2023
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new TerminatedEmployeeAndBeneficiaryResponse
                    {
                        ReportName = "Terminated Employee and Beneficiary Report",
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
                                TerminatedEmployeeAndBeneficiaryDataResponseDto.ResponseExample()
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

    public override Task<TerminatedEmployeeAndBeneficiaryResponse> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _terminatedEmployeeAndBeneficiaryReportService.GetReportAsync(req, ct);
    }

    public override string ReportFileName { get; }


    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, TerminatedEmployeeAndBeneficiaryResponse responseWithTotals,
        CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<TerminatedEmployeeAndBeneficiaryDataResponseMap>();

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Amount In Profit Sharing");
        csvWriter.WriteField(responseWithTotals.TotalEndingBalance);
        csvWriter.WriteField("Vested Amount");
        csvWriter.WriteField(responseWithTotals.TotalVested);
        csvWriter.WriteField("Total Forfeitures");
        csvWriter.WriteField(responseWithTotals.TotalForfeit);
        csvWriter.WriteField("Total Beneficiary Allocations");
        csvWriter.WriteField(responseWithTotals.TotalBeneficiaryAllocation);

        await csvWriter.NextRecordAsync();

        // Move to the next record to separate the headers from the data
        await csvWriter.NextRecordAsync();

        // Write the headers using the registered class map
        csvWriter.WriteHeader<TerminatedEmployeeAndBeneficiaryDataResponseDto>();

        await csvWriter.NextRecordAsync();

        await base.GenerateCsvContent(csvWriter, responseWithTotals, cancellationToken);
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
