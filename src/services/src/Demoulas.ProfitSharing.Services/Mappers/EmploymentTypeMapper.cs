// DemographicMapper.cs

using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class EmploymentTypeMapper
{
    public partial EmploymentTypeResponseDto? Map(EmploymentType? source);

}
