using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Extensions;

namespace Demoulas.ProfitSharing.OracleHcm.Messaging;

public static class DemographicsRequestExtension
{
    public static DemographicsRequest CreateDemographicsRequest(this OracleEmployee employee, Dictionary<long, int> fakeSsnLookup)
    {
        return new DemographicsRequest
        {
            OracleHcmId = employee.PersonId,
            BadgeNumber = employee.BadgeNumber,
            DateOfBirth = employee.DateOfBirth,
            HireDate = employee.WorkRelationship?.StartDate ?? ReferenceData.DsmMinValue,
            TerminationDate = employee.WorkRelationship?.TerminationDate,
            Ssn = employee.NationalIdentifier?.NationalIdentifierNumber != null
                ? employee.NationalIdentifier.NationalIdentifierNumber.ConvertSsnToInt()
                : (fakeSsnLookup.TryGetValue(employee.PersonId, out var fakeSsn) ? fakeSsn : 0),
            StoreNumber = employee.WorkRelationship?.Assignment.LocationCode ?? 0,
            DepartmentId = employee.WorkRelationship?.Assignment.GetDepartmentId() ?? 0,
            // PayClassificationId is a string in DemographicsRequest; use JobCode or default "0"
            PayClassificationId = employee.WorkRelationship?.Assignment.JobCode ?? "0",
            EmploymentTypeCode = employee.WorkRelationship?.Assignment.GetEmploymentType() ?? char.MinValue,
            PayFrequencyId = employee.WorkRelationship?.Assignment.GetPayFrequency() ?? byte.MinValue,
            EmploymentStatusId = employee.WorkRelationship?.TerminationDate == null ? EmploymentStatus.Constants.Active : EmploymentStatus.Constants.Terminated,
            GenderCode = employee.LegislativeInfoItem?.Gender switch
            {
                "M" => Gender.Constants.Male,
                "F" => Gender.Constants.Female,
                "ORA_HRX_X" => Gender.Constants.Nonbinary,
                _ => Gender.Constants.Unknown
            },
            ContactInfo = new ContactInfoRequestDto
            {
                FirstName = employee.Name.FirstName,
                MiddleName = employee.Name.MiddleNames,
                LastName = employee.Name.LastName,
                FullName = DtoCommonExtensions.ComputeFullNameWithInitial(employee.Name.LastName, employee.Name.FirstName, employee.Name.MiddleNames),
                PhoneNumber = employee.Phone?.PhoneNumber,
                EmailAddress = employee.Email?.EmailAddress
            },
            Address = new AddressRequestDto
            {
                Street = employee.Address!.AddressLine1,
                Street2 = employee.Address.AddressLine2,
                Street3 = employee.Address.AddressLine3,
                Street4 = employee.Address.AddressLine4,
                City = employee.Address.TownOrCity,
                State = employee.Address.State,
                PostalCode = employee.Address.PostalCode,
                CountryIso = employee.Address.Country
            }
        };
    }
}
