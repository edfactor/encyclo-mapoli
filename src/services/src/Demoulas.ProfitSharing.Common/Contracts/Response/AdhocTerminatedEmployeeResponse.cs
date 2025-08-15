using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record AdhocTerminatedEmployeeResponse
{
    public required int BadgeNumber { get; set; }
    [MaskSensitive] public required string FullName { get; set; }
    public required string Ssn { get; set; }
    public required DateOnly TerminationDate { get; set; }
    public required char? TerminationCodeId { get; set; }
}
