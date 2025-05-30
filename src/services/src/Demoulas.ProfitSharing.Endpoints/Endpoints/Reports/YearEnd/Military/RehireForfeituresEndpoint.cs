using CsvHelper;
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

public class RehireForfeituresEndpoint :
    EndpointWithCsvBase<StartAndEndDateRequest, RehireForfeituresResponse, RehireForfeituresEndpoint.RehireProfitSharingResponseMap>
{
    private readonly ITerminationAndRehireService _reportService;

    public RehireForfeituresEndpoint(ITerminationAndRehireService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Post("rehire-forfeitures");
        Summary(s =>
        {
            s.Summary = "Rehire Forfeiture Adjustments Endpoint";
            s.Description =
                "The Rehire Profit Sharing Data endpoint produces a comprehensive report on employees who are either currently on military leave or have been rehired, focusing on their eligibility for forfeiture adjustments in profit-sharing. The report contains employee information, such as badge number, rehire date, and current year-to-date hours, along with profit-sharing records, including profit year, forfeiture amounts, and comments. This report supports multiple executions and is primarily used to address forfeiture discrepancies before profit sharing is finalized.";

            s.ExampleRequest = StartAndEndDateRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<RehireForfeituresResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<RehireForfeituresResponse>
                        {
                            Results = new List<RehireForfeituresResponse> { RehireForfeituresResponse.ResponseExample() }
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

    public override Task<ReportResponseBase<RehireForfeituresResponse>> GetResponse(StartAndEndDateRequest req, CancellationToken ct)
    {
        return _reportService.FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, ReportResponseBase<RehireForfeituresResponse> report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<RehireProfitSharingResponseMap>();

        // Write the headers using the registered class map
        csvWriter.WriteHeader<RehireForfeituresResponse>();

        // Add additional headers for the details section (Profit Year, Forfeitures, Comment)
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField(string.Empty);
        csvWriter.WriteField("YEAR");
        csvWriter.WriteField("FORFEITURES");
        csvWriter.WriteField("COMMENT");

        // Move to the next record to separate the headers from the data
        await csvWriter.NextRecordAsync();

        // Write the records (member + details)
        foreach (var member in report.Response.Results)
        {
            // Write the member details once
            csvWriter.WriteRecord(member);
            await csvWriter.NextRecordAsync();

            // Write each profit-detail record under the employee details
            foreach (var record in member.Details)
            {
                // These fields correspond to the empty fields for the member data
                csvWriter.WriteField(string.Empty); // Empty field for BadgeNumber
                csvWriter.WriteField(string.Empty); // Empty field for EmployeeName
                csvWriter.WriteField(string.Empty); // Empty field for SSN
                csvWriter.WriteField(string.Empty); // Empty field for ReHireDate
                csvWriter.WriteField(string.Empty); // Empty field for PY-YRS
                csvWriter.WriteField(string.Empty); // Empty field for YTD HOURS
                csvWriter.WriteField(record.ProfitYear);   // YEAR
                csvWriter.WriteField(record.Forfeiture);   // FORFEITURES
                csvWriter.WriteField(record.Remark);       // COMMENT
                await csvWriter.NextRecordAsync();
            }
        }
    }

    public sealed class RehireProfitSharingResponseMap : ClassMap<RehireForfeituresResponse>
    {
        public RehireProfitSharingResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.FullName).Index(3).Name("EMPLOYEE NAME");
            Map(m => m.Ssn).Index(4).Name("SSN");
            Map(m => m.ReHiredDate).Index(5).Name("REHIRED").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.HireDate).Index(6).Name("HIRE DATE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.TerminationDate).Index(7).Name("TERMINATION DATE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.StoreNumber).Index(8).Name("STORE");
            Map(m => m.NetBalanceLastYear).Index(9).Name("BEGINNING BALANCE").TypeConverterOption.Format("0.00");
            Map(m => m.VestedBalanceLastYear).Index(10).Name("BEGIN VESTED AMOUNT").TypeConverterOption.Format("0.00");
            Map(m => m.CompanyContributionYears).Index(11).Name("PY-YRS");
            Map(m => m.HoursCurrentYear).Index(12).Name("YTD HOURS").TypeConverterOption.Format("0.00");
            Map().Index(13).Name("EC"); // Assuming EC is blank, leave an empty column
        }
    }
}

