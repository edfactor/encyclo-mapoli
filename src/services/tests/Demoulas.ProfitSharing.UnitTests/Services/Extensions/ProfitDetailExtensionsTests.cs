using System.ComponentModel;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.Extensions;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.Extensions;

/// <summary>
/// Tests for ProfitDetailExtensions aggregation methods.
/// These tests verify correct aggregation of profit details by profit code,
/// including the critical ProfitCode 8 (100% vested earnings) inclusion.
/// Related Ticket: PS-2424 (Account History Report missing ProfitCode 8 earnings)
/// </summary>
public class ProfitDetailExtensionsTests
{
    #region AggregateContributions Tests

    [Description("PS-2424: Verify contributions aggregation includes only ProfitCode 0")]
    [Fact]
    public void AggregateContributions_OnlyIncludesProfitCode0()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 50.00m),
            CreateProfitDetail(profitCodeId: 0, contribution: 2000.00m, earnings: 100.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 42.60m), // PC 8 - should be excluded from contributions
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 500.00m) // Payment - should be excluded
        };

        // Act
        var result = ProfitDetailExtensions.AggregateContributions(profitDetails);

        // Assert
        result.ShouldBe(3000.00m);
    }

    [Description("PS-2424: Verify empty list returns zero contributions")]
    [Fact]
    public void AggregateContributions_EmptyList_ReturnsZero()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>();

        // Act
        var result = ProfitDetailExtensions.AggregateContributions(profitDetails);

        // Assert
        result.ShouldBe(0.00m);
    }

    [Description("PS-2424: Verify contributions with no ProfitCode 0 returns zero")]
    [Fact]
    public void AggregateContributions_NoProfitCode0_ReturnsZero()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 100.00m),
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 500.00m)
        };

        // Act
        var result = ProfitDetailExtensions.AggregateContributions(profitDetails);

        // Assert
        result.ShouldBe(0.00m);
    }

    #endregion

    #region AggregateEarnings Tests

    [Description("PS-2424: CRITICAL - Verify earnings aggregation includes both ProfitCode 0 AND ProfitCode 8")]
    [Fact]
    public void AggregateEarnings_IncludesBothProfitCode0AndProfitCode8()
    {
        // Arrange - This is the exact scenario from PS-2424 ticket
        // Badge 706355 had 2021 showing $608.56 but should be $651.16
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 608.56m), // Regular earnings
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 42.60m), // ETVA/Class Action - MUST be included
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 100.00m) // Payment - should be excluded
        };

        // Act
        var result = ProfitDetailExtensions.AggregateEarnings(profitDetails);

        // Assert - Should include BOTH PC 0 and PC 8 earnings
        result.ShouldBe(651.16m, "Earnings must include both ProfitCode 0 ($608.56) and ProfitCode 8 ($42.60) for total $651.16");
    }

    [Description("PS-2424: Verify earnings with only ProfitCode 0")]
    [Fact]
    public void AggregateEarnings_OnlyProfitCode0_ReturnsCorrectSum()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 50.00m),
            CreateProfitDetail(profitCodeId: 0, contribution: 2000.00m, earnings: 100.00m),
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 500.00m)
        };

        // Act
        var result = ProfitDetailExtensions.AggregateEarnings(profitDetails);

        // Assert
        result.ShouldBe(150.00m);
    }

    [Description("PS-2424: Verify earnings with only ProfitCode 8 (ETVA only)")]
    [Fact]
    public void AggregateEarnings_OnlyProfitCode8_ReturnsCorrectSum()
    {
        // Arrange - Member with ONLY ETVA earnings (e.g., Class Action settlement, no regular earnings yet)
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 250.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 150.00m),
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 100.00m)
        };

        // Act
        var result = ProfitDetailExtensions.AggregateEarnings(profitDetails);

        // Assert
        result.ShouldBe(400.00m, "Should correctly sum ProfitCode 8 earnings");
    }

    [Description("PS-2424: Verify earnings with multiple years of PC 0 and PC 8")]
    [Fact]
    public void AggregateEarnings_MultipleYearsMixedCodes_ReturnsCorrectSum()
    {
        // Arrange - Simulating multiple years of earnings
        var profitDetails = new List<ProfitDetail>
        {
            // Year 1
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 100.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 25.00m),
            // Year 2
            CreateProfitDetail(profitCodeId: 0, contribution: 1200.00m, earnings: 150.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 30.00m),
            // Year 3
            CreateProfitDetail(profitCodeId: 0, contribution: 1500.00m, earnings: 200.00m)
            // No PC 8 in year 3
        };

        // Act
        var result = ProfitDetailExtensions.AggregateEarnings(profitDetails);

        // Assert
        result.ShouldBe(505.00m, "Should sum all PC 0 and PC 8 earnings: 100+25+150+30+200 = 505");
    }

    [Description("PS-2424: Verify empty list returns zero earnings")]
    [Fact]
    public void AggregateEarnings_EmptyList_ReturnsZero()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>();

        // Act
        var result = ProfitDetailExtensions.AggregateEarnings(profitDetails);

        // Assert
        result.ShouldBe(0.00m);
    }

    [Description("PS-2424: Verify earnings excludes payment codes")]
    [Fact]
    public void AggregateEarnings_ExcludesPaymentCodes()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 100.00m),
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 200.00m), // Payment
            CreateProfitDetail(profitCodeId: 2, contribution: 0.00m, earnings: 0.00m, forfeiture: 300.00m), // Forfeiture out
            CreateProfitDetail(profitCodeId: 9, contribution: 0.00m, earnings: 0.00m, forfeiture: 400.00m)  // PC 9 payment
        };

        // Act
        var result = ProfitDetailExtensions.AggregateEarnings(profitDetails);

        // Assert
        result.ShouldBe(100.00m, "Should only include PC 0 earnings, excluding all payment codes");
    }

    #endregion

    #region AggregateForfeitures Tests

    [Description("PS-2424: Verify forfeitures exclude payment codes")]
    [Fact]
    public void AggregateForfeitures_ExcludesPaymentCodes()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 100.00m, forfeiture: 50.00m),
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 200.00m), // Payment - excluded
            CreateProfitDetail(profitCodeId: 2, contribution: 0.00m, earnings: 0.00m, forfeiture: 300.00m), // Forfeiture out - excluded
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 42.00m, forfeiture: 0.00m)
        };

        // Act
        var result = ProfitDetailExtensions.AggregateForfeitures(profitDetails);

        // Assert
        result.ShouldBe(50.00m, "Should only include non-payment forfeiture amounts");
    }

    [Description("PS-2424: Verify forfeitures with no payment codes")]
    [Fact]
    public void AggregateForfeitures_NoPaymentCodes_ReturnsAllForfeitures()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 100.00m, forfeiture: 50.00m),
            CreateProfitDetail(profitCodeId: 0, contribution: 2000.00m, earnings: 200.00m, forfeiture: 75.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 42.00m, forfeiture: 0.00m)
        };

        // Act
        var result = ProfitDetailExtensions.AggregateForfeitures(profitDetails);

        // Assert
        result.ShouldBe(125.00m);
    }

    #endregion

    #region AggregateAllProfitValues Tests

    [Description("PS-2424: CRITICAL - Verify complete aggregation includes ProfitCode 8 earnings")]
    [Fact]
    public void AggregateAllProfitValues_IncludesProfitCode8InEarnings()
    {
        // Arrange - Complete year scenario with PC 0 and PC 8
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 608.56m, forfeiture: 25.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 42.60m, forfeiture: 0.00m), // ETVA
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 100.00m) // Payment - excluded from forfeitures
        };

        // Act
        var (contributions, earnings, forfeitures) = ProfitDetailExtensions.AggregateAllProfitValues(profitDetails);

        // Assert
        contributions.ShouldBe(1000.00m, "Contributions should only include PC 0");
        earnings.ShouldBe(651.16m, "Earnings MUST include both PC 0 ($608.56) and PC 8 ($42.60)");
        forfeitures.ShouldBe(25.00m, "Forfeitures should exclude payment codes");
    }

    [Description("PS-2424: Verify aggregation with multiple profit codes")]
    [Fact]
    public void AggregateAllProfitValues_MultipleYearsAllCodes_ReturnsCorrectSums()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            // Year 1 - PC 0 only
            CreateProfitDetail(profitCodeId: 0, contribution: 1000.00m, earnings: 100.00m, forfeiture: 10.00m),
            // Year 2 - PC 0 and PC 8
            CreateProfitDetail(profitCodeId: 0, contribution: 1200.00m, earnings: 150.00m, forfeiture: 15.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 50.00m, forfeiture: 0.00m),
            // Year 3 - Withdrawal year
            CreateProfitDetail(profitCodeId: 0, contribution: 1500.00m, earnings: 200.00m, forfeiture: 20.00m),
            CreateProfitDetail(profitCodeId: 1, contribution: 0.00m, earnings: 0.00m, forfeiture: 500.00m), // Withdrawal
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 75.00m, forfeiture: 0.00m)
        };

        // Act
        var (contributions, earnings, forfeitures) = ProfitDetailExtensions.AggregateAllProfitValues(profitDetails);

        // Assert
        contributions.ShouldBe(3700.00m, "Sum of all PC 0 contributions: 1000+1200+1500");
        earnings.ShouldBe(575.00m, "Sum of PC 0 and PC 8 earnings: 100+150+50+200+75");
        forfeitures.ShouldBe(45.00m, "Sum of non-payment forfeitures: 10+15+20 (excludes 500 from PC 1)");
    }

    [Description("PS-2424: Verify aggregation with zero values")]
    [Fact]
    public void AggregateAllProfitValues_ZeroValues_ReturnsZero()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>
        {
            CreateProfitDetail(profitCodeId: 0, contribution: 0.00m, earnings: 0.00m, forfeiture: 0.00m),
            CreateProfitDetail(profitCodeId: 8, contribution: 0.00m, earnings: 0.00m, forfeiture: 0.00m)
        };

        // Act
        var (contributions, earnings, forfeitures) = ProfitDetailExtensions.AggregateAllProfitValues(profitDetails);

        // Assert
        contributions.ShouldBe(0.00m);
        earnings.ShouldBe(0.00m);
        forfeitures.ShouldBe(0.00m);
    }

    [Description("PS-2424: Verify aggregation with empty list")]
    [Fact]
    public void AggregateAllProfitValues_EmptyList_ReturnsZero()
    {
        // Arrange
        var profitDetails = new List<ProfitDetail>();

        // Act
        var (contributions, earnings, forfeitures) = ProfitDetailExtensions.AggregateAllProfitValues(profitDetails);

        // Assert
        contributions.ShouldBe(0.00m);
        earnings.ShouldBe(0.00m);
        forfeitures.ShouldBe(0.00m);
    }

    #endregion

    #region GetProfitCodesForBalanceCalc Tests

    [Description("PS-2424: Verify payment codes list includes all expected codes")]
    [Fact]
    public void GetProfitCodesForBalanceCalc_ReturnsExpectedPaymentCodes()
    {
        // Act
        var paymentCodes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

        // Assert
        paymentCodes.ShouldNotBeEmpty();
        paymentCodes.ShouldContain((byte)1, "Should include PC 1 - Partial Withdrawal");
        paymentCodes.ShouldContain((byte)2, "Should include PC 2 - Outgoing Forfeitures");
        paymentCodes.ShouldContain((byte)3, "Should include PC 3 - Direct Payments");
        paymentCodes.ShouldContain((byte)5, "Should include PC 5 - Transfer to Beneficiary");
        paymentCodes.ShouldContain((byte)9, "Should include PC 9 - 100% Vested Payment");
        paymentCodes.Length.ShouldBe(5, "Should have exactly 5 payment codes");
    }

    [Description("PS-2424: Verify payment codes do NOT include PC 0 or PC 8")]
    [Fact]
    public void GetProfitCodesForBalanceCalc_DoesNotIncludeIncomingCodes()
    {
        // Act
        var paymentCodes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

        // Assert
        paymentCodes.ShouldNotContain((byte)0, "Should NOT include PC 0 - Incoming Contributions");
        paymentCodes.ShouldNotContain((byte)8, "Should NOT include PC 8 - Incoming 100% Vested Earnings");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a ProfitDetail entity for testing purposes.
    /// </summary>
    private static ProfitDetail CreateProfitDetail(
        byte profitCodeId,
        decimal contribution = 0.00m,
        decimal earnings = 0.00m,
        decimal forfeiture = 0.00m,
        short profitYear = 2021,
        int ssn = 123456789)
    {
        return new ProfitDetail
        {
            Id = 0, // Not important for aggregation tests
            Ssn = ssn,
            ProfitYear = profitYear,
            ProfitYearIteration = 0,
            ProfitCodeId = profitCodeId,
            Contribution = contribution,
            Earnings = earnings,
            Forfeiture = forfeiture,
            FederalTaxes = 0.00m,
            StateTaxes = 0.00m,
            MonthToDate = 0,
            YearToDate = 0,
            DistributionSequence = 0,
            ReversedFromProfitDetailId = null,
            YearsOfServiceCredit = 0
        };
    }

    #endregion
}
