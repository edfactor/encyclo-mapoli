using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record MasterInquiryWithDetailsResponseDto
{
    public MemberDetails? EmployeeDetails { get; init; }
    public PaginatedResponseDto<MasterInquiryResponseDto> InquiryResults { get; init; } = null!;

    public static MasterInquiryWithDetailsResponseDto ResponseExample()
    {
        return new MasterInquiryWithDetailsResponseDto
        {
            EmployeeDetails = null,
            InquiryResults = new PaginatedResponseDto<MasterInquiryResponseDto>
            {
                Results = new List<MasterInquiryResponseDto> { MasterInquiryResponseDto.ResponseExample() }
            }
        };
    }
}
