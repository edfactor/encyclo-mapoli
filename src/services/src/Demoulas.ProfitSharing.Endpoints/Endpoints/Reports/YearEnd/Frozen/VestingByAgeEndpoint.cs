using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.VestingByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class VestingByAgeEndpoint : EndpointWithCsvTotalsBase<ProfitYearAndAsOfDateRequest, VestedAmountsByAge, VestedAmountsByAgeDetail, ProfitSharingVestingByAgeByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public VestingByAgeEndpoint(IFrozenReportService frozenReportService)
        : base(Navigation.Constants.VestedAmountsByAge)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING VESTING BY AGE";

    public override void Configure()
    {
        Get("frozen/vested-amounts-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their vested balances over the year grouped by age";

            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, VestedAmountsByAge.ResponseExample() } };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<VestedAmountsByAge> GetResponse(ProfitYearAndAsOfDateRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetVestedAmountsByAgeYearAsync(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, VestedAmountsByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for VestedAmountsByAgeDetail
        csvWriter.Context.RegisterClassMap<ProfitSharingVestingByAgeByAgeMapper>();

        // Write the header row
        csvWriter.WriteHeader<VestedAmountsByAgeDetail>();
        await csvWriter.NextRecordAsync();

        // Write the detailed rows grouped by age using the mapped results
        foreach (var detail in report.Response.Results)
        {
            // Write the detail row using the mapper for counts
            csvWriter.WriteRecord(detail);
            await csvWriter.NextRecordAsync();

            // Write a secondary row for the corresponding monetary values
            csvWriter.WriteField(""); // Empty field to align monetary values under the headers
            csvWriter.WriteField(detail.FullTime100PercentAmount.ToString("N2"));
            csvWriter.WriteField(detail.FullTimePartialAmount.ToString("N2"));
            csvWriter.WriteField(detail.FullTimeNotVestedAmount.ToString("N2"));
            csvWriter.WriteField(detail.PartTime100PercentAmount.ToString("N2"));
            csvWriter.WriteField(detail.PartTimePartialAmount.ToString("N2"));
            csvWriter.WriteField(detail.PartTimeNotVestedAmount.ToString("N2"));
            await csvWriter.NextRecordAsync();
        }

        // Add a separator line
        csvWriter.WriteField("----------------------------------------------------------------------------------------------------------------");
        await csvWriter.NextRecordAsync();

        // Write the "BEN" row
        csvWriter.WriteField("BEN");
        csvWriter.WriteField(report.TotalBeneficiaryCount);
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        await csvWriter.NextRecordAsync();

        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalBeneficiaryAmount.ToString("N2"));
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        csvWriter.WriteField("");
        await csvWriter.NextRecordAsync();

        // Write the "TOTALS" section
        csvWriter.WriteField("TOTALS");
        csvWriter.WriteField(report.TotalFullTimeCount);
        csvWriter.WriteField(report.TotalPartialVestedCount);
        csvWriter.WriteField(report.TotalNotVestedCount);
        csvWriter.WriteField(""); // Placeholder for part-time counts if needed
        csvWriter.WriteField(""); // Placeholder for part-time counts if needed
        csvWriter.WriteField(""); // Placeholder for part-time counts if needed
        await csvWriter.NextRecordAsync();

        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalFullTime100PercentAmount.ToString("N2"));
        csvWriter.WriteField(report.TotalFullTimePartialAmount.ToString("N2"));
        csvWriter.WriteField(report.TotalFullTimeNotVestedAmount.ToString("N2"));
        csvWriter.WriteField(report.TotalPartTime100PercentAmount.ToString("N2"));
        csvWriter.WriteField(report.TotalPartTimePartialAmount.ToString("N2"));
        csvWriter.WriteField(report.TotalPartTimeNotVestedAmount.ToString("N2"));
        await csvWriter.NextRecordAsync();
    }


    public class ProfitSharingVestingByAgeByAgeMapper : ClassMap<VestedAmountsByAgeDetail>
    {
        public ProfitSharingVestingByAgeByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.FullTime100PercentCount).Index(1).Name("FT 100% VESTED EMPLOY/$AMNT");
            Map(m => m.FullTimePartialCount).Index(2).Name("FT PARTIAL VEST EMPLOY/$VESTED");
            Map(m => m.FullTimeNotVestedCount).Index(3).Name("FT NOT VESTED EMPLOY/$BALANCE");
            Map(m => m.PartTime100PercentCount).Index(4).Name("PT 100% VESTED EMPLOY/$AMNT");
            Map(m => m.PartTimePartialCount).Index(5).Name("PT PARTIAL VESTED EMPLOY/$VESTED");
            Map(m => m.PartTimeNotVestedCount).Index(6).Name("PT NOT VESTED EMPLOY/$BALANCE");
        }
    }
}
