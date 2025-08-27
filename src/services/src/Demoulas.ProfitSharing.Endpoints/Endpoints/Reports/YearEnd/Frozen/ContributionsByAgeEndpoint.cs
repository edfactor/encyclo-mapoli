using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.ContributionsByAgeEndpoint;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class ContributionsByAgeEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, ContributionsByAge, ContributionsByAgeDetail, ProfitSharingContributionsByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public ContributionsByAgeEndpoint(IFrozenReportService frozenReportService)
        : base(Navigation.Constants.ContributionsByAge)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING CONTRIBUTIONS BY AGE";

    public override void Configure()
    {
        Get("frozen/contributions-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their contributions over the year grouped by age";

            s.ExampleRequest = FrozenReportsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, ContributionsByAge.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ContributionsByAge> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetContributionsByAgeYearAsync(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, ContributionsByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingContributionsByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("CONT TTL");
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalAmount);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ContributionsByAgeDetail>();
        await csvWriter.NextRecordAsync();

       
    }
  

    public class ProfitSharingContributionsByAgeMapper : ClassMap<ContributionsByAgeDetail>
    {
        public ProfitSharingContributionsByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.Amount).Index(2).Name("AMOUNT");
        }
    }
}
