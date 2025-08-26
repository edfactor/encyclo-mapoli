using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.BalanceByYearsEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class BalanceByYearsEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, BalanceByYears, BalanceByYearsDetail, ProfitSharingBalanceByYearsMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public BalanceByYearsEndpoint(IFrozenReportService frozenReportService)
        : base(Navigation.Constants.BalanceByYears)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING BALANCE BY YEARS";

    public override void Configure()
    {
        Get("frozen/balance-by-years");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their balances over the year grouped by years";

            s.ExampleRequest = FrozenReportsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, BalanceByYears.ResponseExample() } };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<BalanceByYears> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetBalanceByYearsAsync(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, BalanceByYears report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingBalanceByYearsMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("BEN");
        csvWriter.WriteField(report.TotalBeneficiaries);
        csvWriter.WriteField(report.TotalBeneficiariesAmount);
        csvWriter.WriteField(report.TotalBeneficiariesVestedAmount);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalEmployee);
        csvWriter.WriteField(report.TotalEmployeeAmount);
        csvWriter.WriteField(report.TotalEmployeesVestedAmount);


        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ForfeituresByAgeDetail>();
        await csvWriter.NextRecordAsync();
    }


    public class ProfitSharingBalanceByYearsMapper : ClassMap<BalanceByYearsDetail>
    {
        public ProfitSharingBalanceByYearsMapper()
        {
            Map(m => m.Years).Index(0).Name("YRS");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.CurrentBalance).Index(2).Name("BALANCE");
            Map(m => m.VestedBalance).Index(2).Name("VESTED");
        }
    }
}
