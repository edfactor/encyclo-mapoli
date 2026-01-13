using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

public interface IFirstName { [MaskSensitive] string FirstName { get; } }
