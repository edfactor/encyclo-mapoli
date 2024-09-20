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
    public partial PayProfitResponseDto Map(PayProfitLegacy source);

    public partial IEnumerable<PayProfitLegacy> Map(IEnumerable<PayProfitRequestDto> source);

    public partial IEnumerable<PayProfitResponseDto> Map(IEnumerable<PayProfitLegacy> source);
    public partial PayProfitLegacy Map(PayProfitRequestDto source);
}
