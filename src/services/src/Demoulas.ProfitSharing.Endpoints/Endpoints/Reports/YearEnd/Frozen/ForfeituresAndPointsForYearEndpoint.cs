using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
public class ForfeituresAndPointsForYearEndpoint : EndpointWithCsvTotalsBase<FrozenProfitYearRequest, ForfeituresAndPointsForYearResponseWithTotals, ForfeituresAndPointsForYearResponse, ForfeituresAndPointsForYearEndpoint.ForfeituresAndPointsForYearEndpointMapper>
{
    private readonly IForfeituresAndPointsForYearService _forfeituresAndPointsForYearService;

    public ForfeituresAndPointsForYearEndpoint(IForfeituresAndPointsForYearService forfeituresAndPointsForYearService)
        : base(Navigation.Constants.Forfeitures)
    {
        _forfeituresAndPointsForYearService = forfeituresAndPointsForYearService;
    }
    public override string ReportFileName => "Forfeitures and Points for Year";

    public override void Configure()
    {
        Get("frozen/forfeitures-and-points");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their points, and any forfeitures over the year";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    ForfeituresAndPointsForYearResponseWithTotals.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ForfeituresAndPointsForYearResponseWithTotals> GetResponse(FrozenProfitYearRequest req, CancellationToken ct)
    {
        return _forfeituresAndPointsForYearService.GetForfeituresAndPointsForYearAsync(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, ForfeituresAndPointsForYearResponseWithTotals report, CancellationToken cancellationToken)
    {
        csvWriter.WriteField("Total Forfeitures:");
        csvWriter.WriteField(report.TotalForfeitures);
        await csvWriter.NextRecordAsync();
        
        csvWriter.WriteField("Total Forfeit Points:");
        csvWriter.WriteField(report.TotalForfeitPoints);
        await csvWriter.NextRecordAsync();
        
        csvWriter.WriteField("Total Earning Points:");
        csvWriter.WriteField(report.TotalEarningPoints);
        await csvWriter.NextRecordAsync();
        
        await csvWriter.NextRecordAsync();

        csvWriter.Context.RegisterClassMap<ForfeituresAndPointsForYearEndpointMapper>();
        await csvWriter.WriteRecordsAsync(report.Response.Results, cancellationToken);
    }

    public class ForfeituresAndPointsForYearEndpointMapper : ClassMap<ForfeituresAndPointsForYearResponse>
    {
        public ForfeituresAndPointsForYearEndpointMapper()
        {
            Map(m => m.BadgeNumber).Index(0).Name("EMPLOYEE_BADGE");
            Map(m => m.EmployeeName).Index(1).Name("EMPLOYEE_NAME");
            Map(m => m.Ssn).Index(2).Name("EMPLOYEE_SSN");
            Map(m => m.Forfeitures).Index(3).Name("FORFEITURES");
            Map(m => m.ContForfeitPoints).Index(4).Name("CONTFORFEIT_POINTS");
            Map(m => m.EarningPoints).Index(5).Name("EARNING_POINTS");
            Map(m => m.BeneficiaryPsn).Index(6).Name("BENEFICIARY_SSN");
        }
    }
}
