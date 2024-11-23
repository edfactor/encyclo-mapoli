using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;

public class MilitaryAndRehireProfitSummaryEndpoint :
    EndpointWithCsvBase<ProfitYearRequest, MilitaryAndRehireProfitSummaryResponse, MilitaryAndRehireProfitSummaryEndpoint.MilitaryRehireProfitSharingResponseMap>
{
    private readonly IMilitaryAndRehireService _reportService;

    public MilitaryAndRehireProfitSummaryEndpoint(IMilitaryAndRehireService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("military-and-rehire-profit-summary/{reportingYear}");
        Summary(s =>
        {
            s.Summary = "Military and Rehire Profit Sharing Data Report Endpoint";
            s.Description =
                "The Military and Rehire Profit Sharing Data endpoint produces a comprehensive report on employees who are either currently on military leave or have been rehired, focusing on their eligibility for forfeiture adjustments in profit-sharing. The report contains employee information, such as badge number, rehire date, and current year-to-date hours, along with profit-sharing records, including profit year, forfeiture amounts, and comments. This report supports multiple executions and is primarily used to address forfeiture discrepancies before profit sharing is finalized.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<MilitaryAndRehireProfitSummaryResponse>
                        {
                            Results = new List<MilitaryAndRehireProfitSummaryResponse> { MilitaryAndRehireProfitSummaryResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "MILITARY TERM-REHIRE";

    public override Task<ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _reportService.GetMilitaryAndRehireProfitSummaryReport(req, ct);
    }

    public sealed class MilitaryRehireProfitSharingResponseMap : ClassMap<MilitaryAndRehireProfitSummaryResponse>
    {
        public MilitaryRehireProfitSharingResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("SSN");
            Map(m => m.FullName).Index(4).Name("NAME");
            Map(m => m.StoreNumber).Index(5).Name("STR");
            Map(m => m.HireDate).Index(6).Name("HIRE DT").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.ReHiredDate).Index(7).Name("REHIRE DT").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.TerminationDate).Index(8).Name("TERM DT").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.EmploymentStatusId).Index(9).Name("STATUS");
            Map(m => m.NetBalanceLastYear).Index(10).Name("BEG BAL").TypeConverterOption.Format("0.00");
            Map(m => m.VestedBalanceLastYear).Index(11).Name("BEG VEST").TypeConverterOption.Format("0.00");
            Map(m => m.HoursCurrentYear).Index(12).Name("CUR HRS").TypeConverterOption.Format("0.0");
            Map(m => m.CompanyContributionYears).Index(13).Name("PLAN YEARS");
            Map(m => m.EnrollmentId).Index(14).Name("ENROLL");
            Map(m => m.ProfitYear).Index(15).Name("YEAR");
            Map(m => m.Remark).Index(16).Name("CMNT");
            Map(m => m.Forfeiture).Index(17).Name("FORT AMT");
        }
    }
}
    
