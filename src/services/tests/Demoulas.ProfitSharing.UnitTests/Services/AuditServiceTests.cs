using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Services.Audit;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Unit tests for ProfitSharingAuditService.ToKeyValuePairs method to verify all primitive numeric types
/// and their nullable versions are correctly archived with proper hashing.
/// </summary>
[Description("PS-XXXX : Verify all primitive numeric types are correctly archived")]
public sealed class AuditServiceTests
{
    #region Test Data Classes

    /// <summary>
    /// Test class with explicit [YearEndArchiveProperty] attributes on individual properties.
    /// Tests all C# primitive numeric types and their nullable versions.
    /// </summary>
    private sealed record ExplicitAttributeTestClass
    {
        [YearEndArchiveProperty]
        public byte ByteValue { get; set; }

        [YearEndArchiveProperty]
        public byte? NullableByteValue { get; set; }

        [YearEndArchiveProperty]
        public sbyte SByteValue { get; set; }

        [YearEndArchiveProperty]
        public sbyte? NullableSByteValue { get; set; }

        [YearEndArchiveProperty]
        public short ShortValue { get; set; }

        [YearEndArchiveProperty]
        public short? NullableShortValue { get; set; }

        [YearEndArchiveProperty]
        public ushort UShortValue { get; set; }

        [YearEndArchiveProperty]
        public ushort? NullableUShortValue { get; set; }

        [YearEndArchiveProperty]
        public int IntValue { get; set; }

        [YearEndArchiveProperty]
        public int? NullableIntValue { get; set; }

        [YearEndArchiveProperty]
        public uint UIntValue { get; set; }

        [YearEndArchiveProperty]
        public uint? NullableUIntValue { get; set; }

        [YearEndArchiveProperty]
        public long LongValue { get; set; }

        [YearEndArchiveProperty]
        public long? NullableLongValue { get; set; }

        [YearEndArchiveProperty]
        public ulong ULongValue { get; set; }

        [YearEndArchiveProperty]
        public ulong? NullableULongValue { get; set; }

        [YearEndArchiveProperty]
        public float FloatValue { get; set; }

        [YearEndArchiveProperty]
        public float? NullableFloatValue { get; set; }

        [YearEndArchiveProperty]
        public double DoubleValue { get; set; }

        [YearEndArchiveProperty]
        public double? NullableDoubleValue { get; set; }

        [YearEndArchiveProperty]
        public decimal DecimalValue { get; set; }

        [YearEndArchiveProperty]
        public decimal? NullableDecimalValue { get; set; }

        // Non-numeric property that should NOT be archived
        public string StringValue { get; set; } = string.Empty;

        // Non-attributed numeric property that should NOT be archived
        public int NonAttributedValue { get; set; }
    }

    /// <summary>
    /// Test class with [YearEndArchiveProperty] attribute on the class itself.
    /// Should archive all numeric properties automatically.
    /// </summary>
    [YearEndArchiveProperty]
    private sealed record ClassAttributeTestClass
    {
        public byte ByteValue { get; set; }
        public byte? NullableByteValue { get; set; }
        public short ShortValue { get; set; }
        public short? NullableShortValue { get; set; }
        public int IntValue { get; set; }
        public int? NullableIntValue { get; set; }
        public long LongValue { get; set; }
        public long? NullableLongValue { get; set; }
        public decimal DecimalValue { get; set; }
        public decimal? NullableDecimalValue { get; set; }
        public float FloatValue { get; set; }
        public float? NullableFloatValue { get; set; }
        public double DoubleValue { get; set; }
        public double? NullableDoubleValue { get; set; }

        // Non-numeric properties that should NOT be archived even with class attribute
        public string StringValue { get; set; } = string.Empty;
        public bool BoolValue { get; set; }
        public DateTime DateValue { get; set; }
    }

    /// <summary>
    /// Test class simulating real-world report response with nullable decimals.
    /// Similar to ForfeituresAndPointsForYearResponseWithTotals.
    /// </summary>
    private sealed record ReportResponseTestClass
    {
        [YearEndArchiveProperty]
        public decimal TotalAmount { get; set; }

        [YearEndArchiveProperty]
        public decimal? OptionalTotal1 { get; set; }

        [YearEndArchiveProperty]
        public decimal? OptionalTotal2 { get; set; }

        [YearEndArchiveProperty]
        public int RecordCount { get; set; }

        [YearEndArchiveProperty]
        public int? OptionalCount { get; set; }

        // Should not be archived
        public string ReportName { get; set; } = string.Empty;
        public DateTimeOffset ReportDate { get; set; }
    }

    #endregion

    #region Explicit Attribute Tests

    [Fact]
    public void ToKeyValuePairs_WithExplicitAttributes_ShouldArchiveAllAttributedNumericTypes()
    {
        // Arrange
        var testObj = new ExplicitAttributeTestClass
        {
            ByteValue = 255,
            NullableByteValue = 128,
            SByteValue = -128,
            NullableSByteValue = 64,
            ShortValue = -32768,
            NullableShortValue = 16384,
            UShortValue = 65535,
            NullableUShortValue = 32768,
            IntValue = -2147483648,
            NullableIntValue = 1073741824,
            UIntValue = 4294967295,
            NullableUIntValue = 2147483648,
            LongValue = -9223372036854775808,
            NullableLongValue = 4611686018427387904,
            ULongValue = 18446744073709551615,
            NullableULongValue = 9223372036854775808,
            FloatValue = 123.45f,
            NullableFloatValue = 67.89f,
            DoubleValue = 9876.54,
            NullableDoubleValue = 4321.98,
            DecimalValue = 12345.6789m,
            NullableDecimalValue = 98765.4321m,
            StringValue = "Should not be archived",
            NonAttributedValue = 999
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBeGreaterThanOrEqualTo(20); // At least 20 numeric properties with attribute

        // Verify each numeric type is present (non-nullable)
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.ByteValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.SByteValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.ShortValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.UShortValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.IntValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.UIntValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.LongValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.ULongValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.FloatValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.DoubleValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.DecimalValue));

        // Verify nullable types are present
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableByteValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableSByteValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableShortValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableUShortValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableIntValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableUIntValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableLongValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableULongValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableFloatValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableDoubleValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableDecimalValue));

        // Verify non-numeric and non-attributed properties are NOT present
        result.ShouldNotContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.StringValue));
        result.ShouldNotContain(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NonAttributedValue));

        // Verify all entries have hashes
        result.ShouldAllBe(kvp => kvp.Value.Value.Length == 32); // SHA256 = 32 bytes
    }

    [Fact]
    public void ToKeyValuePairs_WithExplicitAttributes_ShouldConvertAllTypesToDecimal()
    {
        // Arrange
        var testObj = new ExplicitAttributeTestClass
        {
            ByteValue = 100,
            ShortValue = 1000,
            IntValue = 10000,
            LongValue = 100000,
            DecimalValue = 123.45m
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert - all numeric types should be converted to decimal for hashing
        var byteEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.ByteValue));
        byteEntry.Value.Key.ShouldBe(100m);

        var shortEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.ShortValue));
        shortEntry.Value.Key.ShouldBe(1000m);

        var intEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.IntValue));
        intEntry.Value.Key.ShouldBe(10000m);

        var longEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.LongValue));
        longEntry.Value.Key.ShouldBe(100000m);

        var decimalEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.DecimalValue));
        decimalEntry.Value.Key.ShouldBe(123.45m);
    }

    [Fact]
    public void ToKeyValuePairs_WithNullNullableProperties_ShouldStoreAsZero()
    {
        // Arrange
        var testObj = new ExplicitAttributeTestClass
        {
            NullableByteValue = null,
            NullableIntValue = null,
            NullableLongValue = null,
            NullableDecimalValue = null
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert - null nullable properties should be stored as 0m
        var byteEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableByteValue));
        byteEntry.Value.Key.ShouldBe(0m);

        var intEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableIntValue));
        intEntry.Value.Key.ShouldBe(0m);

        var longEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableLongValue));
        longEntry.Value.Key.ShouldBe(0m);

        var decimalEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.NullableDecimalValue));
        decimalEntry.Value.Key.ShouldBe(0m);
    }

    #endregion

    #region Class Attribute Tests

    [Fact]
    public void ToKeyValuePairs_WithClassAttribute_ShouldArchiveAllNumericProperties()
    {
        // Arrange
        var testObj = new ClassAttributeTestClass
        {
            ByteValue = 255,
            NullableByteValue = 128,
            ShortValue = 32767,
            NullableShortValue = 16384,
            IntValue = 2147483647,
            NullableIntValue = 1073741824,
            LongValue = 9223372036854775807,
            NullableLongValue = 4611686018427387904,
            DecimalValue = 12345.67m,
            NullableDecimalValue = 98765.43m,
            FloatValue = 123.45f,
            NullableFloatValue = 67.89f,
            DoubleValue = 9876.54,
            NullableDoubleValue = 4321.98,
            StringValue = "Should not be archived",
            BoolValue = true,
            DateValue = DateTime.Now
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBeGreaterThanOrEqualTo(14); // All numeric properties

        // Verify numeric types are present
        result.ShouldContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.ByteValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.NullableByteValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.IntValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.NullableIntValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.DecimalValue));
        result.ShouldContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.NullableDecimalValue));

        // Verify non-numeric properties are NOT present
        result.ShouldNotContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.StringValue));
        result.ShouldNotContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.BoolValue));
        result.ShouldNotContain(kvp => kvp.Key == nameof(ClassAttributeTestClass.DateValue));
    }

    #endregion

    #region Real-World Report Tests

    [Fact]
    public void ToKeyValuePairs_WithReportResponse_ShouldArchiveAllMarkedFields()
    {
        // Arrange - simulates ForfeituresAndPointsForYearResponseWithTotals
        var testObj = new ReportResponseTestClass
        {
            TotalAmount = 12345.67m,
            OptionalTotal1 = 98765.43m,
            OptionalTotal2 = null, // Null nullable decimal
            RecordCount = 150,
            OptionalCount = null, // Null nullable int
            ReportName = "Test Report",
            ReportDate = DateTimeOffset.Now
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert
        result.Count.ShouldBe(5); // 5 properties with [YearEndArchiveProperty]

        // Verify all attributed properties are present
        result.ShouldContain(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount));
        result.ShouldContain(kvp => kvp.Key == nameof(ReportResponseTestClass.OptionalTotal1));
        result.ShouldContain(kvp => kvp.Key == nameof(ReportResponseTestClass.OptionalTotal2));
        result.ShouldContain(kvp => kvp.Key == nameof(ReportResponseTestClass.RecordCount));
        result.ShouldContain(kvp => kvp.Key == nameof(ReportResponseTestClass.OptionalCount));

        // Verify non-attributed properties are NOT present
        result.ShouldNotContain(kvp => kvp.Key == nameof(ReportResponseTestClass.ReportName));
        result.ShouldNotContain(kvp => kvp.Key == nameof(ReportResponseTestClass.ReportDate));

        // Verify values including nulls converted to zero
        var totalAmount = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount));
        totalAmount.Value.Key.ShouldBe(12345.67m);

        var optionalTotal1 = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.OptionalTotal1));
        optionalTotal1.Value.Key.ShouldBe(98765.43m);

        var optionalTotal2 = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.OptionalTotal2));
        optionalTotal2.Value.Key.ShouldBe(0m); // Null should be 0

        var recordCount = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.RecordCount));
        recordCount.Value.Key.ShouldBe(150m);

        var optionalCount = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.OptionalCount));
        optionalCount.Value.Key.ShouldBe(0m); // Null should be 0
    }

    #endregion

    #region Hash Consistency Tests

    [Fact]
    public void ToKeyValuePairs_WithSameValue_ShouldProduceSameHash()
    {
        // Arrange
        var testObj1 = new ReportResponseTestClass { TotalAmount = 12345.67m };
        var testObj2 = new ReportResponseTestClass { TotalAmount = 12345.67m };

        // Act
        var result1 = ProfitSharingAuditService.ToKeyValuePairs(testObj1, []).ToList();
        var result2 = ProfitSharingAuditService.ToKeyValuePairs(testObj2, []).ToList();

        // Assert
        var hash1 = result1.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount)).Value.Value;
        var hash2 = result2.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount)).Value.Value;

        hash1.ShouldBe(hash2);
    }

    [Fact]
    public void ToKeyValuePairs_WithDifferentValue_ShouldProduceDifferentHash()
    {
        // Arrange
        var testObj1 = new ReportResponseTestClass { TotalAmount = 12345.67m };
        var testObj2 = new ReportResponseTestClass { TotalAmount = 98765.43m };

        // Act
        var result1 = ProfitSharingAuditService.ToKeyValuePairs(testObj1, []).ToList();
        var result2 = ProfitSharingAuditService.ToKeyValuePairs(testObj2, []).ToList();

        // Assert
        var hash1 = result1.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount)).Value.Value;
        var hash2 = result2.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount)).Value.Value;

        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void ToKeyValuePairs_ShouldProduceSHA256Hash()
    {
        // Arrange
        var testObj = new ReportResponseTestClass { TotalAmount = 12345.67m };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert
        var hash = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount)).Value.Value;
        hash.Length.ShouldBe(32); // SHA256 produces 32 bytes
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ToKeyValuePairs_WithNoAttributedProperties_ShouldReturnEmptyCollection()
    {
        // Arrange
        var testObj = new { Name = "Test", Value = 123 }; // Anonymous type with no attributes

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, new List<Func<object, (string, object)>>()).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToKeyValuePairs_WithZeroValues_ShouldArchiveCorrectly()
    {
        // Arrange
        var testObj = new ReportResponseTestClass
        {
            TotalAmount = 0m,
            OptionalTotal1 = 0m,
            RecordCount = 0
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert
        result.ShouldNotBeEmpty();

        var totalAmount = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount));
        totalAmount.Value.Key.ShouldBe(0m);

        var optionalTotal1 = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.OptionalTotal1));
        optionalTotal1.Value.Key.ShouldBe(0m);
    }

    [Fact]
    public void ToKeyValuePairs_WithNegativeValues_ShouldArchiveCorrectly()
    {
        // Arrange
        var testObj = new ReportResponseTestClass
        {
            TotalAmount = -12345.67m,
            RecordCount = -100
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert
        var totalAmount = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.TotalAmount));
        totalAmount.Value.Key.ShouldBe(-12345.67m);

        var recordCount = result.First(kvp => kvp.Key == nameof(ReportResponseTestClass.RecordCount));
        recordCount.Value.Key.ShouldBe(-100m);
    }

    [Fact]
    public void ToKeyValuePairs_WithVeryLargeValues_ShouldArchiveCorrectly()
    {
        // Arrange
        var testObj = new ExplicitAttributeTestClass
        {
            LongValue = long.MaxValue,
            DecimalValue = decimal.MaxValue
        };

        // Act
        var result = ProfitSharingAuditService.ToKeyValuePairs(testObj, []).ToList();

        // Assert
        var longEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.LongValue));
        longEntry.Value.Key.ShouldBe(long.MaxValue);

        var decimalEntry = result.First(kvp => kvp.Key == nameof(ExplicitAttributeTestClass.DecimalValue));
        decimalEntry.Value.Key.ShouldBe(decimal.MaxValue);
    }

    #endregion
}
