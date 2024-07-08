// DemographicMapper.cs

using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class GenderMapper
{
    public partial GenderResponseDto? Map(Gender? source);

}
