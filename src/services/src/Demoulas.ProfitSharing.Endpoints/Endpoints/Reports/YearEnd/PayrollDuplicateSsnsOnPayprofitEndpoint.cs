using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Services.Reports;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class PayrollDuplicateSsnsOnPayprofitEndpoint : EndpointWithCSVBase<EmptyRequest, PayrollDuplicateSsnsOnPayprofitResponseDto, PayrollDuplicateSsnsOnPayprofitEndpoint.PayrollDuplicateSsnsOnPayprofitResponseMap>
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

    public override string ReportFileName => "PAYROLL DUPLICATE SSNs ON PAYPROFIT";

    public override async Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetResponse(CancellationToken ct)
    {
        return await _reportService.GetPayrollDuplicateSsnsOnPayprofit(ct);
    }

    public sealed class PayrollDuplicateSsnsOnPayprofitResponseMap : ClassMap<PayrollDuplicateSsnsOnPayprofitResponseDto>
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
            Map(m => m.HireDate.ToString("YYYYMMDD")).Index(9).Name("HIRE");
            Map(m => m.TermDate!.Value.ToString("YYYYMMDD")).Index(10).Name("TERM");
            Map(m => m.RehireDate!.Value.ToString("YYYYMMDD")).Index(11).Name("REHIRE");
            Map(m => m.Status).Index(12).Name("ST");
            Map(m => m.Store).Index(13).Name("STR");
            Map(m => m.PayProfitSSN).Index(13).Name("PS RECS");

            Map(m => m.EarningsCurrentYear).Index(15).Name("CUR WAGE");
        }
    }
}
