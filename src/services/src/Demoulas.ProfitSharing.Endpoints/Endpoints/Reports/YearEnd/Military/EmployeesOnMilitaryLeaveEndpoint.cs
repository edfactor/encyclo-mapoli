using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;

public class EmployeesOnMilitaryLeaveEndpoint : EndpointWithCsvBase<SortedPaginationRequestDto, EmployeesOnMilitaryLeaveResponse, EmployeesOnMilitaryLeaveEndpoint.EmployeesOnMilitaryLeaveResponseMap>
{
    private readonly ITerminationAndRehireService _reportService;

    public EmployeesOnMilitaryLeaveEndpoint(ITerminationAndRehireService reportService)
        : base(Navigation.Constants.MilitaryContributions)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("employees-on-military-leave");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "Provides a report on employees who are on military leave. This report helps identify potential issues that need to be addressed before running profit sharing. The endpoint can be executed multiple times.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<EmployeesOnMilitaryLeaveResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<EmployeesOnMilitaryLeaveResponse>
                        {
                            Results = new List<EmployeesOnMilitaryLeaveResponse> { EmployeesOnMilitaryLeaveResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "EMPLOYEES ON MILITARY LEAVE";

    public override Task<ReportResponseBase<EmployeesOnMilitaryLeaveResponse>> GetResponse(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return _reportService.GetEmployeesOnMilitaryLeaveAsync(req, ct);
    }



    public sealed class EmployeesOnMilitaryLeaveResponseMap : ClassMap<EmployeesOnMilitaryLeaveResponse>
    {
        public EmployeesOnMilitaryLeaveResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.DepartmentId).Index(2).Name("STR");
            Map(m => m.BadgeNumber).Index(3).Name("BADGE");
            Map(m => m.FullName).Index(4).Name("EMPLOYEE NAME");
            Map(m => m.DateOfBirth).Index(5).Name("BIRTH DATE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.Ssn).Index(6).Name("SSN");
            Map(m => m.TerminationDate).Index(7).Name("TERM DATE").TypeConverter<YearMonthDayTypeConverter>();
        }
    }
}
