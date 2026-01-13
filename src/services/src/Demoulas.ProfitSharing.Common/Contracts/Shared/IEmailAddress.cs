using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

public interface IEmailAddress { [MaskSensitive] string? EmailAddress { get; } }
