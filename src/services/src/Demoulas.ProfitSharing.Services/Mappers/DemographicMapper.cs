// DemographicMapper.cs

using System.Data.SqlTypes;
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

    public DemographicsResponseDto Map(Demographic source)
    {
        var target = new DemographicsResponseDto()
        {
            SSN = MaskSsn(source.SSN),
            OracleHcmId = source.OracleHcmId,
            FullName = source.FullName ?? throw new System.ArgumentNullException(nameof(source.FullName)),
            LastName = source.LastName,
            FirstName = source.FirstName,
            StoreNumber = source.StoreNumber,
            Department = source.Department,
            PayClassificationId = source.PayClassificationId,
            ContactInfo = _contactInfoMapper.Map(source.ContactInfo),
            DateOfBirth = source.DateOfBirth,
            HireDate = source.HireDate,
            ReHireDate = source.ReHireDate,
            EmploymentType = source.EmploymentType,
            PayFrequency = source.PayFrequency,
            Gender = source.Gender,
        };
        target.BadgeNumber = source.BadgeNumber;
        target.MiddleName = source.MiddleName;
        target.Address = _addressMapper.Map(source.Address);
        target.FullTimeDate = source.FullTimeDate;
        target.TerminationCode = source.TerminationCode;
        target.TerminationDate = source.TerminationDate;
        return target;
    }

    public partial IEnumerable<Demographic> Map(IEnumerable<DemographicsRequestDto> sources);

    public Demographic Map(DemographicsRequestDto source)
    {
        return new Demographic
        {
            SSN = source.SSN,
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

    internal static string MaskSsn(long ssn)
    {
        Span<char> ssnSpan = stackalloc char[9];
        ssn.ToString().AsSpan().CopyTo(ssnSpan[(9 - ssn.ToString().Length)..]);
        ssnSpan[..(9 - ssn.ToString().Length)].Fill('0');

        Span<char> resultSpan = stackalloc char[11];
        "XXX-XX-".AsSpan().CopyTo(resultSpan);
        ssnSpan.Slice(5, 4).CopyTo(resultSpan[7..]);

        return new string(resultSpan);
    }
}
