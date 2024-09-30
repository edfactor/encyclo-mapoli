using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;

public sealed record TerminatedEmployeeAndBeneficiaryResponse : ReportResponseBase<TerminatedEmployeeAndBeneficiaryDataResponseDto>
{
    public required decimal TotalVested { get; set; }
    public required decimal TotalForfeit { get; set; }
    public required decimal TotalEndingBalance { get; set; }
    public required decimal TotalBeneficiaryAllocation { get; set; }

}
