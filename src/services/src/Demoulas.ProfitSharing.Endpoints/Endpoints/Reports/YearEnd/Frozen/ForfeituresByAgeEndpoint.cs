using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.ForfeituresByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class ForfeituresByAgeEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, ForfeituresByAge, ForfeituresByAgeDetail, ProfitSharingForfeituresByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public ForfeituresByAgeEndpoint(IFrozenReportService frozenReportService)
        : base(Navigation.Constants.ForfeituresByAge)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING FORFEITURE BY AGE";

    public override void Configure()
    {
        Get("frozen/forfeitures-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their forfeitures over the year grouped by age";

            s.ExampleRequest = FrozenReportsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, ForfeituresByAge.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ForfeituresByAge> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetForfeituresByAgeYearAsync(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, ForfeituresByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingForfeituresByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("FORF TTL");
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalAmount);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ForfeituresByAgeDetail>();
        await csvWriter.NextRecordAsync();

       
    }
  

    public class ProfitSharingForfeituresByAgeMapper : ClassMap<ForfeituresByAgeDetail>
    {
        public ProfitSharingForfeituresByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.Amount).Index(2).Name("AMOUNT");
        }
    }
}
