using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed class MasterInquiryResponseWithoutPaginationDto
{
    public EmployeeDetails? EmployeeDetails { get; init; }
    public List<MasterInquiryResponseDto> InquiryResults { get; init; } = null!;
    public int TotalRecord { get; set; }
}
