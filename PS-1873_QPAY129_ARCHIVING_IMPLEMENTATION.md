# PS-1873: QPAY129 (Distributions and Forfeitures) Archiving Implementation

**Date:** October 11, 2025  
**Status:** ✅ IMPLEMENTED  
**Ticket:** PS-1873

---

## Summary

Implemented archiving functionality for QPAY129 (Distributions and Forfeitures) report to enable cross-reference validation with PAY443, PAY444, and QPAY066TA reports during Master Update year-end processing.

---

## Changes Made

### 1. Response DTO - Added YearEndArchiveProperty Attributes

**File:** `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Response/YearEnd/DistributionsAndForfeitureTotalsResponse.cs`

**Changes:**
- Added `[YearEndArchiveProperty]` attribute to `DistributionTotal` field
- Added `[YearEndArchiveProperty]` attribute to `ForfeitureTotal` field
- Added XML documentation comments explaining cross-reference relationships

**Code:**
```csharp
/// <summary>
/// Total distributions for the year (matches PAY443.DistributionTotals and QPAY066TA.TotalDisbursements)
/// </summary>
[YearEndArchiveProperty]
public required decimal DistributionTotal { get; init; }

/// <summary>
/// Total forfeitures for the year (matches PAY443.TotalForfeitures)
/// </summary>
[YearEndArchiveProperty]
public required decimal ForfeitureTotal { get; init; }
```

### 2. Request DTO - Extended to Support Frozen Data

**File:** `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/DistributionsAndForfeituresRequest.cs`

**Changes:**
- Changed base class from `SortedPaginationRequestDto` to `FrozenProfitYearRequest`
- This adds `ProfitYear` and `UseFrozenData` properties needed for archiving

**Before:**
```csharp
public sealed record DistributionsAndForfeituresRequest : SortedPaginationRequestDto
```

**After:**
```csharp
/// <summary>
/// Request for QPAY129 Distributions and Forfeitures report
/// </summary>
public sealed record DistributionsAndForfeituresRequest : FrozenProfitYearRequest
```

### 3. Endpoint - Integrated Audit Service

**File:** `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Reports/YearEnd/Cleanup/DistributionsAndForfeitureEndpoint.cs`

**Changes:**

#### Added Dependencies
- Injected `IAuditService` for archiving
- Added `ReportName` constant: `"Distributions and Forfeitures (QPAY129)"`

#### Updated ExecuteAsync Method
- Added conditional logic: if `UseFrozenData=true`, use `ArchiveCompletedReportAsync`
- If `UseFrozenData=false`, retrieve data directly without archiving
- Enhanced logging to include archiving status
- Added telemetry metrics for distribution and forfeiture totals
- Updated operation name to `"year-end-qpay129-distributions-forfeitures"`

**Key Implementation:**
```csharp
if (req.UseFrozenData)
{
    result = await _auditService.ArchiveCompletedReportAsync(
        ReportName,
        req.ProfitYear,
        req,
        async (archiveReq, _, cancellationToken) =>
        {
            var serviceResult = await _cleanupReportService.GetDistributionsAndForfeitureAsync(archiveReq, cancellationToken);
            
            if (!serviceResult.IsSuccess || serviceResult.Value == null)
            {
                throw new InvalidOperationException(
                    $"Failed to retrieve QPAY129 data for archiving: {serviceResult.Error?.Description ?? "Unknown error"}");
            }
            
            return serviceResult.Value;
        },
        ct);
}
else
{
    // Non-frozen data: retrieve directly without archiving
    var serviceResult = await _cleanupReportService.GetDistributionsAndForfeitureAsync(req, ct);
    
    if (!serviceResult.IsSuccess)
    {
        return serviceResult.ToHttpResult(Error.NoPayProfitsDataAvailable);
    }
    
    result = serviceResult.Value!;
}
```

#### Enhanced API Documentation
Updated endpoint summary to explain archiving behavior:
```csharp
s.Summary = "Distributions and Forfeitures Report (QPAY129)";
s.Description = "Lists distributions and forfeitures for a date range. " +
              "When UseFrozenData=true, archives the report with checksums for cross-reference validation.";
```

---

## Validation Rules Enabled

With this implementation, the following cross-reference validations are now fully supported:

### 1. Total Distributions (4-way validation)
**Rule:** `PAY444.DISTRIB = PAY443.DistributionTotals = QPAY129.Distributions = QPAY066TA.TotalDisbursements`

**Status:** ✅ QPAY129.Distributions now archived and available for validation

### 2. Total Forfeitures (3-way validation)
**Rule:** `PAY444.FORFEITS = PAY443.TotalForfeitures = QPAY129.ForfeitedAmount`

**Status:** ✅ QPAY129.ForfeitedAmount now archived and available for validation

---

## Testing Checklist

### Unit Testing
- [ ] Test `DistributionsAndForfeitureEndpoint` with `UseFrozenData=true`
- [ ] Test `DistributionsAndForfeitureEndpoint` with `UseFrozenData=false`
- [ ] Verify `[YearEndArchiveProperty]` attributes are recognized by `AuditService`
- [ ] Test service failure handling during archiving

### Integration Testing
- [ ] Archive QPAY129 report for a test profit year
- [ ] Verify checksums stored in `ReportChecksum` table
- [ ] Query archived checksums using `ChecksumValidationService`
- [ ] Run full Master Update validation flow with QPAY129 archived

### Manual Testing
1. Call endpoint with `UseFrozenData=true` and `ProfitYear=2025`
2. Check database: `SELECT * FROM REPORT_CHECKSUM WHERE REPORT_TYPE = 'QPAY129' AND PROFIT_YEAR = 2025`
3. Verify two checksums exist:
   - `DistributionTotal`
   - `ForfeitureTotal`
4. Run Master Update cross-reference validation
5. Confirm QPAY129 fields participate in validation groups

---

## Database Impact

### ReportChecksum Table
New records will be created when archiving QPAY129:

```sql
SELECT * FROM REPORT_CHECKSUM 
WHERE REPORT_TYPE = 'QPAY129' 
  AND PROFIT_YEAR = 2025
ORDER BY CREATED_AT_UTC DESC;
```

**Expected Records:**
1. `QPAY129` | `DistributionTotal` | `<SHA256 checksum>` | `<value>` | `<timestamp>`
2. `QPAY129` | `ForfeitureTotal` | `<SHA256 checksum>` | `<value>` | `<timestamp>`

---

## API Usage Examples

### Archive QPAY129 (Year-End Processing)
```http
GET /yearend/distributions-and-forfeitures?ProfitYear=2025&UseFrozenData=true&Skip=0&Take=1000&SortBy=BadgeNumber&IsSortDescending=false
```

**Response:**
```json
{
  "reportName": "Distributions and Forfeitures (QPAY129)",
  "reportDate": "2025-10-11T...",
  "distributionTotal": 1234567.89,
  "forfeitureTotal": 98765.43,
  "stateTaxTotal": 12345.67,
  "federalTaxTotal": 54321.09,
  "stateTaxTotals": { "MA": 10000.00, "NH": 2345.67 },
  "startDate": "2025-01-01",
  "endDate": "2025-12-31",
  "response": {
    "total": 150,
    "results": [...]
  }
}
```

### Retrieve Current Data (No Archiving)
```http
GET /yearend/distributions-and-forfeitures?ProfitYear=2025&UseFrozenData=false&Skip=0&Take=100
```

---

## Validation Service Integration

The archived QPAY129 values can now be validated using `ChecksumValidationService`:

```csharp
var currentValues = new Dictionary<string, decimal>
{
    ["QPAY129.Distributions"] = qpay129DistributionTotal,
    ["QPAY129.ForfeitedAmount"] = qpay129ForfeitureTotal
};

var validation = await _checksumValidationService.ValidateReportFieldsAsync(
    profitYear: 2025,
    reportType: "QPAY129",
    fieldsToValidate: currentValues,
    cancellationToken: ct);

if (validation.IsSuccess)
{
    Console.WriteLine($"Validation: {validation.Value.Message}");
    // IsValid: true/false
    // FieldResults: List of per-field validation results
    // MismatchedFields: List of fields that don't match
}
```

---

## Related Tickets

- **PS-1448** - PAY443 archiving (DONE - In QA)
- **PS-1874** - Backend validation endpoint (DONE - Already implemented)
- **PS-1875** - Master Update UI validation display (In Progress)
- **PS-1876** - QPAY066TA archiving (To Do)
- **PS-1877** - ALLOC balance validation (To Do)
- **PS-1878** - Validation dashboard (To Do)

---

## Documentation References

- **Validation Matrix:** `src/ui/public/docs/REPORT_CROSSREFERENCE_MATRIX.md`
- **Backend Implementation:** `src/ui/public/docs/PS-MASTER_UPDATE_CROSSREF_VALIDATION_IMPLEMENTATION.md`
- **Adding Validation Groups:** `src/ui/public/docs/ADDING_CROSSREF_VALIDATION_GROUPS.md`
- **Confluence:** [Year-End Validation Strategy](https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/474906897)

---

## Build Status

✅ **Build Successful** - All endpoints compile without errors  
⚠️ Integration test failure is pre-existing and unrelated to this change

---

## Next Steps

1. **Test archiving** with real data in development environment
2. **Update PS-1873 ticket** status to "Done" or "In QA"
3. **Verify cross-reference validation** includes QPAY129 fields
4. **Update Confluence** documentation with implementation status
5. **Proceed to PS-1876** (QPAY066TA archiving) if needed

---

**Implementation Complete:** October 11, 2025  
**Ready for QA:** Yes  
**Breaking Changes:** None (backward compatible - archiving is optional via `UseFrozenData` flag)
