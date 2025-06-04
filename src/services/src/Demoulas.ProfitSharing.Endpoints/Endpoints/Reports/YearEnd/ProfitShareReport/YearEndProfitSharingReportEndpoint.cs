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
public class YearEndProfitSharingReportEndpoint: EndpointWithCsvTotalsBase<YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse,YearEndProfitSharingReportDetail, YearEndProfitSharingReportEndpoint.YearEndProfitSharingReportClassMap>
{
    private readonly IProfitSharingSummaryReportService _cleanupReportService;

    public YearEndProfitSharingReportEndpoint(IProfitSharingSummaryReportService cleanupReportService)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Post("yearend-profit-sharing-report");
        Summary(s =>
        {
            s.Summary = "Year end profit sharing report";
            s.Description = "Returns a list of employees who will participate in the profit sharing this year, as well as their qualifying attributes.";
            s.ExampleRequest = new YearEndProfitSharingReportRequest() { IsYearEnd = true, ProfitYear = 2025, Skip = SimpleExampleRequest.Skip, Take =SimpleExampleRequest.Take};
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    YearEndProfitSharingReportResponse.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }
    public override Task<YearEndProfitSharingReportResponse> GetResponse(YearEndProfitSharingReportRequest req, CancellationToken ct)
    {
        return _cleanupReportService.GetYearEndProfitSharingReportAsync(req, ct);
    }

    public override string ReportFileName => "yearend-profit-sharing-report";

    public class YearEndProfitSharingReportClassMap: ClassMap<YearEndProfitSharingReportDetail>
    {
        public YearEndProfitSharingReportClassMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("Badge Number");
            Map(m => m.EmployeeName).Index(1).Name("Employee Name");
            Map(m => m.StoreNumber).Index(2).Name("Store Number");
            Map(m => m.EmployeeTypeCode).Index(3).Name("Employee Type");
            Map(m => m.DateOfBirth).Index(4).Name("Date of Birth");
            Map(m => m.Age).Index(5).Name("Age");
            Map(m => m.Ssn).Index(6).Name("SSN");
            Map(m => m.Wages).Index(7).Name("Wages");
            Map(m => m.Hours).Index(8).Name("Hours");
            Map(m => m.Points).Index(9).Name("Points");
            Map(m => m.IsUnder21).Index(10).Name("Is under 21");
            Map(m => m.IsNew).Index(11).Name("Is new");
            Map(m => m.EmployeeStatus).Index(12).Name("Employee Status");
            Map(m => m.Balance).Index(13).Name("Balance");
            Map(m => m.YearsInPlan).Index(14).Name("Years in Plan");
            Map(m => m.TerminationDate).Index(15).Name("Inactive date");
        }
    }

}
