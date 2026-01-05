using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[NoMemberDataExposed]
public record GrandTotalsByStoreResponseDto
{
    public List<GrandTotalsByStoreRowDto> Rows { get; init; } = [];

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static GrandTotalsByStoreResponseDto ResponseExample()
    {
        return new GrandTotalsByStoreResponseDto
        {
            Rows = new List<GrandTotalsByStoreRowDto>
            {
                GrandTotalsByStoreRowDto.ResponseExample()
            }
        };
    }
}
