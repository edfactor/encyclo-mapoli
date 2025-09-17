using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Demoulas.ProfitSharing.OracleHcm.Messaging;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Demoulas.ProfitSharing.UnitTests.Common.Helpers;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Services;

public class DemographicsServiceTests
{
    public DemographicsServiceTests()
    {
    }

    private static Mock<DbSet<T>> BuildMockDbSetWithBackingList<T>(List<T> data) where T : class
    {
        // Start with IQueryable-enabled DbSet backed by the list
        var mockSet = data.BuildMockDbSet();

        // Ensure Add/Remove operations mutate the backing list
        mockSet.Setup(s => s.Add(It.IsAny<T>()))
            .Callback<T>(e => data.Add(e));
        mockSet.Setup(s => s.AddRange(It.IsAny<IEnumerable<T>>()))
            .Callback<IEnumerable<T>>(range => data.AddRange(range));
        mockSet.Setup(s => s.Remove(It.IsAny<T>()))
            .Callback<T>(e => data.Remove(e));
        mockSet.Setup(s => s.RemoveRange(It.IsAny<IEnumerable<T>>()))
            .Callback<IEnumerable<T>>(range =>
            {
                foreach (var e in range.ToList())
                {
                    data.Remove(e);
                }
            });

        // Async adds used by service (e.g., DemographicSyncAudit.AddAsync)
        mockSet.Setup(s => s.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback<T, CancellationToken>((e, _) => data.Add(e))
            .Returns<T, CancellationToken>((e, _) =>
                ValueTask.FromResult((EntityEntry<T>)null!));

        return mockSet;
    }

    [Fact]
    public async Task AuditError_AddsAuditRecordsAndSaves()
    {
        // Arrange
        var audits = new List<DemographicSyncAudit>();
        var mockAuditSet = BuildMockDbSetWithBackingList(audits);

        var scenarioFactory = new ScenarioDataContextFactory();
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.DemographicSyncAudit)
            .Returns(mockAuditSet.Object);
        scenarioFactory.ProfitSharingDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var badgeNumber = 123;
        var oracleHcmId = 456L;
        var errorMessages = new List<ValidationFailure>
        {
            new ValidationFailure("SSN", "Invalid SSN", "123456789")
        };
        var requestedBy = "tester";
        var cancellationToken = CancellationToken.None;

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            scenarioFactory,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object
        );

        // Act
        await service.AuditError(badgeNumber, oracleHcmId, errorMessages, requestedBy, cancellationToken, null!);

        // Assert (via backing list)
        Assert.Single(audits);
        Assert.Equal(badgeNumber, audits[0].BadgeNumber);
        Assert.Equal(oracleHcmId, audits[0].OracleHcmId);
        Assert.Equal("Invalid SSN", audits[0].Message);
        Assert.Equal("SSN", audits[0].PropertyName);
        Assert.Equal("123456789", audits[0].InvalidValue);
        Assert.Equal(requestedBy, audits[0].UserName);
    }

    [Fact]
    public async Task AddDemographicsStreamAsync_InsertsNewEntities()
    {
        // Arrange list-backed sets
        var demographics = new List<Demographic>();
        var demographicHistories = new List<DemographicHistory>();
        var audits = new List<DemographicSyncAudit>();

        var demographicsSet = BuildMockDbSetWithBackingList(demographics);
        var historiesSet = BuildMockDbSetWithBackingList(demographicHistories);
        var auditsSet = BuildMockDbSetWithBackingList(audits);

        var scenarioFactory = new ScenarioDataContextFactory();
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.Demographics).Returns(demographicsSet.Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.DemographicHistories).Returns(historiesSet.Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.DemographicSyncAudit).Returns(auditsSet.Object);
        scenarioFactory.ProfitSharingDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var employees = new[]
        {
            new DemographicsRequest
            {
                OracleHcmId = 1,
                Ssn = 111111111,
                BadgeNumber = 100,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                StoreNumber = 1,
                DepartmentId = 1,
                PayClassificationId = 1,
                HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                EmploymentTypeCode = 'F',
                PayFrequencyId = 1,
                EmploymentStatusId = 'A',
                ContactInfo = new ContactInfoRequestDto
                {
                    PhoneNumber = "1234567890",
                    FirstName = "First",
                    LastName = "Last",
                    FullName = "Last, First",
                    MiddleName = "M",
                },
                Address = new AddressRequestDto
                {
                    Street = "123 Main St",
                    City = "City",
                    State = "ST",
                },
                GenderCode = 'M'
            }
        };

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            scenarioFactory,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert via backing lists
        Assert.Single(demographics);
        Assert.Single(demographicHistories);
        Assert.Equal(111111111, demographics[0].Ssn);
        Assert.Equal(100, demographics[0].BadgeNumber);
        Assert.Equal("First", demographics[0].ContactInfo.FirstName);
        Assert.Equal("123 Main St", demographics[0].Address.Street);
    }

    private static ScenarioDataContextFactory SetupScenarioFactoryWithSeededDemographics(out List<Demographic> demographics, out List<DemographicHistory> histories, out List<DemographicSyncAudit> audits)
    {
        // Use faker to produce realistic demographics (and match OracleEmployeeExtensions expectations)
        var faker = new DemographicFaker();
        demographics = faker.Generate(2);

        // Overwrite generated values with deterministic fields expected by tests
        // First existing employee
        demographics[0].OracleHcmId = 1;
        demographics[0].Ssn = 222222222;
        demographics[0].BadgeNumber = 101;
        demographics[0].DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));
        demographics[0].StoreNumber = 1;
        demographics[0].DepartmentId = 1;
        demographics[0].PayClassificationId = 1;
        demographics[0].HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
        demographics[0].EmploymentTypeId = 'F';
        demographics[0].PayFrequencyId = 1;
        demographics[0].EmploymentStatusId = 'A';
        demographics[0].ContactInfo = new ContactInfo
        {
            PhoneNumber = "0987654321",
            FirstName = "Existing",
            LastName = "User",
            FullName = "User, Existing",
            MiddleName = "E",
        };
        demographics[0].Address = new Address
        {
            Street = "456 Elm St",
            City = "OldCity",
            State = "OS",
            PostalCode = "12345",
        };

        // Second existing employee (duplicate SSN)
        demographics[1].OracleHcmId = 2;
        demographics[1].Ssn = 222222222;
        demographics[1].BadgeNumber = 102;
        demographics[1].DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));
        demographics[1].StoreNumber = 1;
        demographics[1].DepartmentId = 1;
        demographics[1].PayClassificationId = 1;
        demographics[1].HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
        demographics[1].EmploymentTypeId = 'F';
        demographics[1].PayFrequencyId = 1;
        demographics[1].EmploymentStatusId = 'A';
        demographics[1].ContactInfo = new ContactInfo
        {
            PhoneNumber = "0987654321",
            FirstName = "Existing",
            LastName = "User",
            FullName = "User, Existing",
            MiddleName = "E",
        };
        demographics[1].Address = new Address
        {
            Street = "456 Elm St",
            City = "OldCity",
            State = "OS",
            PostalCode = "12345",
        };

        histories = new List<DemographicHistory>();
        // Create current valid history per demographic
        foreach (var d in demographics)
        {
            var h = DemographicHistory.FromDemographic(d, d.Id);
            h.ValidFrom = DateTimeOffset.UtcNow.AddYears(-1);
            h.ValidTo = DateTimeOffset.MaxValue;
            histories.Add(h);
        }

        audits = new List<DemographicSyncAudit>();

        var scenarioFactory = new ScenarioDataContextFactory();
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.Demographics).Returns(BuildMockDbSetWithBackingList(demographics).Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.DemographicHistories).Returns(BuildMockDbSetWithBackingList(histories).Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.DemographicSyncAudit).Returns(BuildMockDbSetWithBackingList(audits).Object);
        scenarioFactory.ProfitSharingDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Exercise the new OracleEmployee factory (produces OracleEmployee[] from faker)
        _ = OracleEmployeeFactory.Generate(demographics.Count);

        return scenarioFactory;
    }

    [Fact]
    public async Task AddDemographicsStreamAsync_HandlesDuplicateSsn()
    {
        // Arrange
        var scenarioFactory = SetupScenarioFactoryWithSeededDemographics(out var demographics, out var _ /*histories*/, out var audits);


        Dictionary<long, int>? fakeSsnLookup = demographics.ToDictionary(d => d.OracleHcmId, d => 222222222);

        var oracleEmployees = demographics.Select(d => d.ToOracleFromDemographic());
        var employees = oracleEmployees.Select(e => e.CreateDemographicsRequest(fakeSsnLookup)).ToArray();

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            scenarioFactory,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        Assert.True(audits.Count > 0, "Expected audit records for duplicate SSN handling.");
    }

    /// <summary>
    /// UC - If SSN matches but DateOfBirth does not match, and existing employee is terminated,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddDemographicsStreamAsync_SSNMatch_NoDobMatch()
    {
        // Arrange
        var scenarioFactory = SetupScenarioFactoryWithSeededDemographics(out var demographics, out var _ /*histories*/, out var audits);

        //// diff dob and terminated employee
        var demoWithDiffDob = new Demographic
        {
            OracleHcmId = 3,
            Ssn = 222222222,
            BadgeNumber = 101,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            StoreNumber = 1,
            DepartmentId = 1,
            PayClassificationId = 1,
            HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
            EmploymentTypeId = 'F',
            PayFrequencyId = 1,
            EmploymentStatusId = 't',
            ContactInfo = new ContactInfo
            {
                PhoneNumber = "0987654321",
                FirstName = "Existing",
                LastName = "User",
                FullName = "User, Existing",
                MiddleName = "E",
            },
            Address = new Address
            {
                Street = "456 Elm St",
                City = "OldCity",
                State = "OS",
                PostalCode = "12345",
            }
        };

        // change dob and SSN so it does not show as a match
        var test = demographics.First(d => d.Ssn == 222222222);
        test.Ssn = 33333333;
        test.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-26));

        demographics.Add(demoWithDiffDob);

        var employees = new[]
        {
            new DemographicsRequest
            {
                OracleHcmId = 1,
                Ssn = 44444444,
                BadgeNumber = 100,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-31)),
                StoreNumber = 1,
                DepartmentId = 1,
                PayClassificationId = 1,
                HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                EmploymentTypeCode = 'F',
                PayFrequencyId = 1,
                EmploymentStatusId = 'A',
                ContactInfo = new ContactInfoRequestDto
                {
                    PhoneNumber = "1234567890",
                    FirstName = "First",
                    LastName = "Last",
                    FullName = "Last, First",
                    MiddleName = "M",
                },
                Address = new AddressRequestDto
                {
                    Street = "123 Main St",
                    City = "City",
                    State = "ST",
                    PostalCode = "12345",
                },
                GenderCode = 'M'
            }
        };

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            scenarioFactory,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        Assert.True(audits.Count > 0, "Expected audit records for duplicate SSN handling.");
        // add beneficiaries and pay details
    }

    /// <summary>
    /// UC - If SSN matches but DateOfBirth does not match, and existing employee is terminated,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddDemographicsStreamAsync_SSNMatch_NoDobMatch_ExistingEmployeeTerminated_NoBalance()
    {
        // Arrange
        var scenarioFactory = SetupScenarioFactoryWithSeededDemographics(out var demographics, out var _ /*histories*/, out var audits);

        // diff dob and terminated employee
        var demoWithDiffDob = new Demographic
        {
            OracleHcmId = 3,
            Ssn = 44444444,
            BadgeNumber = 101,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            StoreNumber = 1,
            DepartmentId = 1,
            PayClassificationId = 1,
            HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
            EmploymentTypeId = 'F',
            PayFrequencyId = 1,
            EmploymentStatusId = 't',
            ContactInfo = new ContactInfo
            {
                PhoneNumber = "0987654321",
                FirstName = "Existing",
                LastName = "User",
                FullName = "User, Existing",
                MiddleName = "E",
            },
            Address = new Address
            {
                Street = "456 Elm St",
                City = "OldCity",
                State = "OS",
                PostalCode = "12345",
            }
        };

        // change dob and SSN so it does not show as a match
        var test = demographics.First(d => d.Ssn == 222222222);
        test.Ssn = 33333333;
        test.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-26));

        demographics.Add(demoWithDiffDob);

        var employees = new[]
        {
            new DemographicsRequest
            {
                OracleHcmId = 1,
                Ssn = 44444444,
                BadgeNumber = 100,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-31)),
                StoreNumber = 1,
                DepartmentId = 1,
                PayClassificationId = 1,
                HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                EmploymentTypeCode = 'F',
                PayFrequencyId = 1,
                EmploymentStatusId = 't',
                ContactInfo = new ContactInfoRequestDto
                {
                    PhoneNumber = "1234567890",
                    FirstName = "First",
                    LastName = "Last",
                    FullName = "Last, First",
                    MiddleName = "M",
                },
                Address = new AddressRequestDto
                {
                    Street = "123 Main St",
                    City = "City",
                    State = "ST",
                    PostalCode = "12345",
                },
                GenderCode = 'M'
            }
        };

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        totalServiceMock.Setup(t => t.GetVestingBalanceForSingleMemberAsync(It.IsAny<Demoulas.ProfitSharing.Common.Contracts.Request.SearchBy>(), It.IsAny<int>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Demoulas.ProfitSharing.Common.Contracts.Response.BalanceEndpointResponse
            {
                Id = 123456789,
                Ssn = "xxx-xx-6789",
                VestedBalance = 2030,
                Etva = 250,
                VestingPercent = .4m,
                CurrentBalance = 0.0M,
                YearsInPlan = 4,
                AllocationsToBeneficiary = 5,
                AllocationsFromBeneficiary = 6,
            });

        var service = new DemographicsService(
            scenarioFactory,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        Assert.True(audits.Count > 0, "Expected audit records for duplicate SSN handling.");

        // verify SSN was not changed for termed employee to fake SSN
        var termedEmployee = demographics.First(d => d.OracleHcmId == 3);
        Assert.Equal(44444444, termedEmployee.Ssn);

        // verify existing employee was updated with new SSN
        var existingEmployee = demographics.First(d => d.OracleHcmId == 1);
        Assert.Equal(44444444, existingEmployee.Ssn);

        // add beneficiaries and pay details
    }

    [Fact]
    public async Task AddDemographicsStreamAsync_SSNMatch_NoDobMatch_ExistingEmployeeTerminated_HasBalance()
    {
        // Arrange
        var scenarioFactory = SetupScenarioFactoryWithSeededDemographics(out var demographics, out var histories, out var audits);

        // diff dob and terminated employee
        var demoWithDiffDob = new Demographic
        {
            OracleHcmId = 3,
            Ssn = 222222222,
            BadgeNumber = 101,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            StoreNumber = 1,
            DepartmentId = 1,
            PayClassificationId = 1,
            HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
            EmploymentTypeId = 'F',
            PayFrequencyId = 1,
            EmploymentStatusId = 't',
            ContactInfo = new ContactInfo
            {
                PhoneNumber = "0987654321",
                FirstName = "Existing",
                LastName = "User",
                FullName = "User, Existing",
                MiddleName = "E",
            },
            Address = new Address
            {
                Street = "456 Elm St",
                City = "OldCity",
                State = "OS",
                PostalCode = "12345",
            }
        };

        // change dob and SSN so it does not show as a match
        var test = demographics.First(d => d.Ssn == 222222222);
        test.Ssn = 33333333;
        test.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-26));

        demographics.Add(demoWithDiffDob);

        var employees = new[]
        {
            new DemographicsRequest
            {
                OracleHcmId = 1,
                Ssn = 44444444,
                BadgeNumber = 100,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-31)),
                StoreNumber = 1,
                DepartmentId = 1,
                PayClassificationId = 1,
                HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                EmploymentTypeCode = 'F',
                PayFrequencyId = 1,
                EmploymentStatusId = 'A',
                ContactInfo = new ContactInfoRequestDto
                {
                    PhoneNumber = "1234567890",
                    FirstName = "First",
                    LastName = "Last",
                    FullName = "Last, First",
                    MiddleName = "M",
                },
                Address = new AddressRequestDto
                {
                    Street = "123 Main St",
                    City = "City",
                    State = "ST",
                    PostalCode = "12345",
                },
                GenderCode = 'M'
            }
        };

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        totalServiceMock.Setup(t => t.GetVestingBalanceForSingleMemberAsync(It.IsAny<Demoulas.ProfitSharing.Common.Contracts.Request.SearchBy>(), It.IsAny<int>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Demoulas.ProfitSharing.Common.Contracts.Response.BalanceEndpointResponse
            {
                Id = 123456789,
                Ssn = "xxx-xx-6789",
                VestedBalance = 2030,
                Etva = 250,
                VestingPercent = .4m,
                CurrentBalance = 5000.0M,
                YearsInPlan = 4,
                AllocationsToBeneficiary = 5,
                AllocationsFromBeneficiary = 6,
            });

        var service = new DemographicsService(
            scenarioFactory,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        Assert.True(audits.Count > 0, "Expected audit records for duplicate SSN handling.");

        // verify SSN was NOT changed for termed employee to fake SSN
        var termedEmployee = demographics.First(d => d.OracleHcmId == 3);
        Assert.Equal(222222222, termedEmployee.Ssn);

        // verify existing employee was updated with new SSN
        var existingEmployee = demographics.First(d => d.OracleHcmId == 1);
        Assert.Equal(44444444, existingEmployee.Ssn);
    }
}
