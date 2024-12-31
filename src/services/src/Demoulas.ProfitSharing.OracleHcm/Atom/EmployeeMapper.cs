using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public class EmployeeMapper
{
    public DemographicsRequest MapFromAtomFeed(DeltaContext record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return new DemographicsRequest
        {
            OracleHcmId = record.PersonId,
            Ssn = 0,
            StoreNumber = 0,
            DepartmentId = 0,
            PayClassificationId = 0,
            DateOfBirth = default,
            PayFrequencyId = 0,
            GenderCode = '\0',
            EmploymentStatusId = '\0',
            ContactInfo = new ContactInfoRequestDto { LastName = "null", FirstName = "null" },
            Address = new AddressRequestDto { Street = "null" },
            HireDate = default,
            EmploymentTypeCode = '\0'
        };
    }
}
