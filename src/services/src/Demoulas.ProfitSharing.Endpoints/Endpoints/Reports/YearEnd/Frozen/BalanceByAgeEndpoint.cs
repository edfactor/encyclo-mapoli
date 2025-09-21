using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.BalanceByAgeEndpoint;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class BalanceByAgeEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, BalanceByAge, BalanceByAgeDetail, ProfitSharingBalanceByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public BalanceByAgeEndpoint(IFrozenReportService frozenReportService)
        : base(Navigation.Constants.BalanceByAge)
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
            s.ResponseExamples = new Dictionary<int, object> { { 200, BalanceByAge.ResponseExample() } };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<BalanceByAge> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        // Underlying base class already handles HTTP negotiation (JSON/CSV). We just ensure exceptions become domain failures upstream if needed.
        try
        {
            return await _frozenReportService.GetBalanceByAgeYearAsync(req, ct);
        }
        catch (Exception ex)
        {
            // Rethrow as is; higher middleware will map. (Alternative: wrap in a domain Result and adapt base class, but out-of-scope since base expects raw DTO.)
            throw new InvalidOperationException($"Failed to retrieve Balance By Age report: {ex.Message}", ex);
        }
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, BalanceByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingBalanceByAgeMapper>();

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
        csvWriter.WriteHeader<BalanceByAgeDetail>();
        await csvWriter.NextRecordAsync();
    }


    public class ProfitSharingBalanceByAgeMapper : ClassMap<BalanceByAgeDetail>
    {
        public ProfitSharingBalanceByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.CurrentBalance).Index(2).Name("BALANCE");
            Map(m => m.VestedBalance).Index(2).Name("VESTED");
        }
    }
}
