using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.VestingByAgeEndpoint;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.ForfeituresByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class VestingByAgeEndpoint : EndpointWithCsvTotalsBase<ProfitYearRequest, VestedAmountsByAge, VestedAmountsByAgeDetail, ProfitSharingVestingByAgeByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public VestingByAgeEndpoint(IFrozenReportService frozenReportService)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING VESTING BY AGE";

    public override void Configure()
    {
        Get("frozen/vesting-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their vested balances over the year grouped by age";

            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, VestedAmountsByAge.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<VestedAmountsByAge> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetVestedAmountsByAgeYearAsync(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, VestedAmountsByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingForfeituresByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("BEN");
        csvWriter.WriteField(report.TotalBeneficiaryCount);
        csvWriter.WriteField(report.TotalPartTime100PercentAmount);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalFullTimeCount);
        csvWriter.WriteField(report.TotalFullTime100PercentAmount);


        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ForfeituresByAgeDetail>();
        await csvWriter.NextRecordAsync();

       
    }
  

    public class ProfitSharingVestingByAgeByAgeMapper : ClassMap<VestedAmountsByAgeDetail>
    {
        public ProfitSharingVestingByAgeByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
        }
    }
}
