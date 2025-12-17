using Bogus;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class DemographicHistoryFaker : Faker<DemographicHistory>
{
    private int _demographicHistoryid = 10000;

    internal DemographicHistoryFaker(IList<Demographic> demographicFakes)
    {
        short minYear = (short)ReferenceData.DsmMinValue.Year;
        var demographicQueue = new Queue<Demographic>(demographicFakes.Skip(1));
        Demographic? currentDemographic = demographicFakes[0];
        FinishWith((f, u) => { currentDemographic = demographicQueue.Peek(); })
            .RuleFor(dh => dh.Id, (f, o) => _demographicHistoryid++)
            .RuleFor(dh => dh.DemographicId, (f, o) => currentDemographic.Id)
            .RuleFor(dh => dh.ValidFrom, f => new DateTime(minYear, 1, 1, 0, 0, 0, DateTimeKind.Local))
            .RuleFor(dh => dh.ValidTo, f => new DateTime(2100, 12, 31, 0, 0, 0, DateTimeKind.Local))
            .RuleFor(dh => dh.OracleHcmId, f => currentDemographic.OracleHcmId)
            .RuleFor(dh => dh.BadgeNumber, f => currentDemographic.BadgeNumber)
            .RuleFor(dh => dh.StoreNumber, f => currentDemographic.StoreNumber)
            .RuleFor(dh => dh.PayClassificationId, f => currentDemographic.PayClassificationId)
            .RuleFor(dh => dh.DateOfBirth, f => currentDemographic.DateOfBirth)
            .RuleFor(dh => dh.HireDate, f => currentDemographic.HireDate)
            .RuleFor(dh => dh.ReHireDate, f => currentDemographic.ReHireDate)
            .RuleFor(dh => dh.TerminationDate, f => currentDemographic.TerminationDate)
            .RuleFor(dh => dh.DepartmentId, f => currentDemographic.DepartmentId)
            .RuleFor(dh => dh.EmploymentTypeId, f => currentDemographic.EmploymentTypeId)
            .RuleFor(dh => dh.PayFrequencyId, f => currentDemographic.PayFrequencyId)
            .RuleFor(dh => dh.TerminationCodeId, f => currentDemographic.TerminationCodeId)
            .RuleFor(dh => dh.EmploymentStatusId, f => currentDemographic.EmploymentStatusId)
            // ContactInfo fields
            .RuleFor(dh => dh.FirstName, f => currentDemographic.ContactInfo.FirstName)
            .RuleFor(dh => dh.LastName, f => currentDemographic.ContactInfo.LastName)
            .RuleFor(dh => dh.MiddleName, f => currentDemographic.ContactInfo.MiddleName)
            .RuleFor(dh => dh.PhoneNumber, f => currentDemographic.ContactInfo.PhoneNumber)
            .RuleFor(dh => dh.MobileNumber, f => currentDemographic.ContactInfo.MobileNumber)
            .RuleFor(dh => dh.EmailAddress, f => currentDemographic.ContactInfo.EmailAddress)
            // Address fields
            .RuleFor(dh => dh.Street, f => currentDemographic.Address.Street)
            .RuleFor(dh => dh.Street2, f => currentDemographic.Address.Street2)
            .RuleFor(dh => dh.City, f => currentDemographic.Address.City)
            .RuleFor(dh => dh.State, f => currentDemographic.Address.State)
            .RuleFor(dh => dh.PostalCode, f => currentDemographic.Address.PostalCode)
            .RuleFor(dh => dh.CreatedDateTime, f => DateTime.UtcNow).UseSeed(100);
    }
}
