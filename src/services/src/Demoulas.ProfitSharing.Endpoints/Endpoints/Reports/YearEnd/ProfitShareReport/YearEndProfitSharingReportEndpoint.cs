using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
public class YearEndProfitSharingReportEndpoint: EndpointWithCsvBase<YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse, YearEndProfitSharingReportEndpoint.YearEndProfitSharingReportClassMap>
{
    private readonly ICleanupReportService _cleanupReportService;

    public YearEndProfitSharingReportEndpoint(ICleanupReportService cleanupReportService)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Get("yearend-profit-sharing-report");
        Summary(s =>
        {
            s.Summary = "Yearend profit sharing report";
            s.Description = "Returns a list of employees who will participate in the profit sharing this year, as well as their qualifying attributes.";
            s.ExampleRequest = new YearEndProfitSharingReportRequest() { IsYearEnd = true, ProfitYear = 2023, Skip = SimpleExampleRequest.Skip, Take =SimpleExampleRequest.Take};
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<YearEndProfitSharingReportResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<YearEndProfitSharingReportResponse>(new Demoulas.Common.Contracts.Contracts.Request.PaginationRequestDto(){Skip = 0 , Take=255})
                        {
                            Results =
                            [
                               YearEndProfitSharingReportResponse.ResponseExample()
                            ],
                            Total = 1,
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }
    public override Task<ReportResponseBase<YearEndProfitSharingReportResponse>> GetResponse(YearEndProfitSharingReportRequest req, CancellationToken ct)
    {
        return _cleanupReportService.GetYearEndProfitSharingReportAsync(req, ct);
    }

    public override string ReportFileName => "yearend-profit-sharing-report";

    public class YearEndProfitSharingReportClassMap: ClassMap<YearEndProfitSharingReportResponse>
    {
        public YearEndProfitSharingReportClassMap()
        {
            Map(m => m.EmployeeId).Index(0).Name("Badge Number");
            Map(m => m.EmployeeName).Index(1).Name("Employee Name");
            Map(m => m.StoreNumber).Index(2).Name("Store Number");
            Map(m => m.EmployeeTypeCode).Index(3).Name("Employee Type");
            Map(m => m.DateOfBirth).Index(4).Name("Date of Birth");
            Map(m => m.Age).Index(5).Name("Age");
            Map(m => m.EmployeeSsn).Index(6).Name("SSN");
            Map(m => m.Wages).Index(7).Name("Wages");
            Map(m => m.Hours).Index(8).Name("Hours");
            Map(m => m.Points).Index(9).Name("Points");
            Map(m => m.IsUnder21).Index(10).Name("Is under 21");
            Map(m => m.IsNew).Index(11).Name("Is new");
        }
    }

}
