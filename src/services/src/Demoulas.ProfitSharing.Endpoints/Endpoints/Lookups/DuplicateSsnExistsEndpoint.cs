using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public class DuplicateSsnExistsEndpoint : ProfitSharingResponseEndpoint<bool>
{
    private readonly IPayrollDuplicateSsnReportService _duplicateSsnReportService;

    public DuplicateSsnExistsEndpoint(IPayrollDuplicateSsnReportService duplicateSsnReportService) : base(Navigation.Constants.Inquiries)
    {
        _duplicateSsnReportService = duplicateSsnReportService;
    }

    public override void Configure()
    {
        Get("duplicate-ssns/exists");
        Summary(s =>
        {
            s.Summary = "Returns true when there are duplicate SSNs present in demographics";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, true }
            };
        });
        Group<LookupGroup>();
    }

    public override Task<bool> ExecuteAsync(CancellationToken ct)
    {
        return _duplicateSsnReportService.DuplicateSsnExistsAsync(ct);
    }
}
