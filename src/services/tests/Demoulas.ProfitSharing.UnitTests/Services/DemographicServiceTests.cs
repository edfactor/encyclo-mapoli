using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FastEndpoints;
using FluentValidation.Results;
using Grpc.Core.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz.Impl.AdoJobStore.Common;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Services;

public class DemographicsServiceTests
{
    public DemographicsServiceTests()
    {
    }

    [Fact]
    public async Task AuditError_AddsAuditRecordsAndSaves()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ProfitSharingDbContext(options);

        var badgeNumber = 123;
        var oracleHcmId = 456L;
        var errorMessages = new List<ValidationFailure>
    {
        new ValidationFailure("SSN", "Invalid SSN", "123456789")
    };
        var requestedBy = "tester";
        var cancellationToken = CancellationToken.None;

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task<int>>>(), cancellationToken))
            .Returns((Func<ProfitSharingDbContext, Task<int>> func, CancellationToken ct) => func(dbContext));

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var fakeSsnServiceMock = new Mock<IFakeSsnService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            dataContextFactoryMock.Object,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object,
            fakeSsnServiceMock.Object
        );

        // Act
        await service.AuditError(badgeNumber, oracleHcmId, errorMessages, requestedBy, cancellationToken, null!);

        // Assert
        var audits = await dbContext.DemographicSyncAudit.ToListAsync();
        Assert.Single(audits);
        Assert.Equal(badgeNumber, audits[0].BadgeNumber);
        Assert.Equal(oracleHcmId, audits[0].OracleHcmId);
        Assert.Equal("Invalid SSN", audits[0].Message);
        Assert.Equal("SSN", audits[0].PropertyName);
        Assert.Equal("123456789", audits[0].InvalidValue);
        Assert.Equal(requestedBy, audits[0].UserName);
    }

    /// <summary>
    /// this one is never tested since it fails...
    /// Message: 
    ///    System.InvalidOperationException : The methods 'ExecuteDelete' and 'ExecuteDeleteAsync' are not supported by the current database provider.Please contact the publisher of the database provider for more information. 
    ///
    /// Claude suggests  
    /// 
    ///     For tests using the InMemory provider, replace ExecuteDeleteAsync with code that manually removes entities and calls SaveChangesAsync.For example:
    /// 
    /// var oldAudits = dbContext.DemographicSyncAudit.Where(t => t.Created < clearBackTo).ToList();
    /// dbContext.DemographicSyncAudit.RemoveRange(oldAudits);
    /// await dbContext.SaveChangesAsync(cancellationToken);
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "See comment")]
    private async Task CleanAuditError_DeletesOldAuditRecords()
    {
        // Arrange
        var dbContext = await SetupProfitShareDbContextAsync();

        // Add some audit records, some older than 30 days
        var oldAudit = new DemographicSyncAudit
        {
            BadgeNumber = 101,
            OracleHcmId = 1,
            InvalidValue = "123-45-6789",
            Message = "Test old audit",
            UserName = "TestUser",
            PropertyName = "SSN",
            Created = DateTime.Today.AddDays(-31)
        };

        var recentAudit = new DemographicSyncAudit
        {
            BadgeNumber = 102,
            OracleHcmId = 2,
            InvalidValue = "987-65-4321",
            Message = "Test recent audit",
            UserName = "TestUser",
            PropertyName = "SSN",
            Created = DateTime.Today
        };

        await dbContext.DemographicSyncAudit.AddRangeAsync(oldAudit, recentAudit);
        await dbContext.SaveChangesAsync();

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task<int>>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<ProfitSharingDbContext, Task<int>> func, CancellationToken ct) => func(dbContext));

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var fakeSsnServiceMock = new Mock<IFakeSsnService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            dataContextFactoryMock.Object,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object,
            fakeSsnServiceMock.Object
        );

        // Act
        await service.CleanAuditError(CancellationToken.None);

        // Assert
        var remainingRecords = await dbContext.DemographicSyncAudit.CountAsync();
        Assert.Equal(1, remainingRecords);

        var remainingAudit = await dbContext.DemographicSyncAudit.SingleAsync();
        Assert.Equal(recentAudit.BadgeNumber, remainingAudit.BadgeNumber);
        Assert.Equal(recentAudit.Message, remainingAudit.Message);
    }

    [Fact]
    public async Task AddDemographicsStreamAsync_InsertsNewEntities()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ProfitSharingDbContext(options);

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

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<ProfitSharingDbContext, Task> func, CancellationToken ct) => func(dbContext));

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var fakeSsnServiceMock = new Mock<IFakeSsnService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            dataContextFactoryMock.Object,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object,
            fakeSsnServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        var demographics = await dbContext.Demographics.ToListAsync();
        var histories = await dbContext.DemographicHistories.ToListAsync();

        Assert.Single(demographics);
        Assert.Single(histories);
        Assert.Equal(111111111, demographics[0].Ssn);
        Assert.Equal(100, demographics[0].BadgeNumber);
        Assert.Equal("First", demographics[0].ContactInfo.FirstName);
        Assert.Equal("123 Main St", demographics[0].Address.Street);
    }

    private async Task<ProfitSharingDbContext> SetupProfitShareDbContextAsync()
    {
        var demographics = new List<Demographic>()
        {
            new Demographic
            {
                OracleHcmId = 1,
                Ssn = 222222222,
                BadgeNumber = 101,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                StoreNumber = 1,
                DepartmentId = 1,
                PayClassificationId = 1,
                HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                EmploymentTypeId = 'F',
                PayFrequencyId = 1,
                EmploymentStatusId = 'A',
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
            },
            new Demographic
            {
                OracleHcmId = 2,
                Ssn = 222222222,
                BadgeNumber = 102,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                StoreNumber = 1,
                DepartmentId = 1,
                PayClassificationId = 1,
                HireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                EmploymentTypeId = 'F',
                PayFrequencyId = 1,
                EmploymentStatusId = 'A',
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
            }
        };

        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ProfitSharingDbContext(options);
        // Seed existing entities with duplicate SSNs
        dbContext.Demographics.AddRange(demographics);

        demographics.ForEach(demographic =>
        {
            DemographicHistory newHistoryRecord = DemographicHistory.FromDemographic(demographic, demographic.Id);
            dbContext.DemographicHistories.Add(newHistoryRecord);
        });

        await dbContext.SaveChangesAsync();

        return dbContext;
    }

    [Fact]
    public async Task AddDemographicsStreamAsync_HandlesDuplicateSsn()
    {
        // Arrange
        var dbContext = await SetupProfitShareDbContextAsync();

        var employees = new[]
        {
            new DemographicsRequest
            {
                OracleHcmId = 1,
                Ssn = 222222222,
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
                    PostalCode = "12345",
                },
                GenderCode = 'M'
            }
        };

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<ProfitSharingDbContext, Task> func, CancellationToken ct) => func(dbContext));

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var fakeSsnServiceMock = new Mock<IFakeSsnService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            dataContextFactoryMock.Object,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object,
            fakeSsnServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        var result = await dbContext.DemographicSyncAudit.CountAsync();
        Assert.True(result > 0, "Expected audit records for duplicate SSN handling.");
    }

    /// <summary>
    /// UC - If SSN matches but DateOfBirth does not match, and existing employee is terminated,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddDemographicsStreamAsync_SSNMatch_NoDobMatch()
    {
        // Arrange
        var dbContext = await SetupProfitShareDbContextAsync();

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
        var test = await dbContext.Demographics.FirstAsync(d => d.Ssn == 222222222);
        test.Ssn = 33333333;
        test.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-26));

        await dbContext.Demographics.AddAsync(demoWithDiffDob);

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

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<ProfitSharingDbContext, Task> func, CancellationToken ct) => func(dbContext));

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var fakeSsnServiceMock = new Mock<IFakeSsnService>();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());

        var service = new DemographicsService(
            dataContextFactoryMock.Object,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object,
            fakeSsnServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        var result = await dbContext.DemographicSyncAudit.CountAsync();
        Assert.True(result > 0, "Expected audit records for duplicate SSN handling.");
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
        var dbContext = await SetupProfitShareDbContextAsync();

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
        var test = await dbContext.Demographics.FirstAsync(d => d.Ssn == 222222222);
        test.Ssn = 33333333;
        test.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-26));

        await dbContext.Demographics.AddAsync(demoWithDiffDob);
        await dbContext.SaveChangesAsync();

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

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<ProfitSharingDbContext, Task> func, CancellationToken ct) => func(dbContext));

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var fakeSsnServiceMock = new Mock<IFakeSsnService>();
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

        fakeSsnServiceMock.Setup(f => f.GenerateFakeSsnAsync(It.IsAny<CancellationToken>())).ReturnsAsync(555555555);

        var service = new DemographicsService(
            dataContextFactoryMock.Object,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object,
            fakeSsnServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        var result = await dbContext.DemographicSyncAudit.CountAsync();
        Assert.True(result > 0, "Expected audit records for duplicate SSN handling.");

        // verify SSN was changed for termed employee to fake SSN
        var termedEmployee = await dbContext.Demographics.FirstAsync(d => d.OracleHcmId == 3);
        Assert.Equal(555555555, termedEmployee.Ssn);

        // verify existing employee was updated with new SSN
        var existingEmployee = await dbContext.Demographics.FirstAsync(d => d.OracleHcmId == 1);
        Assert.Equal(44444444, existingEmployee.Ssn);

        // add beneficiaries and pay details
    }

    [Fact]
    public async Task AddDemographicsStreamAsync_SSNMatch_NoDobMatch_ExistingEmployeeTerminated_HasBalance()
    {
        // Arrange
        var dbContext = await SetupProfitShareDbContextAsync();

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
        var test = await dbContext.Demographics.FirstAsync(d => d.Ssn == 222222222);
        test.Ssn = 33333333;
        test.DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-26));

        await dbContext.Demographics.AddAsync(demoWithDiffDob);

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

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<ProfitSharingDbContext, Task> func, CancellationToken ct) => func(dbContext));

        var loggerMock = new Mock<ILogger<DemographicsService>>();
        var totalServiceMock = new Mock<ITotalService>();
        var fakeSsnServiceMock = new Mock<IFakeSsnService>();
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

        fakeSsnServiceMock.Setup(f => f.GenerateFakeSsnAsync(It.IsAny<CancellationToken>())).ReturnsAsync(555555555);

        var service = new DemographicsService(
            dataContextFactoryMock.Object,
            mapper,
            loggerMock.Object,
            totalServiceMock.Object,
            fakeSsnServiceMock.Object
        );

        // Act
        await service.AddDemographicsStreamAsync(employees);

        // Assert
        var result = await dbContext.DemographicSyncAudit.CountAsync();
        Assert.True(result > 0, "Expected audit records for duplicate SSN handling.");

        // verify SSN was NOT changed for termed employee to fake SSN
        var termedEmployee = await dbContext.Demographics.FirstAsync(d => d.OracleHcmId == 3);
        Assert.Equal(222222222, termedEmployee.Ssn);

        // verify existing employee was updated with new SSN
        var existingEmployee = await dbContext.Demographics.FirstAsync(d => d.OracleHcmId == 1);
        Assert.Equal(44444444, existingEmployee.Ssn);

        // add beneficiaries and pay details

    }
}
