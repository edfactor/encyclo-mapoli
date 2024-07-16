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
    private readonly DepartmentMapper _departmentMapper;
    private readonly EmploymentTypeMapper _employmentTypeMapper;
    private readonly PayFrequencyMapper _payFrequencyMapper;
    private readonly GenderMapper _genderMapper;
    private readonly TerminationCodeMapper _terminationCodeMapper;

    public DemographicMapper(AddressMapper addressMapper, 
        ContactInfoMapper contactInfoMapper, 
        DepartmentMapper departmentMapper, 
        EmploymentTypeMapper employmentTypeMapper,
        PayFrequencyMapper payFrequencyMapper,
        GenderMapper genderMapper,
        TerminationCodeMapper terminationCodeMapper)
    {
        _addressMapper = addressMapper;
        _contactInfoMapper = contactInfoMapper;
        _departmentMapper = departmentMapper;
        _employmentTypeMapper = employmentTypeMapper;
        _payFrequencyMapper = payFrequencyMapper;
        _genderMapper = genderMapper;
        _terminationCodeMapper = terminationCodeMapper;
    }

    public partial IEnumerable<DemographicsRequestDto> MapToRequest(IEnumerable<Demographic> sources);

    public DemographicsRequestDto MapToRequest(Demographic source)
    {
        DemographicsRequestDto target = new DemographicsRequestDto
        {
            SSN = source.SSN,
            OracleHcmId = source.OracleHcmId,
            LastName = source.LastName,
            FirstName = source.FirstName,
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
            FullName = source.FullName,
            MiddleName = source.MiddleName,
            FullTimeDate = source.FullTimeDate,
            TerminationDate = source.TerminationDate
        };
        return target;
    }

    public partial IEnumerable<DemographicsResponseDto> Map(IEnumerable<Demographic> sources);

    public DemographicsResponseDto Map(Demographic source)
    {
        DemographicsResponseDto target = new DemographicsResponseDto
        {
            SSN = MaskSsn(source.SSN),
            OracleHcmId = source.OracleHcmId,
            FullName = source.FullName ?? throw new System.ArgumentNullException(nameof(source.FullName)),
            LastName = source.LastName,
            FirstName = source.FirstName,
            StoreNumber = source.StoreNumber,
            Department = _departmentMapper.Map(source.Department),
            PayClassificationId = source.PayClassificationId,
            ContactInfo = _contactInfoMapper.Map(source.ContactInfo),
            DateOfBirth = source.DateOfBirth,
            HireDate = source.HireDate,
            ReHireDate = source.ReHireDate,
            EmploymentType = _employmentTypeMapper.Map(source.EmploymentType),
            PayFrequency = _payFrequencyMapper.Map(source.PayFrequency),
            Gender = _genderMapper.Map(source.Gender),
        };
        target.BadgeNumber = source.BadgeNumber;
        target.MiddleName = source.MiddleName;
        target.Address = _addressMapper.Map(source.Address);
        target.FullTimeDate = source.FullTimeDate;
        target.TerminationCode = _terminationCodeMapper.Map(source.TerminationCode);
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
