using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Eligibility;

public class GetEligibleEmployeesEndpoint : EndpointWithCsvTotalsBase<ProfitYearRequest, GetEligibleEmployeesResponse, GetEligibleEmployeesResponseDto, GetEligibleEmployeesEndpoint.GetEligibleEmployeesResponseDtoMap>
{
    private readonly IGetEligibleEmployeesService _getEligibleEmployeesService;
    public override string ReportFileName { get; } = "GetEligibleEmployeesReport.csv";

    public GetEligibleEmployeesEndpoint(
        IGetEligibleEmployeesService getEligibleEmployeesService)
    {
        _getEligibleEmployeesService = getEligibleEmployeesService;
    }

    public override void Configure()
    {
        Get("/eligible-employees");
        Summary(s =>
        {
            s.Summary = "Provide the Eligible Employees report.";
            s.Description =
                "Reports on employees eligible to participate in profit sharing based on several factors considered at fiscal end of year.";
            
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<GetEligibleEmployeesResponse> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _getEligibleEmployeesService.GetEligibleEmployees(req, ct);
    }

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, GetEligibleEmployeesResponse responseWithTotals, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<GetEligibleEmployeesResponseDtoMap>();

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Number read on FROZEN");
        csvWriter.WriteField(responseWithTotals.NumberReadOnFrozen);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Number not selected");
        csvWriter.WriteField(responseWithTotals.NumberNotSelected);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("Number written");
        csvWriter.WriteField(responseWithTotals.NumberWritten);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<GetEligibleEmployeesResponseDto>();
        await csvWriter.NextRecordAsync();

        await base.GenerateCsvContent(csvWriter, responseWithTotals, cancellationToken);
    }

    public sealed class GetEligibleEmployeesResponseDtoMap : ClassMap<GetEligibleEmployeesResponseDto>
    {
        public GetEligibleEmployeesResponseDtoMap()
        {
            Map(m => m.OracleHcmId).Index(1).Name("ORACLE_HCM_ID");
            Map(m => m.BadgeNumber).Index(2).Name("BADGE_PSN");
            Map(m => m.FullName).Index(3).Name("NAME");
        }

    }
}
