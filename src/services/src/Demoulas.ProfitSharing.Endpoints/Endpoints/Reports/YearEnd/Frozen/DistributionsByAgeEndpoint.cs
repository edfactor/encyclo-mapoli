using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.DistributionsByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class DistributionsByAgeEndpoint : EndpointWithCsvTotalsBase<DistributionsByAgeRequest, DistributionsByAge, DistributionsByAgeDetail, ProfitSharingDistributionsByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public DistributionsByAgeEndpoint(IFrozenReportService frozenReportService)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING DISTRIBUTIONS BY AGE";

    public override void Configure()
    {
        Get("frozen/distributions-by-age");
        Summary(s =>
        {
            s.Summary = "PROFIT SHARING DISTRIBUTIONS BY AGE";
            s.Description =
                "This report produces a list of members showing their points, and any forfeitures over the year";

            s.ExampleRequest = DistributionsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, DistributionsByAge.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<DistributionsByAge> GetResponse(DistributionsByAgeRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetDistributionsByAgeYear(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, DistributionsByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingDistributionsByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("");
        csvWriter.WriteField(report.RegularTotalEmployees);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("HARDSHIP");
        csvWriter.WriteField(report.HardshipTotalEmployees);
        csvWriter.WriteField(report.HardshipTotalAmount);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("DIST TTL");
        csvWriter.WriteField("");
        csvWriter.WriteField(report.DistributionTotalAmount);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<DistributionsByAgeDetail>();
        await csvWriter.NextRecordAsync();

       
    }
  

    public class ProfitSharingDistributionsByAgeMapper : ClassMap<DistributionsByAgeDetail>
    {
        public ProfitSharingDistributionsByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.Amount).Index(2).Name("AMOUNT");
        }
    }
}
