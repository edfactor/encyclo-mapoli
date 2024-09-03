using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class PayrollDuplicateSsnsOnPayprofitEndpoint : EndpointWithCsvBase<PaginationRequestDto, PayrollDuplicateSsnsOnPayprofitResponseDto, PayrollDuplicateSsnsOnPayprofitEndpoint.PayrollDuplicateSsnsOnPayprofitResponseMap>
{
    private readonly IYearEndService _reportService;

    public PayrollDuplicateSsnsOnPayprofitEndpoint(IYearEndService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("payroll-duplicate-ssns-on-payprofit");
        Summary(s =>
        {
            s.Summary = "Payroll duplicate ssns on payprofit";
            s.Description =
                @"SSN and ""clean up"" reports to highlight possible problems which should be corrected before profit sharing is run. This job can be run multiple times.";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<PayrollDuplicateSsnsOnPayprofitResponseDto>
                        {
                            Results = new List<PayrollDuplicateSsnsOnPayprofitResponseDto>
                            {
                                new PayrollDuplicateSsnsOnPayprofitResponseDto
                                {
                                    EmployeeBadge = 47425,
                                    EmployeeSsn = 900047425,
                                    Name = "John",
                                    Status = EmploymentStatus.Constants.Active,
                                    Store = 14,
                                    IncomeCurrentYear = 32_100,
                                    PayProfitSsn = 900047425,
                                    Address = new AddressResponseDto
                                    {
                                        Street = "123 Main",
                                        City = "Sydney",
                                        State = "HI",
                                        CountryIso = Country.Constants.Us,
                                        PostalCode = "01234"
                                    },
                                    ContactInfo = new ContactInfoResponseDto()
                                }
                            }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5))));
    }

    public override string ReportFileName => "PAYROLL DUPLICATE SSNs ON PAYPROFIT";

    public override async Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _reportService.GetPayrollDuplicateSsnsOnPayprofit(req, ct);
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
            return d.ToString("YYYYMMDD");
        }
    }

    public sealed class PayrollDuplicateSsnsOnPayprofitResponseMap : ClassMap<PayrollDuplicateSsnsOnPayprofitResponseDto>
    {
        public PayrollDuplicateSsnsOnPayprofitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.PayProfitSsn).Index(3).Name("SSN");
            Map(m => m.Name).Index(4).Name("NAME");
            Map(m => m.Address.Street).Index(5).Name("ADDR");
            Map(m => m.Address.City).Index(6).Name("CITY");
            Map(m => m.Address.State).Index(7).Name("ST");
            Map(m => m.Address.PostalCode).Index(8).Name("ZIP");
            Map(m => m.HireDate).Index(9).Name("HIRE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.TermDate).Index(10).Name("TERM").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.RehireDate).Index(11).Name("REHIRE").TypeConverter<YearMonthDayTypeConverter>(); 
            Map(m => m.Status).Index(12).Name("ST");
            Map(m => m.Store).Index(13).Name("STR");
            Map(m => m.PayProfitSsn).Index(13).Name("PS RECS");

            Map(m => m.IncomeCurrentYear).Index(15).Name("CUR WAGE");
        }
    }
}
