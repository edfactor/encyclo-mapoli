using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

public class AdhocBeneficiariesReportEndpoint : EndpointWithCsvTotalsBase<AdhocBeneficiariesReportRequest,
    AdhocBeneficiariesReportResponse,
    BeneficiaryReportDto,
    AdhocBeneficiariesReportEndpoint.AdhocBeneficiariesReportResponseMap>
{
    private readonly IAdhocBeneficiariesReport _reportService;

    public AdhocBeneficiariesReportEndpoint(IAdhocBeneficiariesReport reportService)
        : base(Navigation.Constants.QPAY066AdHocReports)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("adhoc-beneficiaries-report");
        Summary(s =>
        {
            s.Summary = "Adhoc Beneficiaries Report";
            s.Description = "Returns a report of employee and non-employee beneficiaries, with optional detail lines.";
            s.ExampleRequest = new AdhocBeneficiariesReportRequest(true);
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new AdhocBeneficiariesReportResponse
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.MinValue,
                        EndDate = DateOnly.MaxValue,
                        TotalEndingBalance = 10000,
                        Response = new PaginatedResponseDto<BeneficiaryReportDto>
                        {
                            Results = new List<BeneficiaryReportDto>()
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "AdhocBeneficiariesReport";

    public override Task<AdhocBeneficiariesReportResponse> GetResponse(AdhocBeneficiariesReportRequest req, CancellationToken ct)
    {
        return _reportService.GetAdhocBeneficiariesReportAsync(req, ct);
    }

   

    public sealed class AdhocBeneficiariesReportResponseMap : ClassMap<BeneficiaryReportDto>
    {
        public AdhocBeneficiariesReportResponseMap()
        {
            Map(m => m.BeneficiaryId).Name("Beneficiary Id");
            Map(m => m.FullName).Name("Full Name");
            Map(m => m.Ssn).Name("SSN");
            Map(m => m.Relationship).Name("Relationship");
            Map(m => m.Balance).Name("Balance");
            Map(m => m.BadgeNumber).Name("Badge Number");
        }
    }
}
