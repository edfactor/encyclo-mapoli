using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

/**
 * A collect of "stock" objects suitable for use in testing.
 *
 * Things like a "stock employee" or a "typical beneficiary."  Objects are always created here, so tests do not accidentally cross update each other
 */
public static class StockFactory
{
    private static int _id = 42;

    public static Beneficiary CreateBeneficiary()
    {
        return new Beneficiary
        {
            Id = _id++,
            PsnSuffix = 721,
            BadgeNumber = 1,
            Contact = new BeneficiaryContact
            {
                Address = new Address { Street = "1 Main Street" },
                ContactInfo = new ContactInfo
                {
                    FullName = "Benny, Ben",
                    LastName = "Ben",
                    FirstName = "Benny",
                    MiddleName = "",
                    PhoneNumber = ""
                },
                Id = 0,
                Ssn = 555,
                DateOfBirth = default,
                CreatedDate = default
            },
            DemographicId = 0,
            BeneficiaryContactId = 0,
            Percent = 0
        };
    }

    public static (Demographic demographic, List<PayProfit> payprofit) CreateEmployee(short profitYear)
    {
        Demographic demographic = new()
        {
            Id = _id++,
            OracleHcmId = 0,
            Ssn = 444,
            BadgeNumber = 0,
            ModifiedAtUtc = default,
            StoreNumber = 0,
            PayClassification = null,
            PayClassificationId = "0",
            ContactInfo = new ContactInfo
            {
                FullName = null,
                LastName = "Lasty",
                FirstName = "Firsty",
                MiddleName = null,
                PhoneNumber = null,
                MobileNumber = null,
                EmailAddress = null
            },
            Address = new Address
            {
                Street = "22 Main",
                Street2 = null,
                Street3 = null,
                Street4 = null,
                City = null,
                State = null,
                PostalCode = null,
                CountryIso = null
            },
            DateOfBirth = default,
            FullTimeDate = null,
            HireDate = default,
            ReHireDate = null,
            TerminationDate = null,
            DepartmentId = 0,
            Department = null,
            EmploymentTypeId = '\0',
            EmploymentType = null,
            GenderId = '\0',
            Gender = null,
            PayFrequencyId = 0,
            PayFrequency = null,
            TerminationCodeId = null,
            TerminationCode = null,
            EmploymentStatusId = '\0',
            EmploymentStatus = null,
            PayProfits = [],
            Beneficiaries = [],
            Checks = [],
            DistributionRequests = []
        };

        List<PayProfit> payprofits =
        [
            new()
            {
                DemographicId = demographic.Id,
                Demographic = demographic,
                ProfitYear = profitYear, //  Calendar Service: "The date must be between January 1, 2000, and 5 years from today's date."
                CurrentHoursYear = 0,
                CurrentIncomeYear = 0,
                WeeksWorkedYear = 0,
                PsCertificateIssuedDate = null,
                EnrollmentId = Enrollment.Constants.NewVestingPlanHasContributions,
                Enrollment = new Enrollment { Id = Enrollment.Constants.NewVestingPlanHasContributions, Name = "" },
                BeneficiaryTypeId = 0,
                BeneficiaryType = null,
                EmployeeTypeId = 0,
                EmployeeType = null,
                ZeroContributionReasonId = null,
                ZeroContributionReason = null,
                HoursExecutive = 0,
                IncomeExecutive = 0,
                ModifiedAtUtc = default,
                PointsEarned = null,
                Etva = 0
            },
            new()
            {
                DemographicId = demographic.Id,
                Demographic = demographic,
                ProfitYear = (short)(profitYear + 1),
                CurrentHoursYear = 0,
                CurrentIncomeYear = 0,
                WeeksWorkedYear = 0,
                PsCertificateIssuedDate = null,
                EnrollmentId = Enrollment.Constants.NewVestingPlanHasContributions,
                Enrollment = new Enrollment { Id = Enrollment.Constants.NewVestingPlanHasContributions, Name = "" },
                BeneficiaryTypeId = 0,
                BeneficiaryType = null,
                EmployeeTypeId = 0,
                EmployeeType = null,
                ZeroContributionReasonId = null,
                ZeroContributionReason = null,
                HoursExecutive = 0,
                IncomeExecutive = 0,
                ModifiedAtUtc = default,
                PointsEarned = null,
                Etva = 0
            }
        ];
        return (demographic, payprofits);
    }

    public static List<YearEndUpdateStatus> CreateYearEndUpdateStatuses(short profitYear)
    {
        return
        [
            new YearEndUpdateStatus
            {
                ProfitYear = profitYear,
                CreatedAtUtc = default,
                UserName = "someone",
                BeneficiariesEffected = 0,
                EmployeesEffected = 0,
                EtvasEffected = 0,
                ContributionPercent = 0,
                IncomingForfeitPercent = 0,
                EarningsPercent = 0,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 0,
                BadgeAdjusted = 0,
                BadgeAdjusted2 = 0,
                AdjustContributionAmount = 0,
                AdjustEarningsAmount = 0,
                AdjustIncomingForfeitAmount = 0,
                AdjustEarningsSecondaryAmount = 0,
                IsYearEndCompleted = false,
            }
        ];
    }

    public static ProfitDetail CreateAllocation(short profitYear, int ssn, decimal amount)
    {
        return new ProfitDetail { ProfitYear = profitYear, Ssn = ssn, ProfitCodeId = /*6*/ ProfitCode.Constants.IncomingQdroBeneficiary, Contribution = amount };
    }
}
