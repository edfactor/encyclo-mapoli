
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record AdhocBeneficiariesReportRequest(
    int MinProfitYear,
    bool IsAlsoEmployee,
    bool DetailSwitch
) : SortedPaginationRequestDto;
