using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.MilitaryAndRehireEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class MilitaryAndRehireEndpoint : EndpointWithCsvBase<PaginationRequestDto, MilitaryAndRehireReportResponse, MilitaryAndRehireReportResponseMap>
{
    private readonly IMilitaryAndRehireService _reportService;

    public MilitaryAndRehireEndpoint(IMilitaryAndRehireService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("military-and-rehire");
        Summary(s =>
        {
            s.Summary = "Military and Rehire Report Endpoint";
            s.Description =
                "Provides a report on employees who are on military leave or have been rehired. This report helps identify potential issues that need to be addressed before running profit sharing. The endpoint can be executed multiple times.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<MilitaryAndRehireReportResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse>
                        {
                            Results = new List<MilitaryAndRehireReportResponse> { MilitaryAndRehireReportResponse.ResponseExample() }
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "EMPLOYEES ON MILITARY LEAVE";

    public override async Task<ReportResponseBase<MilitaryAndRehireReportResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _reportService.GetMilitaryAndRehireReport(req, ct);
    }

    public sealed class YearMonthDayTypeConverter : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
            {
                return null;
            }

            var d = (DateOnly)value;
            return d.ToString("d");
        }
    }

    public sealed class MilitaryAndRehireReportResponseMap : ClassMap<MilitaryAndRehireReportResponse>
    {
        public MilitaryAndRehireReportResponseMap()
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
