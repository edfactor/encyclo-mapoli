using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public partial class PayrollDuplicateSsnsOnPayprofitEndpoint : EndpointWithoutRequest<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>>
{
    private readonly IYearEndService _reportService;

    public PayrollDuplicateSsnsOnPayprofitEndpoint(IYearEndService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("payroll-duplicate-ssns-on-payprofit");
        Summary(s =>
        {
            s.Summary = "Payroll duplicate ssns on payprofit";
            s.Description = @"SSN and ""clean up"" reports to highlight possible problems which should be corrected before profit sharing is run. This job can be run multiple times.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>
                    {
                        ReportName = "PAYROLL DUPLICATE SSNs ON PAYPROFIT", 
                        ReportDate = DateTimeOffset.Now,
                        Results = new HashSet<PayrollDuplicateSsnsOnPayprofitResponseDto>
                        {
                            new PayrollDuplicateSsnsOnPayprofitResponseDto
                            {
                                EmployeeBadge = 47425, 
                                EmployeeSSN = 900047425, 
                                Name = "John", 
                                Status = EmploymentStatus.Constants.Active, 
                                Store = 14,
                                EarningsCurrentYear = 32_100,
                                PayProfitSSN = 900047425,
                                Address = new AddressResponseDto
                                {
                                    Street = "123 Main",
                                    City = "Sydney",
                                    State = "HI",
                                    CountryISO = Common.Constants.US,
                                    PostalCode = "01234"
                                },
                                ContactInfo = new ContactInfoResponseDto()
                            }
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5))));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string acceptHeader = HttpContext.Request.Headers["Accept"].ToString().ToLower(CultureInfo.InvariantCulture);

        ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto> response = await _reportService.GetPayrollDuplicateSsnsOnPayprofit(ct);

        if (acceptHeader.Contains("text/csv"))
        {
            await using MemoryStream csvData = GenerateCsvStream(response);
            await SendStreamAsync(csvData, "PAYROLL DUPLICATE SSNs ON PAYPROFIT.csv", cancellation: ct);
            return;
        }

        await SendOkAsync(response, ct);
    }


    private MemoryStream GenerateCsvStream(ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto> report)
    {
        MemoryStream memoryStream = new MemoryStream();
        using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        using (CsvWriter csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            streamWriter.WriteLine($"{report.ReportDate:MMM dd yyyy HH:mm}");
            streamWriter.WriteLine(report.ReportName);

            csvWriter.Context.RegisterClassMap<PayrollDuplicateSsnsOnPayprofitResponseMap>();
            csvWriter.WriteRecords(report.Results);
            streamWriter.Flush();
        }
        memoryStream.Position = 0; // Reset the stream position to the beginning
        return memoryStream;
    }

    private sealed class PayrollDuplicateSsnsOnPayprofitResponseMap : ClassMap<PayrollDuplicateSsnsOnPayprofitResponseDto>
    {
        public PayrollDuplicateSsnsOnPayprofitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.PayProfitSSN).Index(3).Name("SSN");
            Map(m => m.Name).Index(4).Name("NAME");
            Map(m => m.Address.Street).Index(5).Name("ADDR");
            Map(m => m.Address.City).Index(6).Name("CITY");
            Map(m => m.Address.State).Index(7).Name("ST");
            Map(m => m.Address.PostalCode).Index(8).Name("ZIP");
            Map(m => m.HireDate).Index(9).Name("HIRE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.TermDate).Index(10).Name("TERM").TypeConverter<YearMonthDayTypeConverter>(); ;
            Map(m => m.RehireDate).Index(11).Name("REHIRE").TypeConverter<YearMonthDayTypeConverter>(); 
            Map(m => m.Status).Index(12).Name("ST");
            Map(m => m.Store).Index(13).Name("STR");
            Map(m => m.PayProfitSSN).Index(13).Name("PS RECS");

            Map(m => m.EarningsCurrentYear).Index(15).Name("CUR WAGE");
        }
    }
}
