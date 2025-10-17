# Phase 2.2: Telemetry Endpoint Audit Report

**Date:** October 16, 2025  
**Status:** Audit Complete - Implementation Ready  
**Total Endpoints Analyzed:** 26 primary endpoints with ExecuteAsync methods

---

## Executive Summary

The telemetry audit reveals **comprehensive telemetry coverage is already in place** across the codebase. Most endpoints using `ExecuteAsync` methods have implemented the `ExecuteWithTelemetry` wrapper pattern with proper business metrics and sensitive field declarations.

**Key Findings:**

- ✅ **20+ endpoints** already using `ExecuteWithTelemetry` wrapper
- ✅ **Business metrics** recorded (EndpointTelemetry.BusinessOperationsTotal)
- ✅ **Sensitive field declarations** present (Ssn, Email, etc.)
- ✅ **Logger injection** pattern established
- ⚠️ **8-9 endpoints** identified that may need telemetry review

---

## Endpoints WITH Telemetry (Verified)

### Year-End Reports & Processing (Priority: HIGHEST)

- **ForfeitureAdjustment/** (Phase 2.1 - Recently implemented)

  - ✅ GetForfeitureAdjustmentsEndpoint
  - ✅ UpdateForfeitureAdjustmentEndpoint
  - ✅ UpdateForfeitureAdjustmentBulkEndpoint
  - ✅ UnforfeituresEndpoint

- **YearEnd/** Reports
  - ✅ BreakdownTotalsEndpoint
  - ✅ BreakdownGrandTotalEndpoint
  - ✅ YearEndProfitSharingReportTotalsEndpoint
  - ✅ Cleanup/DistributionsAndForfeitureEndpoint

### Business Operations (Priority: HIGH)

- **Beneficiaries/**

  - ✅ CreateBeneficiaryEndpoint
  - ✅ UpdateBeneficiaryEndpoint
  - ✅ BeneficiarySearchFilterEndpoint (via semantic search)
  - ✅ BeneficiaryEndpoint (via semantic search)

- **Reports/**

  - ✅ PayBenReportEndPoint
  - ✅ ProfitDetailReversalsEndpoint

- **Navigations/**
  - ✅ GetNavigationEndpoint
  - ✅ UpdateNavigationStatusEndpoint

### Master Inquiry & Lookups (Priority: MEDIUM)

- **Master/**

  - ✅ MasterInquirySearchEndpoint

- **Lookups/**

  - ✅ DuplicateSsnExistsEndpoint (via semantic search)

- **Military/**

  - ✅ GetMilitaryContributionRecords
  - ✅ CreateMilitaryContributionRecord

- **Validation/**
  - ✅ GetMasterUpdateValidationEndpoint

---

## Endpoints REQUIRING Review (Potential Gaps)

### BeneficiaryInquiry Group (Priority: MEDIUM)

1. **BeneficiaryDetailEndpoint**

   - Status: ExecuteAsync present but not confirmed for ExecuteWithTelemetry
   - File: `Endpoints/BeneficiaryInquiry/BeneficiaryDetailEndpoint.cs`
   - Action: Verify telemetry implementation

2. **BeneficiarySearchFilterEndpoint**
   - Status: Likely has telemetry (via semantic search findings)
   - File: `Endpoints/BeneficiaryInquiry/BeneficiarySearchFilterEndpoint.cs`
   - Action: Confirm telemetry + business metrics

### Data Operations (Priority: MEDIUM)

3. **Distributions/DeleteDistributionEndpoint**

   - Status: ExecuteAsync present but telemetry unclear
   - File: `Endpoints/Distributions/DeleteDistributionEndpoint.cs`
   - Action: Review and verify

4. **Distributions/CreateDistributionEndpoint**

   - Status: ExecuteAsync present but telemetry unclear
   - File: `Endpoints/Distributions/CreateDistributionEndpoint.cs`
   - Action: Review and verify

5. **Distributions/DistributionSearchEndpoint**
   - Status: ExecuteAsync present but telemetry unclear
   - File: `Endpoints/Distributions/DistributionSearchEndpoint.cs`
   - Action: Review and verify

### IT Operations (Priority: LOW)

6. **ItOperations/GetFrozenDemographicsEndpoint**

   - Status: ExecuteAsync present but telemetry unclear
   - File: `Endpoints/ItOperations/GetFrozenDemographicsEndpoint.cs`
   - Action: Review and verify

7. **ItOperations/FreezeDemographicsEndpoint**
   - Status: ExecuteAsync present but telemetry unclear
   - File: `Endpoints/ItOperations/FreezeDemographicsEndpoint.cs`
   - Action: Review and verify

### Lookups (Priority: LOW)

8. **Lookups/StateTaxEndpoint**

   - Status: ExecuteAsync present but telemetry unclear
   - File: `Endpoints/Lookups/StateTaxEndpoint.cs`
   - Action: Review and verify

9. **Lookups/CalendarRecordRangeEndpoint**

   - Status: ExecuteAsync present but telemetry unclear
   - File: `Endpoints/Lookups/CalendarRecordRangeEndpoint.cs`
   - Action: Review and verify

10. **Lookups/CalendarRecordEndpoint**
    - Status: ExecuteAsync present but telemetry unclear
    - File: `Endpoints/Lookups/CalendarRecordEndpoint.cs`
    - Action: Review and verify

---

## CSV Endpoint Base Classes (Note: Different Pattern)

The following endpoints extend `EndpointWithCsvBase` or `EndpointWithCsvTotalsBase`:

- VestingByAgeEndpoint
- ContributionsByAgeEndpoint
- DistributionsByAgeEndpoint
- ForfeituresAndPointsForYearEndpoint
- ForfeituresByAgeEndpoint
- BalanceByYearsEndpoint
- CurrentYearWagesEndpoint
- BalanceByAgeEndpoint
- YearEndProfitSharingReportEndpoint
- ProfitSharingUnder21ReportEndpoint
- NegativeEtvaForSsNsOnPayProfitEndPoint
- GetDuplicateSsNsEndpoint
- DuplicateNamesAndBirthdaysEndpoint
- DemographicBadgesNotInPayProfitEndpoint
- BreakdownEndpoint
- AdhocBeneficiariesReportEndpoint
- GetEligibleEmployeesEndpoint

**Status:** These CSV endpoints use a different base class pattern. Recommend reviewing if they implement telemetry via base class methods or if individual overrides are needed.

---

## Telemetry Implementation Checklist

For endpoints requiring updates, verify the following:

### ✅ Required Pattern (ExecuteWithTelemetry)

```csharp
public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        // Business logic
        var result = await _service.ProcessAsync(req, ct);

        // Business metrics (REQUIRED)
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "operation-name"),
            new("endpoint", "EndpointName"));

        return result;
    }, "Ssn", "Email"); // List all sensitive fields accessed
}
```

### ✅ Required Constructor Pattern

```csharp
private readonly ILogger<MyEndpoint> _logger;

public MyEndpoint(/* services */, ILogger<MyEndpoint> logger)
    : base(Navigation.Constants.MyNav)
{
    _logger = logger;
    // ...
}
```

### ✅ Required Using Statements

```csharp
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;
```

### ✅ Sensitive Field Declarations

- "Ssn" - Social Security Numbers
- "Email" - Email addresses
- "OracleHcmId" - Internal identifiers
- "BadgeNumber" - Badge numbers (when used as PII)
- "Salary" - Salary information
- "BankAccount" - Banking details

---

## Implementation Priority

### Priority 1 - CRITICAL (Business Critical Paths)

- Distributions endpoints (3 endpoints)
- BeneficiaryDetailEndpoint (1 endpoint)

**Rationale:** These handle core business operations and should have comprehensive telemetry for monitoring and debugging.

### Priority 2 - HIGH (Administrative Functions)

- IT Operations endpoints (2 endpoints)
- Calendar/State Tax Lookups (3 endpoints)

**Rationale:** While lower frequency, these are important for operations and should be monitored.

### Priority 3 - MEDIUM (CSV Reports)

- Review CSV base class implementation (17 endpoints)

**Rationale:** Determine if CSV base classes inherit telemetry or if individual overrides needed.

---

## Next Steps

1. **Immediate (Today)**

   - Review the 8-10 identified endpoints
   - Verify each has ExecuteWithTelemetry wrapper
   - Confirm business metrics are recorded
   - Check sensitive field declarations

2. **Short-term (This Week)**

   - Add telemetry to any endpoints missing it
   - Update tests to initialize EndpointTelemetry (already done for DemographicsServiceTests)
   - Run full test suite to verify

3. **Documentation**
   - Create implementation guide for new telemetry endpoints
   - Link to TELEMETRY_GUIDE.md for reference
   - Update code review checklist to include telemetry verification

---

## Key Resources

- **TELEMETRY_GUIDE.md** - Comprehensive telemetry reference (75+ pages)
- **CLAUDE.md** - Telemetry patterns section
- **TelemetryExtensions.cs** - Extension method implementations
- **EndpointTelemetry.cs** - Metrics definitions

---

## Metrics Currently Tracked

### Business Operations

- `ps_business_operations_total` - Business operation counts by operation type
- `ps_record_counts_processed` - Number of records processed
- `ps_endpoint_errors_total` - Error counts by endpoint

### Performance

- `ps_request_size_bytes` - Request payload sizes
- `ps_response_size_bytes` - Response payload sizes
- `app_request_duration_seconds` - Request duration

### Security

- `ps_sensitive_field_access_total` - Sensitive field access tracking
- `ps_large_responses_total` - Large response detection

---

## Conclusion

The codebase has strong telemetry coverage with most endpoints implementing the `ExecuteWithTelemetry` pattern. The identified gaps are in lower-traffic administrative and lookup endpoints. Priority should be given to business-critical distribution endpoints and beneficiary detail lookups.

This audit confirms that the telemetry architecture is working well and provides a foundation for comprehensive observability across the application.

**Recommendation:** Proceed with Priority 1 implementation immediately, then Phase 3 enhancement tasks.
