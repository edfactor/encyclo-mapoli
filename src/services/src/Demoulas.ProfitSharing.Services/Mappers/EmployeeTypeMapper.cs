using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class EmployeeTypeMapper
{
    public partial EmployeeTypeResponseDto Map(EmployeeType source);
    public partial EmployeeType Map(EmployeeTypeRequestDto source);

    public partial EmployeeTypeRequestDto MapToAddressRequestDto(EmployeeType source);
}
