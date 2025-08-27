using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Shared;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record AdhocTerminatedEmployeeResponse : IFullNameProperty
{
    public required int BadgeNumber { get; set; }
    // FullName provided by upstream query; expose via interface. Keep set for backward compatibility.
    [MaskSensitive] public required string FullName { get; set; }
    [MaskSensitive] public string FirstName { get; set; } = string.Empty;
    [MaskSensitive] public string LastName { get; set; } = string.Empty;
    [MaskSensitive] public string MiddleInitial { get; set; } = string.Empty;
    public required string Ssn { get; set; }
    public required DateOnly TerminationDate { get; set; }
    public required char? TerminationCodeId { get; set; }
    [MaskSensitive] public string Address { get; set; } = string.Empty;
    [MaskSensitive ]public string Address2 { get; set; } = string.Empty;
    [MaskSensitive] public string City { get; set; } = string.Empty;
    [MaskSensitive] public string State { get; set; } = string.Empty;
    [MaskSensitive] public string PostalCode { get; set; } = string.Empty;
    public bool IsExecutive { get; set; }
}
