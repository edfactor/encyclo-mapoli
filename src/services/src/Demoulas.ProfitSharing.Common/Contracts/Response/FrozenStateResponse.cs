using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

[NoMemberDataExposed]
public sealed record FrozenStateResponse : IdRequest<int>, IProfitYearRequest
{
    public short ProfitYear { get; set; }
    public string? FrozenBy { get; set; }
    public DateTimeOffset AsOfDateTime { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public bool IsActive { get; set; }

    public static FrozenStateResponse ResponseExample()
    {
        return new FrozenStateResponse
        {
            Id = 1,
            ProfitYear = 2024,
            FrozenBy = "admin",
            AsOfDateTime = DateTimeOffset.UtcNow,
            CreatedDateTime = DateTimeOffset.UtcNow,
            IsActive = true
        };
    }
}
