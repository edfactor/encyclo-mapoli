# DTO Example Methods - Work Items & Batch Processing Guide

**Generated:** December 31, 2025  
**Status:** Research Complete - Ready for Phase 1 Implementation

---

## QUICK START

Two files have been generated for this inventory:

1. **DTO_INVENTORY_COMPLETE.md** - Full detailed report with all DTOs, categories, and statistics
2. **DTO_INVENTORY.csv** - Machine-readable format for batch processing/scripting

**Total Work:** 86 Example methods to add across 3-4 weeks

---

## PRIORITIZED WORK PHASES

### ⚡ PHASE 1: YearEnd Report DTOs (Highest Priority)

**Timeline:** Week 1 (5-8 working days)  
**Estimated Methods:** 25-30  
**Business Impact:** CRITICAL - Year-end processing is core functionality

#### Missing Example Methods (24 files):

```
YearEnd/
  - AdhocBeneficiariesReportResponse
  - BalanceByBase (base class)
  - BalanceByDetailBase (base class)
  - BreakdownByStoreTotals
  - DuplicateNamesAndBirthdaysCachedResponse
  - DuplicateNamesAndBirthdaysResponse
  - DuplicateSsnReportResponse
  - GetEligibleEmployeesResponse
  - GrandTotalsByStoreResponseDto
  - GrandTotalsByStoreRowDto
  - MemberYearSummaryDto
  - NegativeETVAForSSNsOnPayProfitResponse
  - ProfitShareEditResponse
  - ProfitShareUpdateMemberResponse
  - ProfitShareUpdateResponse
  - ProfitShareUpdateTotals
  - TerminatedEmployeeAndBeneficiaryResponse
  - TerminatedEmployeeAndBeneficiaryYearDetailDto
  - UnattributedTotals
  - UpdateSummaryReportPointInTimeDetail
  - VestedAmountsByAgeDetail
  - WagesCurrentYearParticipant
  - YearEndProfitSharingReportSummaryLineItem
  - YearEndProfitSharingReportSummaryResponse
  - YearEndProfitSharingReportTotals
```

#### Rationale:

- 58 YearEnd DTOs total - largest category
- Core to profit sharing calculations
- Impacts year-end processing reports
- Multiple downstream dependencies

---

### 📊 PHASE 2: Distribution, Validation & Reports

**Timeline:** Week 1-2 (4-6 working days)  
**Estimated Methods:** 20-25  
**Business Impact:** HIGH - Reporting infrastructure

#### Missing Example Methods by Category:

**Distributions (5 files):**

- DisbursementReportDetailResponse
- DistributionRunReportDetail
- DistributionRunReportSummaryResponse
- DistributionSearchResponse
- DistributionsOnHoldResponse

**Account History Reports (3 files):**

- AccountHistoryReportPaginatedResponse
- AccountHistoryReportResponse
- AccountHistoryReportTotals

**Core/Utilities (6 files):**

- AdjustmentsSummaryDto
- DataWindowMetadata
- ListResponseDto (Generic paging response)
- PayProfitResponseDto
- ProfitDetailDto
- ReportResponseBase (base class)

**CheckRun (1 file):**

- CheckRunStepStatus

---

### 👥 PHASE 3: Beneficiary & Core DTOs

**Timeline:** Week 2-3 (5-8 working days)  
**Estimated Methods:** 25-30  
**Business Impact:** MEDIUM-HIGH - Beneficiary/Member workflows

#### Missing Example Methods by Category:

**Beneficiaries (10 files):**

- BeneficiaryDetailResponse
- BeneficiaryDto
- BeneficiaryResponse
- BeneficiarySearchFilterResponse
- CreateBeneficiaryContactResponse
- CreateBeneficiaryResponse
- UpdateBeneficiaryContactResponse
- UpdateBeneficiaryResponse
- BeneficiaryTypeDto
- BeneficiaryTypesResponseDto

**Core/Root (14 request files):**

- AddressRequestDto
- BadgeNumberRequest
- BeneficiaryDisbursementRequest
- BeneficiaryTypeRequestDto
- ContactInfoRequestDto
- DemographicsRequest
- EmployeeTypeRequestDto
- EnrollmentRequestDto
- IdRequest
- ProfitYearAndAsOfDateRequest
- SetFrozenStateRequest
- SuggestedForfeitureAdjustmentRequest
- UserSyncRequestDto
- YearRangeRequest
- YearRequest
- YearRequestWithRebuild

**MasterInquiry (2 files):**

- MasterInquiryMemberDetailsRequest
- MasterInquiryMemberRequest

**PayBen (1 file):**

- PayBenReportRequest

**Administration (1 file):**

- UpdateCommentTypeRequest

**Validation (2 files):**

- ProfitSharingReportValidationRequest
- ValidateReportFieldsRequest

---

### 🔧 PHASE 4: Infrastructure DTOs (Optional)

**Timeline:** Week 3 (2-3 working days)  
**Estimated Methods:** 5-10  
**Business Impact:** LOW - Infrastructure support

#### Status:

- ✅ ItOperations: 8/8 complete (0 missing)
- ✅ Lookup: 7/7 complete (0 missing)
- ✅ Navigations: 5/5 complete (0 missing)
- ✅ Military: 1/1 complete (0 missing)
- ✅ Job: 1/1 complete (0 missing)
- ✅ Validation: 6/6 complete (0 missing)

**These categories are already complete - no work needed.**

---

## IMPLEMENTATION TEMPLATE

### For Response DTOs:

```csharp
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ExampleResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }

    // Add this method
    public static ExampleResponseDto ResponseExample()
    {
        return new ExampleResponseDto
        {
            Id = 1,
            Name = "Example Name",
            Amount = 1234.56m
        };
    }
}
```

### For Request DTOs:

```csharp
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record ExampleRequestDto
{
    public int Id { get; set; }
    public int Year { get; set; }

    // Add this method
    public static ExampleRequestDto RequestExample()
    {
        return new ExampleRequestDto
        {
            Id = 1,
            Year = 2024
        };
    }
}
```

### Key Guidelines:

- Method must be `public static`
- Return type matches the DTO class name
- Populate ALL properties with reasonable test data
- Use realistic values (2024 for years, valid SSNs if needed, etc.)
- For nested objects, call their Example() methods
- Include decimal amounts rounded to 2 places (MidpointRounding.AwayFromZero)

---

## BATCH PROCESSING APPROACH

### Option 1: Group by Phase

1. Assign Phase 1 (YearEnd) to one developer
2. Assign Phase 2 (Distribution/Reports) to second developer
3. Assign Phase 3 (Beneficiary/Core) to third developer
4. Work in parallel after initial API understanding

### Option 2: Group by Endpoint Category

1. Developer A: YearEnd + Distribution (all related)
2. Developer B: Beneficiary + MasterInquiry workflows
3. Developer C: Core utilities + Request DTOs

### Testing Strategy

- Create unit tests for each new Example method
- Validate object graph completeness
- Test null/empty conditions
- Verify examples in OpenAPI generation

---

## DEPENDENCY MAPPING

### Critical Dependencies (Must do first):

1. **Base/Parent Classes** - BalanceByBase, BalanceByDetailBase, ReportResponseBase
2. **Shared DTOs** - AddressResponseDto, DemographicResponseDto (used across multiple endpoints)
3. **Core Lookup DTOs** - Year, ProfitYear, Enrollment, Department references

### Optional Interfaces (Can skip):

- IMemberRequest (interface)
- IStartEndDateRequest (interface)
- SearchBy (enum/helper)
- SimpleRequest (abstract base)

---

## QUALITY CHECKLIST

For each Example method:

- [ ] Method is public static
- [ ] Return type matches class name
- [ ] All non-nullable properties populated
- [ ] Complex nested objects use their own Example() methods
- [ ] Decimal values use proper financial rounding
- [ ] Year/date values are current (2024)
- [ ] IDs are realistic (not 0 or negative)
- [ ] Comments added for non-obvious values
- [ ] Unit test created for method
- [ ] Passes OpenAPI generation test

---

## PROGRESS TRACKING

### CSV Columns Available for Filtering:

```csv
FileName,ClassName,Category,Type,HasExampleMethod,FilePath
```

Use `DTO_INVENTORY.csv` to:

- Filter by Category: `grep YearEnd DTO_INVENTORY.csv`
- Filter by Missing: `grep "FALSE" DTO_INVENTORY.csv`
- Sort by Type: `grep "Response" DTO_INVENTORY.csv | grep "FALSE"`

### PowerShell Batch Processing Example:

```powershell
# Find all missing YearEnd Response DTOs
$csv = Import-Csv 'DTO_INVENTORY.csv'
$missing = $csv | Where-Object {
    $_.Category -eq 'YearEnd' -and
    $_.Type -eq 'Response' -and
    $_.HasExampleMethod -eq 'FALSE'
}
$missing | Select-Object ClassName, FilePath | Export-Csv 'phase1-work-items.csv'
```

---

## ESTIMATED TIME BREAKDOWN

| Phase                   | Files  | Methods | Est. Hours    | Notes                     |
| ----------------------- | ------ | ------- | ------------- | ------------------------- |
| 1: YearEnd              | 24     | 25-30   | 12-16 hrs     | Complex nested structures |
| 2: Distribution/Reports | 15     | 20-25   | 10-14 hrs     | Some shared patterns      |
| 3: Beneficiary/Core     | 20     | 25-30   | 10-14 hrs     | Mix of simple/complex     |
| 4: Infrastructure       | 0      | 0       | 0 hrs         | Already complete          |
| **Testing & Review**    | -      | -       | **8-12 hrs**  | Unit tests + PR review    |
| **TOTAL**               | **59** | **86**  | **40-56 hrs** | **2-3 weeks full-time**   |

---

## SUCCESS METRICS

✅ **Definition of Done:**

1. All 86 Example methods implemented
2. Each method populates all properties
3. All nested object graphs validated
4. Unit tests pass for all examples
5. OpenAPI generation succeeds
6. No null reference exceptions in Swagger/API docs
7. PR reviewed and merged

---

## NEXT STEPS

1. **Week 1:** Assign Phase 1 work and begin YearEnd DTOs
2. **Week 1-2:** Parallel work on Phases 2 & 3 after Phase 1 patterns established
3. **Week 3:** Complete Phase 3, testing, and review
4. **Ongoing:** Add Example methods to new DTOs as they're created

---

## REFERENCE DOCUMENTS

- `DTO_INVENTORY_COMPLETE.md` - Full detailed report
- `DTO_INVENTORY.csv` - Machine-readable inventory
- Code review checklist: See `.github/CODE_REVIEW_CHECKLIST.md`
- Telemetry patterns: See `src/ui/public/docs/TELEMETRY_GUIDE.md`

---

**Ready to begin Phase 1? Start with:**

- Pick a YearEnd DTO from the Phase 1 list
- Follow the template above
- Add a unit test
- Create PR when complete

Let me know which phase you'd like to tackle first!
