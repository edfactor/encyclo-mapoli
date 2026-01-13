using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record AdhocTerminatedEmployeeResponse : IFullNameProperty, IIsExecutive
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
    public required string? TerminationCode { get; set; }
    [MaskSensitive] public string Address { get; set; } = string.Empty;
    [MaskSensitive] public string Address2 { get; set; } = string.Empty;
    [MaskSensitive] public string City { get; set; } = string.Empty;
    [MaskSensitive] public string State { get; set; } = string.Empty;
    [MaskSensitive] public string PostalCode { get; set; } = string.Empty;
    public bool IsExecutive { get; set; }

    public static AdhocTerminatedEmployeeResponse ResponseExample() => new()
    {
        BadgeNumber = 12345,
        FullName = "John Michael Doe",
        FirstName = "John",
        LastName = "Doe",
        MiddleInitial = "M",
        Ssn = "***-**-6789",
        TerminationDate = new DateOnly(2024, 6, 30),
        TerminationCodeId = 'V',
        TerminationCode = "Voluntary",
        Address = "123 Main Street",
        Address2 = "Suite 200",
        City = "Boston",
        State = "MA",
        PostalCode = "02101",
        IsExecutive = false
    };
}
