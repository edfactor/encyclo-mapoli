using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[NoMemberDataExposed]
public record GrandTotalsByStoreResponseDto
{
    public List<GrandTotalsByStoreRowDto> Rows { get; init; } = [];
}
