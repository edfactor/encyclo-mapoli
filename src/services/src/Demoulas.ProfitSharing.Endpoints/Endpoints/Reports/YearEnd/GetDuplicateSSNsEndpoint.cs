using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Csv;


namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class GetDuplicateSSNsEndpoint : EndpointWithCSVBase<EmptyRequest, PayrollDuplicateSSNResponseDto, GetDuplicateSSNsEndpoint.GetDuplicateSSNsResponseMap>
{
    private readonly IYearEndService _yearEndService;

    public GetDuplicateSSNsEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("duplicatessns");
        Summary(s =>
        {
            s.Summary = "Get SSNs that are duplicated in the demographics area";
        });
        Group<YearEndGroup>();
    }

    public override string ReportFileName => "DuplicateSSns";

    public override Task<ReportResponseBase<PayrollDuplicateSSNResponseDto>> GetResponse(CancellationToken ct)
    {
        return _yearEndService.GetDuplicateSSNs(ct);
    }
    
    public sealed class GetDuplicateSSNsResponseMap : ClassMap<PayrollDuplicateSSNResponseDto>
    {
        public GetDuplicateSSNsResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.SSN).Index(3).Name("SSN");
            Map(m => m.Address).Index(4).Name("ADDR");
            Map(m => m.City).Index(5).Name("CITY");
            Map(m => m.State).Index(6).Name("ST");
            Map(m => m.PostalCode).Index(7).Name("ZIP");
            Map(m => m.HireDate).Index(8).Name("HIRE");
            Map(m => m.TerminationDate).Index(9).Name("TERM");
            Map(m => m.RehireDate).Index(10).Name("REHIRE");
            Map(m => m.Status).Index(11).Name("ST");
            Map(m => m.StoreNumber).Index(12).Name("STR");
            Map(m => m.ProfitSharingRecords).Index(13).Name("PS RECS");
            Map(m => m.HoursCurrentYear).Index(14).Name("CUR HRS");
            Map(m => m.EarningsCurrentYear).Index(15).Name("CUR WAGE");
        }
    }
}
