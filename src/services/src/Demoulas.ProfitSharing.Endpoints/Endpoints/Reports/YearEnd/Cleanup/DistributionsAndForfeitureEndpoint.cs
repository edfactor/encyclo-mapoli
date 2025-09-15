using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup.DistributionsAndForfeitureEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;
public class DistributionsAndForfeitureEndpoint: EndpointWithCsvTotalsBase<DistributionsAndForfeituresRequest, 
    DistributionsAndForfeitureTotalsResponse, 
    DistributionsAndForfeitureResponse, 
    DistributionsAndForfeitureResponseMap>
{
    private readonly ICleanupReportService _cleanupReportService;

    public DistributionsAndForfeitureEndpoint(ICleanupReportService cleanupReportService)
        : base(Navigation.Constants.DistributionsAndForfeitures)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Get("distributions-and-forfeitures");
        Summary(s =>
        {
            s.Summary = "Lists distributions and forfeitures for a given year";
            s.ExampleRequest = new DistributionsAndForfeituresRequest() { ProfitYear = 2025, Skip = SimpleExampleRequest.Skip, Take = SimpleExampleRequest.Take };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<DistributionsAndForfeitureResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<DistributionsAndForfeitureResponse>
                        {
                            Results = new List<DistributionsAndForfeitureResponse> { DistributionsAndForfeitureResponse.ResponseExample() }
                        }
                    }
                }
            };

        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "DistributionsAndForfeitures";

    public override Task<DistributionsAndForfeitureTotalsResponse> GetResponse(DistributionsAndForfeituresRequest req, CancellationToken ct)
    {
        return _cleanupReportService.GetDistributionsAndForfeitureAsync(req, ct);
    }

    public sealed class DistributionsAndForfeitureResponseMap: ClassMap<DistributionsAndForfeitureResponse>
    {
        public DistributionsAndForfeitureResponseMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE #");
            Map(m => m.EmployeeName).Index(1).Name("NAME");
            Map(m => m.Ssn).Index(2).Name("SSN");
            Map(m => m.Date).Index(3).Name("DATE");
            Map(m => m.DistributionAmount).Index(4).Name("DISTRIBUTION AMOUNT");
            Map(m => m.TaxCode).Index(5).Name("TC");
            Map(m => m.StateTax).Index(6).Name("STATE TAX");
            Map(m => m.FederalTax).Index(7).Name("FEDERAL TAX");
            Map(m => m.ForfeitAmount).Index(8).Name("FORFEIT AMOUNT");
            Map(m => m.Age).Index(9).Name("AGE");
            Map(m => m.OtherName).Index(10).Name("OTHER NAME");
            Map(m => m.OtherSsn).Index(11).Name("OTHER SSN");
            Map(m => m.HasForfeited).Index(12).Name("Forfeited");
        }
    }
}
