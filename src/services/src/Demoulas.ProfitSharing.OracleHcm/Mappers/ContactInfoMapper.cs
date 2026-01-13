using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.OracleHcm.Mappers;

[Mapper]
public partial class ContactInfoMapper
{
    public partial ContactInfoResponseDto Map(ContactInfo source);
    public partial ContactInfo Map(ContactInfoRequestDto source);
}
