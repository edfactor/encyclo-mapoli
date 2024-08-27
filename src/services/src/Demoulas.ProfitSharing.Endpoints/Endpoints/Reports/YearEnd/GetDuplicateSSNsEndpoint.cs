using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Base;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class GetDuplicateSsNsEndpoint : EndpointWithCsvBase<PaginationRequestDto, PayrollDuplicateSsnResponseDto, GetDuplicateSsNsEndpoint.GetDuplicateSsNsResponseMap>
{
    private readonly IYearEndService _yearEndService;

    public GetDuplicateSsNsEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("duplicate-ssns");
        Summary(s =>
        {
            s.Summary = "Get SSNs that are duplicated in the demographics area";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<PayrollDuplicateSsnResponseDto>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
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
        });
        Group<YearEndGroup>();
    }

    public override string ReportFileName => "DuplicateSSns";

    public override Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return _yearEndService.GetDuplicateSsNs(req, ct);
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
            Map(m => m.HoursCurrentYear).Index(14).Name("CUR HRS");
            Map(m => m.IncomeCurrentYear).Index(15).Name("CUR WAGE");
        }
    }
}
