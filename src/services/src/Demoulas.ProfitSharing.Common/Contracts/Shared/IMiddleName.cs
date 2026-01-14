using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

public interface IMiddleName { [MaskSensitive] string? MiddleName { get; } }
