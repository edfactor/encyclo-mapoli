using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

public interface IFullNameProperty { [MaskSensitive] string? FullName { get; } }
