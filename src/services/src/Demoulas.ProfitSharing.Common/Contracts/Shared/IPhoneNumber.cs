using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

public interface IPhoneNumber { [MaskSensitive] string? PhoneNumber { get; } }
