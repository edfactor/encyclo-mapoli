using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class DemographicMapper
{
    private readonly AddressMapper _addressMapper;
    private readonly ContactInfoMapper _contactInfoMapper;

    public DemographicMapper(AddressMapper addressMapper, 
        ContactInfoMapper contactInfoMapper)
    {
        _addressMapper = addressMapper;
        _contactInfoMapper = contactInfoMapper;
    }

    public partial IEnumerable<DemographicsRequest> MapToRequest(IEnumerable<Demographic> sources);

    public DemographicsRequest MapToRequest(Demographic source)
    {
        DemographicsRequest target = new DemographicsRequest
        {
            Ssn = source.Ssn,
            OracleHcmId = source.OracleHcmId,
            StoreNumber = source.StoreNumber,
            DepartmentId = source.DepartmentId,
            PayClassificationId = source.PayClassificationId,
            ContactInfo = _contactInfoMapper.MapToContactInfoRequestDto(source.ContactInfo),
            Address = _addressMapper.MapToAddressRequestDto(source.Address),
            DateOfBirth = source.DateOfBirth,
            HireDate = source.HireDate,
            ReHireDate = source.ReHireDate,
            PayFrequencyId = source.PayFrequencyId,
            EmploymentTypeCode = source.EmploymentTypeId,
            GenderCode = source.GenderId,
            TerminationCodeId = source.TerminationCodeId,
            BadgeNumber = source.BadgeNumber,
            FullTimeDate = source.FullTimeDate,
            TerminationDate = source.TerminationDate,
            EmploymentStatusId = source.EmploymentStatusId
        };
        return target;
    }

    public partial IEnumerable<Demographic> Map(IEnumerable<DemographicsRequest> sources);

    public Demographic Map(DemographicsRequest source)
    {
        return new Demographic
        {
            Ssn = source.Ssn,
            BadgeNumber = source.BadgeNumber,
            OracleHcmId = source.OracleHcmId,
            StoreNumber = source.StoreNumber,
            DepartmentId = source.DepartmentId,
            PayClassificationId = source.PayClassificationId,
            ContactInfo = _contactInfoMapper.Map(source.ContactInfo),
            Address = _addressMapper.Map(source.Address),
            DateOfBirth = source.DateOfBirth,
            FullTimeDate = source.FullTimeDate,
            HireDate = source.HireDate,
            ReHireDate = source.ReHireDate,
            TerminationCodeId = source.TerminationCodeId,
            TerminationDate = source.TerminationDate,
            EmploymentTypeId = source.EmploymentTypeCode,
            PayFrequencyId = source.PayFrequencyId,
            GenderId = source.GenderCode,
            EmploymentStatusId = source.EmploymentStatusId
        };
    }
}
