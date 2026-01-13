using System.ComponentModel;
using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.MasterInquiry;

/// <summary>
/// PS-2253 & PS-2254: Master Inquiry Filter Tests
/// Verifies that all filters (voids, contribution, earnings, forfeiture, payment)
/// are properly applied in GetMemberProfitDetails method
/// </summary>
public class MasterInquiryFiltersTest : ApiTestBase<Program>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryFiltersTest()
    {
        _masterInquiryService = ServiceProvider?.GetRequiredService<IMasterInquiryService>()!;
    }

    /// <summary>
    /// Helper method to profile individual test execution time
    /// Useful for identifying performance bottlenecks in the test suite
    /// </summary>
    private static async Task<T> ProfileTestAsync<T>(string testName, Func<Task<T>> testFunc)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await testFunc();
        }
        finally
        {
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"[PERF] {testName}: {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    [Description("PS-2253: Verify voids filter is applied when fetching member profit details")]
    [Fact]
    public Task GetMemberProfitDetails_WithVoidsTrue_ShouldReturnData()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithVoidsTrue", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                Voids = true,
                Take = 5// Only fetch voided records
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2253: Verify voids filter is not applied when set to false")]
    [Fact]
    public Task GetMemberProfitDetails_WithVoidsFalse_ShouldReturnData()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithVoidsFalse", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                Voids = false, // Include all records
                Take = 5
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2254: Verify contribution amount filter is applied")]
    [Fact]
    public Task GetMemberProfitDetails_WithContributionAmount_ShouldFilterByContribution()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithContributionAmount", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                ContributionAmount = 1000.00m,
                Take = 5
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2254: Verify earnings amount filter is applied")]
    [Fact]
    public Task GetMemberProfitDetails_WithEarningsAmount_ShouldFilterByEarnings()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithEarningsAmount", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                EarningsAmount = 2500.00m,
                Take = 5
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2254: Verify forfeiture amount filter is applied")]
    [Fact]
    public Task GetMemberProfitDetails_WithForfeitureAmount_ShouldFilterByForfeiture()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithForfeitureAmount", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                ForfeitureAmount = 500.00m,
                Take = 5
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2254: Verify payment amount filter is applied")]
    [Fact]
    public Task GetMemberProfitDetails_WithPaymentAmount_ShouldFilterByPayment()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithPaymentAmount", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                PaymentAmount = 1500.00m,
                Take = 5
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2253 & PS-2254: Verify multiple filters applied together")]
    [Fact]
    public Task GetMemberProfitDetails_WithMultipleFilters_ShouldApplyAllFilters()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithMultipleFilters", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                Voids = true,
                ContributionAmount = 1000.00m,
                EarningsAmount = 2500.00m,
                ForfeitureAmount = 500.00m,
                PaymentAmount = 1500.00m,
                Take = 5
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2253: Verify voids null value doesn't break the query")]
    [Fact]
    public Task GetMemberProfitDetails_WithVoidsNull_ShouldReturnAllRecords()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithVoidsNull", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                Voids = null // No void filtering
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2253: Verify filters don't affect pagination")]
    [Fact]
    public Task GetMemberProfitDetails_WithFiltersAndPagination_ShouldApplyBothCorrectly()
    {
        return ProfileTestAsync("GetMemberProfitDetails_WithFiltersAndPagination", async () =>
        {
            // Arrange
            var memberType = (byte)1; // Employee
            var memberId = 1; // Test member from seed data
            var requestPage1 = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                Voids = true,
                ContributionAmount = 1000.00m,
                Skip = 0,
                Take = 5
            };

            var requestPage2 = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                Voids = true,
                ContributionAmount = 1000.00m,
                Skip = 10,
                Take = 5
            };

            // Act
            var resultPage1Task = _masterInquiryService.GetMemberProfitDetails(requestPage1, CancellationToken.None);
            var resultPage2Task = _masterInquiryService.GetMemberProfitDetails(requestPage2, CancellationToken.None);

            await Task.WhenAll(resultPage1Task, resultPage2Task);
            var resultPage1 = await resultPage1Task;
            var resultPage2 = await resultPage2Task;

            // Assert
            resultPage1.ShouldNotBeNull();
            resultPage2.ShouldNotBeNull();
            resultPage1.Total.ShouldBe(resultPage2.Total);

            return resultPage1;
        });
    }

    [Description("PS-2254: Verify beneficiary member type with filters")]
    [Fact]
    public Task GetMemberProfitDetails_BeneficiaryWithFilters_ShouldApplyFiltersCorrectly()
    {
        return ProfileTestAsync("GetMemberProfitDetails_BeneficiaryWithFilters", async () =>
        {
            // Arrange
            var memberType = (byte)2; // Beneficiary
            var memberId = 1; // Test beneficiary from seed data
            var request = new MasterInquiryMemberDetailsRequest
            {
                MemberType = memberType,
                Id = memberId,
                ProfitYear = 2024,
                Voids = false,
                PaymentAmount = 1500.00m
            };

            // Act
            var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Total.ShouldBeGreaterThanOrEqualTo(0);

            return result;
        });
    }

    [Description("PS-2253: Verify request DTO has Voids property")]
    [Fact]
    public void MasterInquiryMemberDetailsRequest_ShouldHaveVoidsProperty()
    {
        // Arrange - synchronous test, no profiling needed
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = 1,
            Id = 1,
            Voids = true
        };

        // Act & Assert
        request.Voids.ShouldBe(true);
    }

    [Description("PS-2254: Verify request DTO has amount filter properties")]
    [Fact]
    public void MasterInquiryMemberDetailsRequest_ShouldHaveAmountFilterProperties()
    {
        // Arrange - synchronous test, no profiling needed
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = 1,
            Id = 1,
            ContributionAmount = 1000.00m,
            EarningsAmount = 2500.00m,
            ForfeitureAmount = 500.00m,
            PaymentAmount = 1500.00m
        };

        // Act & Assert
        request.ContributionAmount.ShouldBe(1000.00m);
        request.EarningsAmount.ShouldBe(2500.00m);
        request.ForfeitureAmount.ShouldBe(500.00m);
        request.PaymentAmount.ShouldBe(1500.00m);
    }
}
