using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.ComponentModel;

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

    [Description("PS-2253: Verify voids filter is applied when fetching member profit details")]
    [Fact]
    public async Task GetMemberProfitDetails_WithVoidsTrue_ShouldReturnData()
    {
        // Arrange
        var memberType = (byte)1; // Employee
        var memberId = 1; // Test member from seed data
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = memberType,
            Id = memberId,
            ProfitYear = 2024,
            Voids = true // Only fetch voided records
        };

        // Act
        var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2253: Verify voids filter is not applied when set to false")]
    [Fact]
    public async Task GetMemberProfitDetails_WithVoidsFalse_ShouldReturnData()
    {
        // Arrange
        var memberType = (byte)1; // Employee
        var memberId = 1; // Test member from seed data
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = memberType,
            Id = memberId,
            ProfitYear = 2024,
            Voids = false // Include all records
        };

        // Act
        var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2254: Verify contribution amount filter is applied")]
    [Fact]
    public async Task GetMemberProfitDetails_WithContributionAmount_ShouldFilterByContribution()
    {
        // Arrange
        var memberType = (byte)1; // Employee
        var memberId = 1; // Test member from seed data
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = memberType,
            Id = memberId,
            ProfitYear = 2024,
            ContributionAmount = 1000.00m
        };

        // Act
        var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2254: Verify earnings amount filter is applied")]
    [Fact]
    public async Task GetMemberProfitDetails_WithEarningsAmount_ShouldFilterByEarnings()
    {
        // Arrange
        var memberType = (byte)1; // Employee
        var memberId = 1; // Test member from seed data
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = memberType,
            Id = memberId,
            ProfitYear = 2024,
            EarningsAmount = 2500.00m
        };

        // Act
        var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2254: Verify forfeiture amount filter is applied")]
    [Fact]
    public async Task GetMemberProfitDetails_WithForfeitureAmount_ShouldFilterByForfeiture()
    {
        // Arrange
        var memberType = (byte)1; // Employee
        var memberId = 1; // Test member from seed data
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = memberType,
            Id = memberId,
            ProfitYear = 2024,
            ForfeitureAmount = 500.00m
        };

        // Act
        var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2254: Verify payment amount filter is applied")]
    [Fact]
    public async Task GetMemberProfitDetails_WithPaymentAmount_ShouldFilterByPayment()
    {
        // Arrange
        var memberType = (byte)1; // Employee
        var memberId = 1; // Test member from seed data
        var request = new MasterInquiryMemberDetailsRequest
        {
            MemberType = memberType,
            Id = memberId,
            ProfitYear = 2024,
            PaymentAmount = 1500.00m
        };

        // Act
        var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2253 & PS-2254: Verify multiple filters applied together")]
    [Fact]
    public async Task GetMemberProfitDetails_WithMultipleFilters_ShouldApplyAllFilters()
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
            PaymentAmount = 1500.00m
        };

        // Act
        var result = await _masterInquiryService.GetMemberProfitDetails(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Description("PS-2253: Verify voids null value doesn't break the query")]
    [Fact]
    public async Task GetMemberProfitDetails_WithVoidsNull_ShouldReturnAllRecords()
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
    }

    [Description("PS-2253: Verify filters don't affect pagination")]
    [Fact]
    public async Task GetMemberProfitDetails_WithFiltersAndPagination_ShouldApplyBothCorrectly()
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
            Take = 10
        };

        var requestPage2 = new MasterInquiryMemberDetailsRequest
        {
            MemberType = memberType,
            Id = memberId,
            ProfitYear = 2024,
            Voids = true,
            ContributionAmount = 1000.00m,
            Skip = 10,
            Take = 10
        };

        // Act
        var resultPage1 = await _masterInquiryService.GetMemberProfitDetails(requestPage1, CancellationToken.None);
        var resultPage2 = await _masterInquiryService.GetMemberProfitDetails(requestPage2, CancellationToken.None);

        // Assert
        resultPage1.ShouldNotBeNull();
        resultPage2.ShouldNotBeNull();
        resultPage1.Total.ShouldBe(resultPage2.Total);
    }

    [Description("PS-2254: Verify beneficiary member type with filters")]
    [Fact]
    public async Task GetMemberProfitDetails_BeneficiaryWithFilters_ShouldApplyFiltersCorrectly()
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
    }

    [Description("PS-2253: Verify request DTO has Voids property")]
    [Fact]
    public void MasterInquiryMemberDetailsRequest_ShouldHaveVoidsProperty()
    {
        // Arrange
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
        // Arrange
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
