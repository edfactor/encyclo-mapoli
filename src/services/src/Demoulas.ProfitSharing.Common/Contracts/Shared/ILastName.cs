using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

public interface ILastName { [MaskSensitive] string LastName { get; } }
