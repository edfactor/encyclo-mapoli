using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;

using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;



public class GetDuplicateSsNsEndpoint : EndpointWithCsvBase<ProfitYearRequest, PayrollDuplicateSsnResponseDto, GetDuplicateSsNsEndpoint.GetDuplicateSsNsResponseMap>
{
    private readonly IPayrollDuplicateSsnReportService _cleanupReportService;


    public GetDuplicateSsNsEndpoint(IPayrollDuplicateSsnReportService cleanupReportService)
        : base(Navigation.Constants.DuplicateSSNsInDemographics)
    {
        _cleanupReportService = cleanupReportService;

    }

    public override void Configure()
    {
        Get("duplicate-ssns");
        Summary(s =>
        {
            s.Summary = "Get SSNs that are duplicated in the demographic table";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<PayrollDuplicateSsnResponseDto>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<PayrollDuplicateSsnResponseDto>
                        {
                            Results = new List<PayrollDuplicateSsnResponseDto>
                            {
                                PayrollDuplicateSsnResponseDto.ResponseExample()
                            }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "DuplicateSSns";

    public override Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _cleanupReportService.GetDuplicateSsnAsync(req, ct);
    }

    public sealed class GetDuplicateSsNsResponseMap : ClassMap<PayrollDuplicateSsnResponseDto>
    {
        public GetDuplicateSsNsResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("SSN");
            Map(m => m.Address.Street).Index(4).Name("ADDR");
            Map(m => m.Address.City).Index(5).Name("CITY");
            Map(m => m.Address.State).Index(6).Name("ST");
            Map(m => m.Address.PostalCode).Index(7).Name("ZIP");
            Map(m => m.HireDate).Index(8).Name("HIRE");
            Map(m => m.TerminationDate).Index(9).Name("TERM");
            Map(m => m.RehireDate).Index(10).Name("REHIRE");
            Map(m => m.Status).Index(11).Name("ST");
            Map(m => m.StoreNumber).Index(12).Name("STR");
            Map(m => m.ProfitSharingRecords).Index(13).Name("PS RECS");

            // Select the PayProfit object where LastUpdate is in May
            Map(m => m.PayProfits
                    .OrderByDescending(p => p.ProfitYear)
                    .FirstOrDefault()!.CurrentHoursYear)
                .Index(14).Name("CUR HRS");

            Map(m => m.PayProfits
                    .OrderByDescending(p => p.ProfitYear)
                    .FirstOrDefault()!.CurrentIncomeYear)
                .Index(15).Name("CUR WAGE");
        }
    }
}
