using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
[UseStaticMapper<EnrollmentMapper>]
[UseStaticMapper<EmployeeTypeMapper>]
[UseStaticMapper<BeneficiaryTypeMapper>]
[UseStaticMapper<ZeroContributionReasonMapper>]
public partial class PayProfitMapper
{
    public partial PayProfitResponseDto Map(PayProfit source);

    public partial IEnumerable<PayProfit> Map(IEnumerable<PayProfitRequestDto> source);

    public partial IEnumerable<PayProfitResponseDto> Map(IEnumerable<PayProfit> source);
    public partial PayProfit Map(PayProfitRequestDto source);
}
