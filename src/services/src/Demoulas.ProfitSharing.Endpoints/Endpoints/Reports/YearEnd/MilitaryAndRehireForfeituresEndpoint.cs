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
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class MilitaryAndRehireForfeituresEndpoint :
    EndpointWithCsvBase<PaginationRequestDto, MilitaryAndRehireForfeituresResponse, MilitaryAndRehireForfeituresEndpoint.MilitaryRehireProfitSharingResponseMap>
{
    private readonly IMilitaryAndRehireService _reportService;

    public MilitaryAndRehireForfeituresEndpoint(IMilitaryAndRehireService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("military-and-rehire-forfeitures");
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
                    new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<MilitaryAndRehireForfeituresResponse>
                        {
                            Results = new List<MilitaryAndRehireForfeituresResponse> { MilitaryAndRehireForfeituresResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "REHIRE'S PROFIT SHARING DATA";

    public override async Task<ReportResponseBase<MilitaryAndRehireForfeituresResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _reportService.FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYears(req, ct);
    }

    protected internal override void GenerateCsvContent(CsvWriter csvWriter, ReportResponseBase<MilitaryAndRehireForfeituresResponse> report)
    {
        csvWriter.Context.RegisterClassMap<MilitaryRehireProfitSharingResponseMap>();

        foreach (var member in report.Response.Results)
        {
            // Write the member details once
            csvWriter.WriteRecord(member);
            csvWriter.NextRecord();

            // Write each profit-detail record under the employee details
            foreach (var record in member.Details)
            {
                csvWriter.WriteField(string.Empty); // Empty field for BadgeNumber
                csvWriter.WriteField(string.Empty); // Empty field for EmployeeName
                csvWriter.WriteField(string.Empty); // Empty field for SSN
                csvWriter.WriteField(string.Empty); // Empty field for ReHireDate
                csvWriter.WriteField(string.Empty); // Empty field for PY-YRS
                csvWriter.WriteField(string.Empty); // Empty field for YTD HOURS
                csvWriter.WriteField(string.Empty); // Empty field for EC
                csvWriter.WriteField(record.ProfitYear); // YEAR
                csvWriter.WriteField(record.Forfeiture); // FORFEITURES
                csvWriter.WriteField(record.Remark); // COMMENT
                csvWriter.NextRecord();
            }
        }
    }

    public sealed class MilitaryRehireProfitSharingResponseMap : ClassMap<MilitaryAndRehireForfeituresResponse>
    {
        public MilitaryRehireProfitSharingResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.FullName).Index(3).Name("EMPLOYEE NAME");
            Map(m => m.Ssn).Index(4).Name("SSN");
            Map(m => m.ReHiredDate).Index(5).Name("REHIRED").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.CompanyContributionYears).Index(6).Name("PY-YRS");
            Map(m => m.HoursCurrentYear).Index(7).Name("YTD HOURS").TypeConverterOption.Format("0.00");
            Map().Index(8).Name("EC"); // Assuming EC is blank, leave an empty column
        }
    }
}
    
