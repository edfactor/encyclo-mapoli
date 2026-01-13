
namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record AdhocBeneficiariesReportRequest(
    bool IsAlsoEmployee
) : ProfitYearRequest
{
    public static new AdhocBeneficiariesReportRequest RequestExample()
    {
        return new AdhocBeneficiariesReportRequest(IsAlsoEmployee: true)
        {
            ProfitYear = 2024
        };
    }
}
