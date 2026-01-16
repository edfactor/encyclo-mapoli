using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

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

    protected override Task<bool> HandleRequestAsync(CancellationToken ct)
    {
        return _duplicateSsnReportService.DuplicateSsnExistsAsync(ct);
    }
}
