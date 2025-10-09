# Quick Reference: Adding New Cross-Reference Validation Groups

## Overview
This guide shows how to add new cross-reference validation groups to the Master Update validation process.

## Step 1: Add Validation Method to ChecksumValidationService

**Location:** `Demoulas.ProfitSharing.Services/Validation/ChecksumValidationService.cs`

```csharp
/// <summary>
/// Validates the [Your Field Name] cross-references.
/// Validation Rule: [Your validation equation]
/// </summary>
private async Task<CrossReferenceValidationGroup> ValidateYourFieldGroupAsync(
    short profitYear,
    Dictionary<string, decimal> currentValues,
    HashSet<string> validatedReports,
    CancellationToken cancellationToken)
{
    var validations = new List<CrossReferenceValidation>();

    // Add each field to validate
    var report1Validation = await ValidateSingleFieldAsync(
        profitYear, "REPORT_CODE_1", "FieldName1", currentValues, cancellationToken);
    validations.Add(report1Validation);
    validatedReports.Add("REPORT_CODE_1");

    var report2Validation = await ValidateSingleFieldAsync(
        profitYear, "REPORT_CODE_2", "FieldName2", currentValues, cancellationToken);
    validations.Add(report2Validation);
    validatedReports.Add("REPORT_CODE_2");

    // Check if all valid
    bool allValid = validations.All(v => v.IsValid);
    
    // Create summary message
    string summary = allValid
        ? "[Success message - all reports in sync]"
        : "[Failure message - reports out of sync]";

    return new CrossReferenceValidationGroup
    {
        GroupName = "[Your Group Name]",
        Description = "[Description of what this validates]",
        IsValid = allValid,
        Validations = validations,
        Summary = summary,
        Priority = "Critical",  // or "High", "Medium", "Low"
        ValidationRule = "[Your validation equation, e.g., 'REPORT1.Field = REPORT2.Field']"
    };
}
```

## Step 2: Call Your Method in ValidateMasterUpdateCrossReferencesAsync

**Location:** Same file, in `ValidateMasterUpdateCrossReferencesAsync` method

```csharp
// Your new validation group
var yourFieldGroup = await ValidateYourFieldGroupAsync(
    profitYear, currentValues, validatedReports, cancellationToken);
validationGroups.Add(yourFieldGroup);
totalValidations += yourFieldGroup.Validations.Count;
passedValidations += yourFieldGroup.Validations.Count(v => v.IsValid);
failedValidations += yourFieldGroup.Validations.Count(v => !v.IsValid);

// Determine severity
if (!yourFieldGroup.IsValid)
{
    // Add to critical issues if this blocks Master Update
    criticalIssues.Add($"[Your field] totals mismatch detected across reports");
    
    // OR add to warnings if this doesn't block
    // warnings.Add($"[Your field] totals mismatch - review data");
}
```

## Step 3: Add Current Values to ProfitMasterUpdateEndpoint

**Location:** `Demoulas.ProfitSharing.Endpoints/Endpoints/Reports/YearEnd/ProfitMaster/ProfitMasterUpdateEndpoint.cs`

In the `HandleAsync` method, add to the `currentValues` dictionary:

```csharp
var currentValues = new Dictionary<string, decimal>
{
    // ... existing fields
    
    // Your new fields
    ["REPORT_CODE_1.FieldName1"] = await GetYourFieldValue1Async(req.ProfitYear, ct),
    ["REPORT_CODE_2.FieldName2"] = await GetYourFieldValue2Async(req.ProfitYear, ct),
};
```

## Step 4: Ensure Source Reports Archive the Fields

**Location:** Source report response DTOs

Add `[YearEndArchiveProperty]` attribute to the fields:

```csharp
[YearEndArchiveProperty]
public decimal YourFieldName { get; set; }
```

**Location:** Source report endpoints

Ensure endpoints use `IAuditService.ArchiveCompletedReportAsync`:

```csharp
var result = await _auditService.ArchiveCompletedReportAsync(
    "REPORT_CODE", 
    req.ProfitYear, 
    req,
    (audit, _, ct) => _service.GetReportAsync(audit, ct),
    ct);
```

## Example: Adding Beginning Balance Validation

### Step 1: Add Validation Method

```csharp
/// <summary>
/// Validates the Beginning Balance cross-references (PAY443, PAY444).
/// Validation Rule: PAY444.BeginningBalance = PAY443.BeginningBalance
/// </summary>
private async Task<CrossReferenceValidationGroup> ValidateBeginningBalanceGroupAsync(
    short profitYear,
    Dictionary<string, decimal> currentValues,
    HashSet<string> validatedReports,
    CancellationToken cancellationToken)
{
    var validations = new List<CrossReferenceValidation>();

    // PAY443.BeginningBalance
    var pay443Validation = await ValidateSingleFieldAsync(
        profitYear, "PAY443", "BeginningBalance", currentValues, cancellationToken);
    validations.Add(pay443Validation);
    validatedReports.Add("PAY443");

    bool allValid = validations.All(v => v.IsValid);
    string summary = allValid
        ? "Beginning balance totals are in sync."
        : "Beginning balance totals mismatch detected.";

    return new CrossReferenceValidationGroup
    {
        GroupName = "Beginning Balance",
        Description = "Cross-validation of beginning balance totals",
        IsValid = allValid,
        Validations = validations,
        Summary = summary,
        Priority = "High",
        ValidationRule = "PAY444.BeginningBalance = PAY443.BeginningBalance"
    };
}
```

### Step 2: Call in ValidateMasterUpdateCrossReferencesAsync

```csharp
// Group 5: Beginning Balance (2-way match)
var beginningBalanceGroup = await ValidateBeginningBalanceGroupAsync(
    profitYear, currentValues, validatedReports, cancellationToken);
validationGroups.Add(beginningBalanceGroup);
totalValidations += beginningBalanceGroup.Validations.Count;
passedValidations += beginningBalanceGroup.Validations.Count(v => v.IsValid);
failedValidations += beginningBalanceGroup.Validations.Count(v => !v.IsValid);
if (!beginningBalanceGroup.IsValid)
{
    warnings.Add($"Beginning balance mismatch - review PAY443 and PAY444 values");
}
```

### Step 3: Add Current Value

```csharp
var currentValues = new Dictionary<string, decimal>
{
    // ... existing fields
    ["PAY443.BeginningBalance"] = await GetBeginningBalanceAsync(req.ProfitYear, ct),
};
```

### Step 4: Ensure PAY443 Archives BeginningBalance

```csharp
// In ForfeituresAndPointsForYearResponseWithTotals.cs
[YearEndArchiveProperty]
public decimal BeginningBalance { get; set; }
```

## Testing Your Validation Group

### Unit Test Pattern

```csharp
[Fact]
public async Task ValidateYourFieldGroup_AllMatch_ReturnsValid()
{
    // Arrange
    var currentValues = new Dictionary<string, decimal>
    {
        ["REPORT1.Field1"] = 1000.00m,
        ["REPORT2.Field2"] = 1000.00m
    };
    
    // Mock archived checksums to match current values
    // ... setup mocks
    
    // Act
    var result = await _service.ValidateMasterUpdateCrossReferencesAsync(
        2025, currentValues, CancellationToken.None);
    
    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.IsValid.ShouldBeTrue();
    result.Value.ValidationGroups
        .First(g => g.GroupName == "[Your Group Name]")
        .IsValid.ShouldBeTrue();
}

[Fact]
public async Task ValidateYourFieldGroup_Mismatch_ReturnsInvalid()
{
    // Arrange
    var currentValues = new Dictionary<string, decimal>
    {
        ["REPORT1.Field1"] = 1000.00m,
        ["REPORT2.Field2"] = 999.00m  // Intentional mismatch
    };
    
    // Act
    var result = await _service.ValidateMasterUpdateCrossReferencesAsync(
        2025, currentValues, CancellationToken.None);
    
    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.IsValid.ShouldBeFalse();
    result.Value.ValidationGroups
        .First(g => g.GroupName == "[Your Group Name]")
        .IsValid.ShouldBeFalse();
}
```

## Checklist for Adding New Validation Group

- [ ] Create validation method in `ChecksumValidationService`
- [ ] Call validation method in `ValidateMasterUpdateCrossReferencesAsync`
- [ ] Determine priority: Critical (blocks) or High (warns)
- [ ] Add current values to `ProfitMasterUpdateEndpoint`
- [ ] Ensure source reports have `[YearEndArchiveProperty]` attributes
- [ ] Ensure source endpoints use `ArchiveCompletedReportAsync`
- [ ] Add unit tests for happy path and mismatch scenarios
- [ ] Update `REPORT_CROSSREFERENCE_MATRIX.md` with validation rule
- [ ] Test with real data
- [ ] Document in PR description

## Common Patterns

### 2-Way Validation
```csharp
// Validates Field1 = Field2
var report1 = await ValidateSingleFieldAsync(...);
var report2 = await ValidateSingleFieldAsync(...);
```

### 3-Way Validation
```csharp
// Validates Field1 = Field2 = Field3
var report1 = await ValidateSingleFieldAsync(...);
var report2 = await ValidateSingleFieldAsync(...);
var report3 = await ValidateSingleFieldAsync(...);
```

### 4-Way Validation (like Distributions)
```csharp
// Validates Field1 = Field2 = Field3 = Field4
var report1 = await ValidateSingleFieldAsync(...);
var report2 = await ValidateSingleFieldAsync(...);
var report3 = await ValidateSingleFieldAsync(...);
var report4 = await ValidateSingleFieldAsync(...);
```

### Calculated Field Validation
```csharp
// Validates Field1 + Field2 = Field3
// Calculate expected value first, then validate
decimal expectedValue = currentValues["REPORT1.Field1"] + currentValues["REPORT1.Field2"];
var calculatedValidation = new CrossReferenceValidation
{
    FieldName = "CalculatedField",
    ReportCode = "REPORT1",
    IsValid = Math.Abs(expectedValue - currentValues["REPORT1.Field3"]) < 0.01m,
    CurrentValue = currentValues["REPORT1.Field3"],
    ExpectedValue = expectedValue,
    Variance = currentValues["REPORT1.Field3"] - expectedValue,
    Message = "Calculated field validation"
};
```

## Priority Levels

| Priority | When to Use | Behavior |
|----------|-------------|----------|
| **Critical** | Financial discrepancies that affect Year-End | Adds to `CriticalIssues`, sets `BlockMasterUpdate=true` |
| **High** | Important but non-blocking mismatches | Adds to `Warnings`, doesn't block |
| **Medium** | Informational checks | Adds to `Warnings` |
| **Low** | Optional validations | Adds to `Warnings` |

## Troubleshooting

**Validation always fails**
- Check that source report is being archived (`[YearEndArchiveProperty]` on fields)
- Verify report code matches exactly (case-sensitive)
- Ensure field name matches property name exactly
- Check that current value is being provided in dictionary

**Current value is null**
- Verify dictionary key format: `"ReportCode.FieldName"`
- Check that value is being calculated/retrieved correctly
- Ensure async method is awaited

**Validation doesn't appear in results**
- Verify you called your validation method in `ValidateMasterUpdateCrossReferencesAsync`
- Check that you added it to `validationGroups` list
- Ensure you're incrementing counters

---

**For More Information:** See `PS-MASTER_UPDATE_CROSSREF_VALIDATION_IMPLEMENTATION.md`
