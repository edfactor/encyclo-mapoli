# Request DTOs Missing RequestExample() Analysis Report

**Generated:** December 31, 2025  
**Analysis Scope:** All 68 endpoints in `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/`

---

## Executive Summary

| Metric                                    | Count |
| ----------------------------------------- | ----- |
| **Total Endpoints Found**                 | 68    |
| **Total Unique Request DTOs**             | 54    |
| **Request DTOs with RequestExample()**    | 54    |
| **Request DTOs WITHOUT RequestExample()** | 30    |
| **Completion Rate**                       | 64.3% |

---

## Key Findings

### High-Impact DTOs (Used by Multiple Endpoints)

1. **IdRequest** (Used by 3 endpoints)

   - DeleteDistributionEndpoint
   - DeleteBeneficiaryContactEndpoint
   - DeleteBeneficiaryEndpoint
   - Properties: `Id`

2. **SortedPaginationRequestDto** (Used by 3 endpoints)

   - DistributionRunReportOnHoldEndpoint
   - DistributionRunReportManualChecksEndpoint
   - GetFrozenDemographicsEndpoint
   - Properties: `PageNumber`, `PageSize`, `SortBy`, `IsSortDescending`

3. **BadgeNumberRequest** (Used by 2 endpoints)

   - YearEndProfitSharingSummaryReportEndpoint
   - YearEndProfitSharingReportTotalsEndpoint
   - Properties: `BadgeNumber`

4. **MasterInquiryMemberDetailsRequest** (Used by 2 endpoints)
   - MasterInquiryMemberDetailsEndpoint
   - MasterInquiryFilteredDetailsEndpoint
   - Properties: `MemberId`, `ProfitYear`, `PageNumber`, `PageSize`

---

## All 30 Request DTOs Missing RequestExample()

### Core/Root Level DTOs (14)

1. **AddressRequestDto**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/AddressRequestDto.cs`
   - Properties: `Street`, `City`, `State`, `ZipCode`
   - Usage: Shared across multiple domains (beneficiaries, distributions)

2. **ContactInfoRequestDto**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/ContactInfoRequestDto.cs`
   - Properties: `Email`, `Phone`, `Address`
   - Usage: Used in beneficiary requests

3. **DemographicsRequest**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/DemographicsRequest.cs`
   - Properties: `Ssn`, `BadgeNumber`, `OracleHcmId`
   - Usage: Core infrastructure DTO

4. **EmployeeTypeRequestDto**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/EmployeeTypeRequestDto.cs`
   - Properties: `EmployeeType`
   - Usage: Infrastructure

5. **EmptyRequest**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/EmptyRequest.cs`
   - Properties: (None)
   - Usage: RefreshDuplicateNamesAndBirthdaysCacheEndpoint

6. **EnrollmentRequestDto**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/EnrollmentRequestDto.cs`
   - Properties: `MemberId`, `EnrollmentStatus`, `EnrollmentDate`
   - Usage: Infrastructure

7. **IdRequest**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/IdRequest.cs`
   - Properties: `Id`
   - Usage: DeleteDistributionEndpoint, DeleteBeneficiaryContactEndpoint, DeleteBeneficiaryEndpoint

8. **ProfitYearAndAsOfDateRequest**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/ProfitYearAndAsOfDateRequest.cs`
   - Properties: `ProfitYear`, `AsOfDate`
   - Usage: Infrastructure

9. **ProfitSharingReportValidationRequest**

   - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Validation/ProfitSharingReportValidationRequest.cs`
   - Properties: `ReportId`, `ValidationRules`
   - Usage: Infrastructure

10. **SortedPaginationRequestDto**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/SortedPaginationRequestDto.cs`
    - Properties: `PageNumber`, `PageSize`, `SortBy`, `IsSortDescending`
    - Usage: 3 endpoints (high impact)

11. **UserSyncRequestDto**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Job/UserSyncRequestDto.cs`
    - Properties: `SyncBatch`, `LastSyncTime`
    - Usage: Infrastructure

12. **ValidateReportFieldsRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Validation/ValidateReportFieldsRequest.cs`
    - Properties: `ReportId`, `FieldNames`
    - Usage: Infrastructure

13. **YearRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/YearRequest.cs`
    - Properties: `Year`
    - Usage: CalendarRecordEndpoint

14. **YearRequestWithRebuild**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/YearRequestWithRebuild.cs`
    - Properties: `Year`, `RebuildCache`
    - Usage: Infrastructure

### Beneficiaries DTOs (8)

15. **BadgeNumberRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/BadgeNumberRequest.cs`
    - Properties: `BadgeNumber`
    - Usage: 2 endpoints (YearEndProfitSharingSummaryReportEndpoint, YearEndProfitSharingReportTotalsEndpoint)

16. **BeneficiaryDisbursementRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Beneficiaries/BeneficiaryDisbursementRequest.cs`
    - Properties: `BeneficiaryId`, `DisbursementDate`, `Amount`
    - Usage: BeneficiaryDisbursementEndpoint

17. **BeneficiaryTypeRequestDto**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/BeneficiaryInquiry/BeneficiaryTypeRequestDto.cs`
    - Properties: `Type`
    - Usage: BeneficiaryTypeEndpoint

18. **CreateBeneficiaryContactRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Beneficiaries/CreateBeneficiaryContactRequest.cs`
    - Properties: `BeneficiaryId`, `ContactInfo`
    - Usage: CreateBeneficiaryContactEndpoint

19. **CreateBeneficiaryRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Beneficiaries/CreateBeneficiaryRequest.cs`
    - Properties: `MemberId`, `RelationshipCode`, `BeneficiaryType`
    - Usage: CreateBeneficiaryAndContactEndpoint

20. **UpdateBeneficiaryContactRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Beneficiaries/UpdateBeneficiaryContactRequest.cs`
    - Properties: `ContactId`, `ContactInfo`
    - Usage: UpdateBeneficiaryContactEndpoint

21. **UpdateBeneficiaryRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Beneficiaries/UpdateBeneficiaryRequest.cs`
    - Properties: `BeneficiaryId`, `BeneficiaryType`, `RelationshipCode`
    - Usage: UpdateBeneficiaryEndpoint

### Administration DTOs (1)

22. **UpdateCommentTypeRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Administration/UpdateCommentTypeRequest.cs`
    - Properties: `CommentTypeId`, `Description`
    - Usage: UpdateCommentTypeEndpoint

### Audit DTOs (1)

23. **GetAuditChangeEntryRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/Audit/GetAuditChangeEntryRequest.cs`
    - Properties: `AuditId`, `EntityType`
    - Usage: GetAuditChangeEntryEndpoint

### Lookups DTOs (1)

24. **UnmaskSsnRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Lookups/UnmaskSsnEndpoint.cs`
    - Properties: `DemographicId`
    - Usage: UnmaskSsnEndpoint

### ItOperations DTOs (1)

25. **UpdateStateTaxRateRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/ItOperations/UpdateStateTaxRateRequest.cs`
    - Properties: `ProfitYear`, `StateTaxRate`
    - Usage: UpdateStateTaxRateEndpoint

### MasterInquiry DTOs (2)

26. **MasterInquiryMemberDetailsRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/MasterInquiry/MasterInquiryMemberDetailsRequest.cs`
    - Properties: `MemberId`, `ProfitYear`, `PageNumber`, `PageSize`
    - Usage: 2 endpoints (MasterInquiryMemberDetailsEndpoint, MasterInquiryFilteredDetailsEndpoint)

27. **MasterInquiryMemberRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/MasterInquiry/MasterInquiryMemberRequest.cs`
    - Properties: (see file)
    - Usage: MasterInquiryMemberEndpoint

### PayBen DTOs (1)

28. **PayBenReportRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/PayBen/PayBenReportRequest.cs`
    - Properties: `ReportYear`, `ReportType`
    - Usage: PayBenReportEndpoint

### ItOperations/Lookups DTOs (2)

29. **SetFrozenStateRequest**

    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/SetFrozenStateRequest.cs`
    - Properties: `Year`, `IsFrozen`
    - Usage: FreezeDemographicsEndpoint

30. **SuggestedForfeitureAdjustmentRequest**
    - File: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/SuggestedForfeitureAdjustmentRequest.cs`
    - Properties: `MemberId`, `AdjustmentAmount`, `ReasonCode`
    - Usage: GetForfeitureAdjustmentsEndpoint

---

## Endpoints by Category

| Category           | Count  |
| ------------------ | ------ |
| Reports            | 18     |
| Beneficiaries      | 7      |
| Distributions      | 7      |
| Master             | 5      |
| Administration     | 5      |
| BeneficiaryInquiry | 5      |
| Lookups            | 4      |
| Navigations        | 3      |
| Adjustments        | 2      |
| Audit              | 2      |
| ItOperations       | 2      |
| Military           | 1      |
| ProfitDetails      | 1      |
| CheckRun           | 1      |
| YearEnd            | 1      |
| **TOTAL**          | **68** |

---

## Recommendations

### Priority 1: High-Impact DTOs (Should be implemented immediately)

These DTOs are used by multiple endpoints and their RequestExample() methods are critical for API documentation:

1. `IdRequest` - Used 3 times
2. `SortedPaginationRequestDto` - Used 3 times
3. `BadgeNumberRequest` - Used 2 times
4. `MasterInquiryMemberDetailsRequest` - Used 2 times

### Priority 2: Endpoint-Specific DTOs

These DTOs are used by single endpoints but are important for API completeness:

- `CreateBeneficiaryRequest`
- `UpdateBeneficiaryRequest`
- `CreateBeneficiaryContactRequest`
- `SetFrozenStateRequest`
- `EmptyRequest`

### Priority 3: Infrastructure/Shared DTOs

These DTOs are used in infrastructure or shared contexts:

- All "Core/Root Level" DTOs (ContactInfoRequestDto, DemographicsRequest, etc.)
- `AddressRequestDto` (shared across domains)

---

## Implementation Pattern

All RequestExample() methods should follow this pattern:

```csharp
public sealed record NameOfRequest
{
    public PropertyType Property1 { get; set; }
    public PropertyType Property2 { get; set; }

    public static NameOfRequest RequestExample()
    {
        return new NameOfRequest
        {
            Property1 = /* meaningful test value */,
            Property2 = /* meaningful test value */,
        };
    }
}
```

For nested objects, call their RequestExample() methods:

```csharp
public static CreateBeneficiaryRequest RequestExample()
{
    return new CreateBeneficiaryRequest
    {
        MemberId = 12345,
        RelationshipCode = "SPOUSE",
        BeneficiaryType = "PRIMARY",
        Address = Address.RequestExample(), // Call nested DTO's method
    };
}
```

---

## Compliance Notes

- **Analyzer Rule:** DSM013 enforces RequestExample() and ResponseExample() methods on all request/response DTOs
- **Build Impact:** Missing RequestExample() methods will trigger analyzer warnings/errors
- **OpenAPI Documentation:** RequestExample() methods are essential for proper Swagger/OpenAPI schema generation
- **Test Coverage:** Each RequestExample() should be testable for null/empty validation

---

## Data Sources

- Comprehensive grep search of all 68 endpoints in `Endpoints/` subdirectories
- Cross-reference with `DTO_INVENTORY_COMPLETE.md` (existing inventory)
- Analysis includes all Request DTOs from endpoints, whether defined in Common/Contracts or inline

---

## File Location

Generated report: `ENDPOINT_REQUEST_DTOS_ANALYSIS.json`
