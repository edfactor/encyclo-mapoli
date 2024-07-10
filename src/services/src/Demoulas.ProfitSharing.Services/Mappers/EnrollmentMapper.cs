using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class EnrollmentMapper
{
    public partial EnrollmentResponseDto Map(Enrollment source);
    public partial Enrollment Map(EnrollmentRequestDto source);

    public partial EnrollmentRequestDto MapToAddressRequestDto(Enrollment source);
}
