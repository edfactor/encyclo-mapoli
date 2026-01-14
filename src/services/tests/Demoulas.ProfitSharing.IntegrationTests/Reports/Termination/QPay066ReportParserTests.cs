using System.ComponentModel;
using System.Reflection;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

public class QPay066ReportParserTests
{
    [Fact]
    [Description("PS-XXXX : Parse complete QPAY066 report with employees and beneficiaries")]
    public void ParseRecords_ShouldLoadFullReportWithEmployeesAndBeneficiaries()
    {
        // Arrange
        string reportText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");

        // Act
        List<QPay066Record> records = QPay066ReportParser.ParseRecords(reportText);

        // Assert - Overall count
        records.ShouldNotBeEmpty();
        records.Count.ShouldBeGreaterThan(490, "R3-QPAY066 should contain around 497 records (excluding headers)");

        // Assert - Validate 3 employees (short badge numbers)
        ValidateEmployee_707319_AcostaTheo(records);
        ValidateEmployee_709150_AliAyla(records);
        ValidateEmployee_701539_AliBella(records);

        // Assert - Validate 3 beneficiaries (long badge numbers with PSN suffix)
        ValidateBeneficiary_7039171000_AllenRaymond(records);
        ValidateBeneficiary_7026461000_AvilaSavannah(records);
        ValidateBeneficiary_7024451000_BartlettAtlas(records);
    }

    [Fact]
    [Description("PS-XXXX : Backward compatibility - ParseBadgeToVestedBalance returns correct dictionary")]
    public void ParseBadgeToVestedBalance_ShouldReturnCorrectDictionary()
    {
        // Arrange
        string reportText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");

        // Act
        Dictionary<int, decimal> badgeToVested = QPay066ReportParser.ParseBadgeToVestedBalance(reportText);

        // Assert
        badgeToVested.ShouldNotBeEmpty();
        // Note: Dictionary uses BadgeNumber only (not PSN suffix), so count may be less than total records
        badgeToVested.Count.ShouldBeGreaterThan(400);

        // Spot check a few values
        badgeToVested[707319].ShouldBe(2418.37m);
        badgeToVested[709150].ShouldBe(1096.42m);
        badgeToVested[703917].ShouldBe(99801.68m); // Beneficiary - badge portion only
    }

    [Fact]
    [Description("PS-XXXX : Parse totals section from QPAY066 report")]
    public void ParseTotals_ShouldExtractTotalsFromReport()
    {
        // Arrange
        string reportText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");

        // Act
        QPay066Totals? totals = QPay066ReportParser.ParseTotals(reportText);

        // Assert
        totals.ShouldNotBeNull();
        totals.AmountInProfitSharing.ShouldBe(24692640.86m);
        totals.VestedAmount.ShouldBe(22666201.39m);
        totals.TotalForfeitures.ShouldBe(-9439.79m);
        totals.TotalBeneficiaryAllocations.ShouldBe(1349459.54m);
    }

    [Fact]
    [Description("PS-XXXX : Computed totals should exactly match parsed totals from report")]
    public void ComputeTotals_ShouldMatchParsedTotals()
    {
        // Arrange
        string reportText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");
        List<QPay066Record> records = QPay066ReportParser.ParseRecords(reportText);
        QPay066Totals? parsedTotals = QPay066ReportParser.ParseTotals(reportText);

        // Act
        QPay066Totals computedTotals = QPay066ReportParser.ComputeTotals(records);

        // Assert
        parsedTotals.ShouldNotBeNull();

        // The COBOL program computes totals by summing the displayed fields
        // Our ComputeTotals should match exactly since we're summing the same parsed values
        computedTotals.AmountInProfitSharing.ShouldBe(parsedTotals.AmountInProfitSharing);
        computedTotals.VestedAmount.ShouldBe(parsedTotals.VestedAmount);
        computedTotals.TotalForfeitures.ShouldBe(parsedTotals.TotalForfeitures);
        computedTotals.TotalBeneficiaryAllocations.ShouldBe(parsedTotals.TotalBeneficiaryAllocations);
    }

    #region Helper Methods

    private static string ReadEmbeddedResource(string resourceName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        }

        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    #endregion

    #region Employee Validators (Short Badge Numbers)

    private static void ValidateEmployee_707319_AcostaTheo(List<QPay066Record> records)
    {
        // Line 9:      707319 ACOSTA, THEO           6,045.93         0.00         0.00         0.00     6,045.93     2,418.37  251111  697.00  40  36
        QPay066Record? record = records.FirstOrDefault(r => r.BadgeNumber == 707319);
        record.ShouldNotBeNull();

        record.BadgeNumber.ShouldBe(707319);
        record.PsnSuffix.ShouldBe((short)0);
        record.EmployeeName.ShouldBe("ACOSTA, THEO");
        record.BeginningBalance.ShouldBe(6045.93m);
        record.BeneficiaryAllocation.ShouldBe(0.00m);
        record.DistributionAmount.ShouldBe(0.00m);
        record.Forfeit.ShouldBe(0.00m);
        record.EndingBalance.ShouldBe(6045.93m);
        record.VestedBalance.ShouldBe(2418.37m);
        record.DateTerm.ShouldBe(new DateOnly(2025, 11, 11));
        record.YtdVstPsHours.ShouldBe(697.00m);
        record.VestedPercent.ShouldBe(40m);
        record.Age.ShouldBe(36);
        record.EnrollmentCode.ShouldBeNull();
    }

    private static void ValidateEmployee_709150_AliAyla(List<QPay066Record> records)
    {
        // Line 10:      709150 ALI, AYLA              5,482.11         0.00         0.00         0.00     5,482.11     1,096.42  250507  234.50  20  45
        QPay066Record? record = records.FirstOrDefault(r => r.BadgeNumber == 709150);
        record.ShouldNotBeNull();

        record.BadgeNumber.ShouldBe(709150);
        record.PsnSuffix.ShouldBe((short)0);
        record.EmployeeName.ShouldBe("ALI, AYLA");
        record.BeginningBalance.ShouldBe(5482.11m);
        record.BeneficiaryAllocation.ShouldBe(0.00m);
        record.DistributionAmount.ShouldBe(0.00m);
        record.Forfeit.ShouldBe(0.00m);
        record.EndingBalance.ShouldBe(5482.11m);
        record.VestedBalance.ShouldBe(1096.42m);
        record.DateTerm.ShouldBe(new DateOnly(2025, 5, 7));
        record.YtdVstPsHours.ShouldBe(234.50m);
        record.VestedPercent.ShouldBe(20m);
        record.Age.ShouldBe(45);
        record.EnrollmentCode.ShouldBeNull();
    }

    private static void ValidateEmployee_701539_AliBella(List<QPay066Record> records)
    {
        // Line 11:      701539 ALI, BELLA             5,692.59         0.00         0.00         0.00     5,692.59     2,277.04  250708  144.50  40  24
        QPay066Record? record = records.FirstOrDefault(r => r.BadgeNumber == 701539);
        record.ShouldNotBeNull();

        record.BadgeNumber.ShouldBe(701539);
        record.PsnSuffix.ShouldBe((short)0);
        record.EmployeeName.ShouldBe("ALI, BELLA");
        record.BeginningBalance.ShouldBe(5692.59m);
        record.BeneficiaryAllocation.ShouldBe(0.00m);
        record.DistributionAmount.ShouldBe(0.00m);
        record.Forfeit.ShouldBe(0.00m);
        record.EndingBalance.ShouldBe(5692.59m);
        record.VestedBalance.ShouldBe(2277.04m);
        record.DateTerm.ShouldBe(new DateOnly(2025, 7, 8));
        record.YtdVstPsHours.ShouldBe(144.50m);
        record.VestedPercent.ShouldBe(40m);
        record.Age.ShouldBe(24);
        record.EnrollmentCode.ShouldBeNull();
    }

    #endregion

    #region Beneficiary Validators (Long Badge Numbers with PSN Suffix)

    private static void ValidateBeneficiary_7039171000_AllenRaymond(List<QPay066Record> records)
    {
        // Line 18:  7039171000 ALLEN, RAYMOND        99,801.68         0.00         0.00         0.00    99,801.68    99,801.68            0.00 100  58
        QPay066Record? record = records.FirstOrDefault(r => r.BadgeNumber == 703917 && r.PsnSuffix == 1000);
        record.ShouldNotBeNull();

        record.BadgeNumber.ShouldBe(703917);
        record.PsnSuffix.ShouldBe((short)1000);
        record.EmployeeName.ShouldBe("ALLEN, RAYMOND");
        record.BeginningBalance.ShouldBe(99801.68m);
        record.BeneficiaryAllocation.ShouldBe(0.00m);
        record.DistributionAmount.ShouldBe(0.00m);
        record.Forfeit.ShouldBe(0.00m);
        record.EndingBalance.ShouldBe(99801.68m);
        record.VestedBalance.ShouldBe(99801.68m);
        record.DateTerm.ShouldBeNull(); // No date term (spaces in report)
        record.YtdVstPsHours.ShouldBe(0.00m);
        record.VestedPercent.ShouldBe(100m);
        record.Age.ShouldBe(58);
        record.EnrollmentCode.ShouldBeNull();
    }

    private static void ValidateBeneficiary_7026461000_AvilaSavannah(List<QPay066Record> records)
    {
        // Line 29:  7026461000 AVILA, SAVANNAH            0.00    45,072.21    45,072.21-        0.00         0.00         0.00            0.00  00  55
        QPay066Record? record = records.FirstOrDefault(r => r.BadgeNumber == 702646 && r.PsnSuffix == 1000);
        record.ShouldNotBeNull();

        record.BadgeNumber.ShouldBe(702646);
        record.PsnSuffix.ShouldBe((short)1000);
        record.EmployeeName.ShouldBe("AVILA, SAVANNAH");
        record.BeginningBalance.ShouldBe(0.00m);
        record.BeneficiaryAllocation.ShouldBe(45072.21m);
        record.DistributionAmount.ShouldBe(-45072.21m); // Negative distribution
        record.Forfeit.ShouldBe(0.00m);
        record.EndingBalance.ShouldBe(0.00m);
        record.VestedBalance.ShouldBe(0.00m);
        record.DateTerm.ShouldBeNull(); // No date term
        record.YtdVstPsHours.ShouldBe(0.00m);
        record.VestedPercent.ShouldBe(0m);
        record.Age.ShouldBe(55);
        record.EnrollmentCode.ShouldBeNull();
    }

    private static void ValidateBeneficiary_7024451000_BartlettAtlas(List<QPay066Record> records)
    {
        // Line 43:  7024451000 BARTLETT, ATLAS       23,386.42         0.00     9,000.00-        0.00    14,386.42    14,386.42            0.00 100  57
        QPay066Record? record = records.FirstOrDefault(r => r.BadgeNumber == 702445 && r.PsnSuffix == 1000);
        record.ShouldNotBeNull();

        record.BadgeNumber.ShouldBe(702445);
        record.PsnSuffix.ShouldBe((short)1000);
        record.EmployeeName.ShouldBe("BARTLETT, ATLAS");
        record.BeginningBalance.ShouldBe(23386.42m);
        record.BeneficiaryAllocation.ShouldBe(0.00m);
        record.DistributionAmount.ShouldBe(-9000.00m); // Negative distribution
        record.Forfeit.ShouldBe(0.00m);
        record.EndingBalance.ShouldBe(14386.42m);
        record.VestedBalance.ShouldBe(14386.42m);
        record.DateTerm.ShouldBeNull(); // No date term
        record.YtdVstPsHours.ShouldBe(0.00m);
        record.VestedPercent.ShouldBe(100m);
        record.Age.ShouldBe(57);
        record.EnrollmentCode.ShouldBeNull();
    }

    #endregion
}
