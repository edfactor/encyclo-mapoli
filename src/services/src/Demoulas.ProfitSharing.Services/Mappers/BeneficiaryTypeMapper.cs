using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class BeneficiaryTypeMapper
{
    public partial BeneficiaryTypeResponseDto Map(BeneficiaryType source);
    public partial BeneficiaryType Map(BeneficiaryTypeRequestDto source);
}
