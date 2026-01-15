using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Demoulas.ProfitSharing.UnitTests.Common.Helpers;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Demoulas.Util.Extensions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Services;

public class DemographicsServiceTests
{
    public DemographicsServiceTests()
    {
        // Initialize telemetry metrics for testing
        EndpointTelemetry.Initialize();
    }

    private static int _demographicIdCounter = 1;
    private static Mock<DbSet<T>> BuildMockDbSetWithBackingList<T>(List<T> data) where T : class
    {
        // Start with IQueryable-enabled DbSet backed by the list
        var mockSet = data.BuildMockDbSet();

        // Ensure Add/Remove operations mutate the backing list
        mockSet.Setup(s => s.Add(It.IsAny<T>()))
            .Callback<T>(e =>
            {
                // Simulate auto-increment ID for Demographic entities
                if (e is Demographic demo && demo.Id == 0)
                {
                    demo.Id = _demographicIdCounter++;
                }
                data.Add(e);
            });
        mockSet.Setup(s => s.AddRange(It.IsAny<IEnumerable<T>>()))
            .Callback<IEnumerable<T>>(range =>
            {
                foreach (var e in range)
                {
                    // Simulate auto-increment ID for Demographic entities
                    if (e is Demographic demo && demo.Id == 0)
                    {
                        demo.Id = _demographicIdCounter++;
                    }
                    data.Add(e);
                }
            });
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
            .Callback<T, CancellationToken>((e, _) =>
            {
                // Simulate auto-increment ID for Demographic entities
                if (e is Demographic demo && demo.Id == 0)
                {
                    demo.Id = _demographicIdCounter++;
                }
                data.Add(e);
            })
            .Returns<T, CancellationToken>((e, _) =>
                ValueTask.FromResult((EntityEntry<T>)null!));

        return mockSet;
    }

    [Fact]
    [Description("PS-0000 : Audit error handling - verify DemographicSyncAudit records are added and saved")]
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
        var fakeSsnService = new Mock<IFakeSsnService>();
        var repositoryMock = new Mock<IDemographicsRepository>();
        var matchingServiceMock = new Mock<IDemographicMatchingService>();
        var auditServiceMock = new Mock<IDemographicAuditService>();
        var historyServiceMock = new Mock<IDemographicHistoryService>();

        var service = new DemographicsService(
            scenarioFactory,
            matchingServiceMock.Object,
            auditServiceMock.Object,
            historyServiceMock.Object,
            mapper,
            loggerMock.Object,
            fakeSsnService.Object
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
    [Description("PS-0000 : Insert new demographic entities - verify new records are added to database")]
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
                PayClassificationId = "1",
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
        var fakeSsnService = new Mock<IFakeSsnService>();

        var repositoryMock = new Mock<IDemographicsRepository>();
        var matchingServiceMock = new Mock<IDemographicMatchingService>();
        var auditServiceMock = new Mock<IDemographicAuditService>();
        var historyServiceMock = new Mock<IDemographicHistoryService>();

        // Setup matching service mocks
        matchingServiceMock
            .Setup(m => m.MatchByOracleIdAsync(It.IsAny<Dictionary<long, Demographic>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Demographic>());

        matchingServiceMock
            .Setup(m => m.MatchByFallbackAsync(It.IsAny<List<(int, int)>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Demographic>(), false));

        matchingServiceMock
            .Setup(m => m.IdentifyNewDemographics(It.IsAny<List<Demographic>>(), It.IsAny<List<Demographic>>()))
            .Returns((List<Demographic> incoming, List<Demographic> existing) => incoming);

        // Setup audit service mocks
        auditServiceMock
            .Setup(m => m.DetectDuplicateSsns(It.IsAny<List<Demographic>>()))
            .Returns(new List<IGrouping<int, Demographic>>());

        auditServiceMock
            .Setup(m => m.PrepareAuditDuplicateSsns(It.IsAny<List<IGrouping<int, Demographic>>>()))
            .Returns(new List<Demoulas.ProfitSharing.OracleHcm.Commands.IDemographicCommand>());

        auditServiceMock
            .Setup(m => m.PrepareCheckSsnConflicts(It.IsAny<List<Demographic>>(), It.IsAny<List<Demographic>>()))
            .Returns(new List<Demoulas.ProfitSharing.OracleHcm.Commands.IDemographicCommand>());

        // Setup history service mocks - using actual command types to allow real execution
        historyServiceMock
            .Setup(m => m.PrepareInsertNewWithHistory(It.IsAny<List<Demographic>>()))
            .Returns<List<Demographic>>(newDemographics =>
            {
                var commands = new List<Demoulas.ProfitSharing.OracleHcm.Commands.IDemographicCommand>();

                // Create real commands that will actually add to the context
                foreach (var demographic in newDemographics)
                {
                    commands.Add(new Demoulas.ProfitSharing.OracleHcm.Commands.AddDemographicCommand(demographic));

                    var history = new DemographicHistory
                    {
                        OracleHcmId = demographic.OracleHcmId,
                        BadgeNumber = demographic.BadgeNumber,
                        DateOfBirth = demographic.DateOfBirth,
                        StoreNumber = demographic.StoreNumber,
                        DepartmentId = demographic.DepartmentId,
                        PayClassificationId = demographic.PayClassificationId,
                        HireDate = demographic.HireDate,
                        EmploymentTypeId = demographic.EmploymentTypeId,
                        PayFrequencyId = demographic.PayFrequencyId,
                        EmploymentStatusId = demographic.EmploymentStatusId,
                        VestingScheduleId = demographic.VestingScheduleId,
                        HasForfeited = demographic.HasForfeited,
                        ValidFrom = DateTimeOffset.UtcNow,
                        ValidTo = new DateTimeOffset(new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    };
                    commands.Add(new Demoulas.ProfitSharing.OracleHcm.Commands.AddHistoryCommand(history));
                }

                return (newDemographics.Count, commands);
            });

        historyServiceMock
            .Setup(m => m.PrepareUpdateExistingWithHistory(It.IsAny<List<Demographic>>(), It.IsAny<List<Demographic>>()))
            .Returns((0, new List<Demoulas.ProfitSharing.OracleHcm.Commands.IDemographicCommand>()));

        historyServiceMock
            .Setup(m => m.DetectSsnChanges(It.IsAny<List<Demographic>>(), It.IsAny<Dictionary<long, Demographic>>()))
            .Returns(new List<Demographic>());

        historyServiceMock
            .Setup(m => m.PrepareSsnUpdateCommands(It.IsAny<List<Demographic>>(), It.IsAny<Dictionary<long, Demographic>>()))
            .Returns(new List<Demoulas.ProfitSharing.OracleHcm.Commands.IDemographicCommand>()); var service = new DemographicsService(
            scenarioFactory,
            matchingServiceMock.Object,
            auditServiceMock.Object,
            historyServiceMock.Object,
            mapper,
            loggerMock.Object,
            fakeSsnService.Object
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

    private static ScenarioDataContextFactory SetupScenarioFactoryWithSeededDemographics(out List<Demographic> demographics, out List<DemographicHistory> histories, out List<DemographicSyncAudit> audits, out List<BeneficiaryContact> beneficiaryContacts, out List<ProfitDetail> profitDetails)
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
        demographics[0].PayClassificationId = "1";
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
        demographics[1].PayClassificationId = "1";
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

        // beneficiary contacts setup
        beneficiaryContacts = new List<BeneficiaryContact>();
        beneficiaryContacts.AddRange(

            new BeneficiaryContact
            {
                Id = 1,
                Address = new Address
                {
                    Street = "789 Oak St",
                    City = "BeneficiaryCity",
                    State = "BS",
                    PostalCode = "67890",
                },
                Ssn = demographics[0].Ssn,
                ContactInfo = new ContactInfo
                {
                    PhoneNumber = "0987654321",
                    FirstName = "Existing",
                    LastName = "User",
                    FullName = "User, Existing",
                    MiddleName = "E"
                },
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-50)),
                CreatedDate = DateTime.UtcNow.AddYears(-1).ToDateOnly()
            },
            new BeneficiaryContact
            {
                Id = 2,
                Address = new Address
                {
                    Street = "789 Oak St",
                    City = "BeneficiaryCity",
                    State = "BS",
                    PostalCode = "67890",
                },
                Ssn = demographics[1].Ssn,
                ContactInfo = new ContactInfo
                {
                    PhoneNumber = "0987654321",
                    FirstName = "Existing",
                    LastName = "User",
                    FullName = "User, Existing",
                    MiddleName = "E"
                },
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-50)),
                CreatedDate = DateTime.UtcNow.AddYears(-1).ToDateOnly()
            }
        );
        // profit details setup
        profitDetails = new List<ProfitDetail>()
        {
            new ProfitDetail
            {
              ProfitCodeId =1,
              Ssn = demographics[0].Ssn,
              Earnings = 1000.00M,
            },
            new ProfitDetail
            {
              ProfitCodeId =1,
              Ssn = demographics[0].Ssn,
              Earnings = 1000.00M,
            }
        };

        var scenarioFactory = new ScenarioDataContextFactory();
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.Demographics).Returns(BuildMockDbSetWithBackingList(demographics).Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.DemographicHistories).Returns(BuildMockDbSetWithBackingList(histories).Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.DemographicSyncAudit).Returns(BuildMockDbSetWithBackingList(audits).Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.BeneficiaryContacts).Returns(BuildMockDbSetWithBackingList(beneficiaryContacts).Object);
        scenarioFactory.ProfitSharingDbContext.Setup(c => c.ProfitDetails).Returns(BuildMockDbSetWithBackingList(profitDetails).Object);

        scenarioFactory.ProfitSharingDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Exercise the new OracleEmployee factory (produces OracleEmployee[] from faker)
        _ = OracleEmployeeFactory.Generate(demographics.Count);

        return scenarioFactory;
    }


    /// <summary>
    /// UC - If SSN matches but DateOfBirth does not match, and existing employee is terminated,
    /// </summary>
    /// <returns></returns>

    [Fact]
    [Description("PS-0000 : Merge profit details - verify profit details are merged from source to target demographic")]
    public async Task MergeProfitDetailsToDemographic_WithValidSourceAndTarget_MergesAndAudits()
    {
        // Arrange
        var scenarioFactory = SetupScenarioFactoryWithSeededDemographics(out var demographics, out var histories, out var audits, out var beneficiaryContacts, out var profitDetails);

        var sourceDemographic = new Demographic
        {
            OracleHcmId = 3,
            Ssn = 333333333,
            BadgeNumber = 103,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            StoreNumber = 1,
            DepartmentId = 1,
            PayClassificationId = "1",
            HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
            EmploymentTypeId = 'F',
            PayFrequencyId = 1,
            EmploymentStatusId = 'A',
            ContactInfo = new ContactInfo
            {
                PhoneNumber = "0987654321",
                FirstName = "Source",
                LastName = "User",
                FullName = "User, Source",
                MiddleName = "S",
            },
            Address = new Address
            {
                Street = "789 Pine St",
                City = "SourceCity",
                State = "SC",
                PostalCode = "54321",
            }
        };

        var targetDemographic = demographics[0]; // Use first seeded demographic as target

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());
        var fakeSsnService = new Mock<IFakeSsnService>();

        // Setup fake SSN service to return a specific value for testing
        const int fakeSsnValue = 999999999;
        fakeSsnService.Setup(f => f.GenerateFakeSsnAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeSsnValue);

        var repositoryMock = new Mock<IDemographicsRepository>();
        var matchingServiceMock = new Mock<IDemographicMatchingService>();
        var auditServiceMock = new Mock<IDemographicAuditService>();
        var historyServiceMock = new Mock<IDemographicHistoryService>();

        var service = new DemographicsService(
            scenarioFactory,
            matchingServiceMock.Object,
            auditServiceMock.Object,
            historyServiceMock.Object,
            mapper,
            loggerMock.Object,
            fakeSsnService.Object
        );

        // Act
        await service.MergeProfitDetailsToDemographic(sourceDemographic, targetDemographic, CancellationToken.None);

        // Assert
        // Verify source demographic SSN was changed to fake SSN
        Assert.Equal(fakeSsnValue, sourceDemographic.Ssn);

        // Verify audit record was created
        var mergeAudit = audits.FirstOrDefault(a =>
            a.OracleHcmId == targetDemographic.OracleHcmId &&
            a.Message.Contains($"Merging ProfitDetails from source OracleHcmId {sourceDemographic.OracleHcmId}"));
        Assert.NotNull(mergeAudit);
        Assert.Equal("ProfitDetails", mergeAudit.PropertyName);
        Assert.Equal(targetDemographic.BadgeNumber, mergeAudit.BadgeNumber);

        // Verify fake SSN service was called
        fakeSsnService.Verify(f => f.GenerateFakeSsnAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Description("PS-0000 : Merge profit details save failure - verify critical error is logged when save changes fails")]
    public async Task MergeProfitDetailsToDemographic_WhenSaveChangesFails_LogsCriticalError()
    {
        // Arrange
        var scenarioFactory = SetupScenarioFactoryWithSeededDemographics(out var demographics, out var histories, out var audits, out var beneficiaryContacts, out var profitDetails);

        // Setup scenario factory to throw on SaveChanges
        scenarioFactory.ProfitSharingDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        var sourceDemographic = new Demographic
        {
            OracleHcmId = 3,
            Ssn = 333333333,
            BadgeNumber = 103,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            StoreNumber = 1,
            DepartmentId = 1,
            PayClassificationId = "1",
            HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
            EmploymentTypeId = 'F',
            PayFrequencyId = 1,
            EmploymentStatusId = 'A',
            ContactInfo = new ContactInfo
            {
                PhoneNumber = "0987654321",
                FirstName = "Source",
                LastName = "User",
                FullName = "User, Source",
                MiddleName = "S",
            },
            Address = new Address
            {
                Street = "789 Pine St",
                City = "SourceCity",
                State = "SC",
                PostalCode = "54321",
            }
        };

        var targetDemographic = demographics[0];

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());
        var fakeSsnService = new Mock<IFakeSsnService>();

        fakeSsnService.Setup(f => f.GenerateFakeSsnAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(999999999);

        var repositoryMock = new Mock<IDemographicsRepository>();
        var matchingServiceMock = new Mock<IDemographicMatchingService>();
        var auditServiceMock = new Mock<IDemographicAuditService>();
        var historyServiceMock = new Mock<IDemographicHistoryService>();

        var service = new DemographicsService(
            scenarioFactory,
            matchingServiceMock.Object,
            auditServiceMock.Object,
            historyServiceMock.Object,
            mapper,
            loggerMock.Object,
            fakeSsnService.Object
        );

        // Act
        await service.MergeProfitDetailsToDemographic(sourceDemographic, targetDemographic, CancellationToken.None);

        // Assert
        // Verify critical error was logged
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
