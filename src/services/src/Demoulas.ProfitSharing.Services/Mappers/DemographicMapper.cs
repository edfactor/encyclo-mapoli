// DemographicMapper.cs

using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class DemographicMapper
{
    private readonly AddressMapper _addressMapper;
    private readonly ContactInfoMapper _contactInfoMapper;

    public DemographicMapper(AddressMapper addressMapper, ContactInfoMapper contactInfoMapper)
    {
        _addressMapper = addressMapper;
        _contactInfoMapper = contactInfoMapper;
    }

    public partial IEnumerable<DemographicsRequestDto> MapToRequest(IEnumerable<Demographic> sources);

    public partial IEnumerable<DemographicsResponseDto> Map(IEnumerable<Demographic> sources);

    public partial DemographicsResponseDto Map(Demographic source);

    public partial IEnumerable<Demographic> Map(IEnumerable<DemographicsRequestDto> sources);

    public Demographic Map(DemographicsRequestDto source)
    {
        return new Demographic
        {
            BadgeNumber = source.BadgeNumber,
            OracleHcmId = source.OracleHcmId,
            FullName = source.FullName,
            LastName = source.LastName,
            FirstName = source.FirstName,
            MiddleName = source.MiddleName,
            StoreNumber = source.StoreNumber,
            Department = source.Department,
            PayClassificationId = source.PayClassificationId,
            ContactInfo = _contactInfoMapper.Map(source.ContactInfo),
            Address = _addressMapper.Map(source.Address),
            DateOfBirth = source.DateOfBirth,
            FullTimeDate = source.FullTimeDate,
            HireDate = source.HireDate,
            ReHireDate = source.ReHireDate,
            TerminationCode = source.TerminationCode,
            TerminationDate = source.TerminationDate,
            EmploymentType = source.EmploymentType,
            PayFrequency = source.PayFrequency,
            Gender = source.Gender
        };
    }
}
