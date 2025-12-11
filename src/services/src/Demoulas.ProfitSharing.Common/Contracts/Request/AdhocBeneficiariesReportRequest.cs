
namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record AdhocBeneficiariesReportRequest(
    bool IsAlsoEmployee
) : ProfitYearRequest;
