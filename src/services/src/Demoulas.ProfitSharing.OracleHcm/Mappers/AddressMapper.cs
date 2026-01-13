using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.OracleHcm.Mappers;

[Mapper]
public partial class AddressMapper
{
    public partial Address Map(AddressRequestDto source);
}
