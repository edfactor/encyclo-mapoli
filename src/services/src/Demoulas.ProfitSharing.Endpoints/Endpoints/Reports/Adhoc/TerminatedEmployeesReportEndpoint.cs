using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc.TerminatedEmployeesReportEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;
public sealed class TerminatedEmployeesReportEndpoint :EndpointWithCsvBase<FrozenProfitYearRequest, AdhocTerminatedEmployeeResponse, TerminatedEmployeesReportResponseMap>
{
    private readonly IAdhocTerminatedEmployeesService _adhocTerminatedEmployeesService;

    public TerminatedEmployeesReportEndpoint(IAdhocTerminatedEmployeesService adhocTerminatedEmployeesService)
        : base(Navigation.Constants.QPAY066AdHocReports)
    {
        _adhocTerminatedEmployeesService = adhocTerminatedEmployeesService;
    }

    public override void Configure()
    {
        Get("adhoc-terminated-employees-report");
        Summary(s =>
        {
            s.Summary = "Adhoc Terminated Employees Report";
            s.Description = "Returns a report of terminated employees within a specified profit year.";
            s.ExampleRequest = new FrozenProfitYearRequest
            {
                ProfitYear = 2023,
                UseFrozenData = true,
                Skip = 0,
                Take = 100,
                SortBy = "TerminationDate",
                IsSortDescending = false
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<AdhocTerminatedEmployeeResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.MinValue,
                        EndDate = DateOnly.MaxValue,
                        Response = new PaginatedResponseDto<AdhocTerminatedEmployeeResponse>
                        {
                            Results = new List<AdhocTerminatedEmployeeResponse>
                            {
                                new AdhocTerminatedEmployeeResponse
                                {
                                    BadgeNumber = 12345,
                                    FullName = "John Doe",
                                    Ssn = "123-45-6789",
                                    TerminationDate = new DateOnly(2023, 5, 15),
                                    TerminationCodeId = 'A'
                                }
                            },
                            Total = 1
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }
    public override string ReportFileName => "Adhoc Terminated Employees Report";

    public override Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetResponse(FrozenProfitYearRequest req, CancellationToken ct)
    {
        return _adhocTerminatedEmployeesService.GetTerminatedEmployees(req, ct);
    }

    public class TerminatedEmployeesReportResponseMap : ClassMap<AdhocTerminatedEmployeeResponse>
    {
        public TerminatedEmployeesReportResponseMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("Badge Number");
            Map(m => m.FullName).Index(1).Name("Full Name");
            Map(m => m.Ssn).Index(2).Name("SSN");
            Map(m => m.TerminationDate).Index(3).Name("Termination Date").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.TerminationCodeId).Index(4).Name("Termination Code");
        }
    }
}
