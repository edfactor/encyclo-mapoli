using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.BalanceByAgeEndpoint;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.ForfeituresByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class BalanceByAgeEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, BalanceByAge, BalanceByAgeDetail, ProfitSharingBalanceByAgeByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public BalanceByAgeEndpoint(IFrozenReportService frozenReportService)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING BALANCE BY AGE";

    public override void Configure()
    {
        Get("frozen/balance-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their balances over the year grouped by age";

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

    public override Task<BalanceByAge> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetBalanceByAgeYear(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, BalanceByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingForfeituresByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("BEN");
        csvWriter.WriteField(report.TotalBeneficiaries);
        csvWriter.WriteField(report.TotalBeneficiariesAmount);
        csvWriter.WriteField(report.TotalBeneficiariesVestedAmount);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalNonBeneficiaries);
        csvWriter.WriteField(report.TotalNonBeneficiariesAmount);
        csvWriter.WriteField(report.TotalNonBeneficiariesVestedAmount);


        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ForfeituresByAgeDetail>();
        await csvWriter.NextRecordAsync();

       
    }
  

    public class ProfitSharingBalanceByAgeByAgeMapper : ClassMap<BalanceByAgeDetail>
    {
        public ProfitSharingBalanceByAgeByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.CurrentBalance).Index(2).Name("BALANCE");
            Map(m => m.VestedBalance).Index(2).Name("VESTED");
        }
    }
}
