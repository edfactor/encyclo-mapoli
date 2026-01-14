using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IAdhocBeneficiariesReport
{
    Task<AdhocBeneficiariesReportResponse> GetAdhocBeneficiariesReportAsync(
        AdhocBeneficiariesReportRequest req,
        CancellationToken cancellationToken = default);
}
