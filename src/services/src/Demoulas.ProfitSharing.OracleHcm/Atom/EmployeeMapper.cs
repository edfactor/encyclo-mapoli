using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public class EmployeeMapper
{
    public DemographicsRequest MapFromAtomFeed(AtomFeedRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return new DemographicsRequest
        {
            OracleHcmId = record.PersonId,
            EmployeeId = record.EmployeeId,
            ContactInfo = new ContactInfoRequestDto { FirstName = record.FirstName, LastName = record.LastName, MiddleName = record.MiddleName, },
            Address = new AddressRequestDto
            {
                Street = record.AddressLine1 ?? string.Empty,
                Street2 = record.AddressLine2,
                City = record.City ?? string.Empty,
                State = record.State ?? string.Empty,
                PostalCode = record.ZipCode ?? string.Empty
            },
            EmploymentTypeCode = record.EmploymentType,
            HireDate = record.HireDate,
            TerminationDate = record.TerminationDate,
            Ssn = 0,
            StoreNumber = 0,
            DepartmentId = 0,
            PayClassificationId = 0,
            DateOfBirth = default,
            PayFrequencyId = 0,
            GenderCode = '\0',
            EmploymentStatusId = '\0'
        };
    }
}
