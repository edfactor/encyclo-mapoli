using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.OracleHcm.Mappers;

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
            DateOfDeath = source.DateOfDeath,
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
