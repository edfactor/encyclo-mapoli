using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
[UseStaticMapper<EnrollmentMapper>]
public partial class PayProfitMapper
{
    public partial EnrollmentResponseDto Map(PayProfit source);
    public partial PayProfit Map(EnrollmentRequestDto source);

    public partial EnrollmentRequestDto MapToAddressRequestDto(PayProfit source);
}
