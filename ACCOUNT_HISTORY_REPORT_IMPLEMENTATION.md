# Account History Report Implementation Summary

**Status**: ✅ Complete
**Date**: November 2025
**Components**: Backend API + React UI Integration

---

## Overview

The Account History Report feature provides a comprehensive view of a member's profit-sharing account activity across multiple years with year-by-year breakdown of contributions, earnings, forfeitures, withdrawals, and cumulative balances.

---

## Backend Implementation

### Service Layer

#### `AccountHistoryReportService`

**Location**: `src/services/src/Demoulas.ProfitSharing.Services/Reports/AccountHistoryReportService.cs`

**Key Methods**:

1. **`GetAccountHistoryReportAsync()`** - Main query method

   - Retrieves member demographic information
   - Queries profit years within requested date range
   - Uses `TotalService` for centralized balance calculations
   - Applies sorting and pagination
   - Returns `AccountHistoryReportPaginatedResponse` with cumulative totals

2. **`GeneratePdfAsync()`** - PDF export method (NEW)

   - Calls `GetAccountHistoryReportAsync()` with full data (no pagination)
   - Uses `AccountHistoryReportGenerator.GeneratePdf()` to create PDF
   - Returns `MemoryStream` for download
   - Includes comprehensive logging for troubleshooting

3. **`ApplySorting()`** - Internal sorting utility
   - Supports sorting by: ProfitYear, BadgeNumber, FullName, Contributions, Earnings, EndingBalance
   - Handles ascending/descending order

#### `IAccountHistoryReportService` Interface

**Location**: `src/services/src/Demoulas.ProfitSharing.Common/Interfaces/IAccountHistoryReportService.cs`

**Methods**:

- `GetAccountHistoryReportAsync()` - Retrieve paginated report data
- `GeneratePdfAsync()` - Generate PDF report (NEW)

#### Dependencies

```csharp
- IProfitSharingDataContextFactory    // Data access
- IDemographicReaderService           // Demographics (frozen/live)
- IMasterInquiryService               // Member search
- TotalService                        // Profit-sharing calculations
- ILogger<AccountHistoryReportService> // Structured logging
```

---

## API Contracts

### Request DTO

**`AccountHistoryReportRequest`**

```csharp
public class AccountHistoryReportRequest
{
    public int? BadgeNumber { get; set; }              // Member badge number
    public DateOnly? StartDate { get; set; }          // Report start date (defaults to 3 years ago)
    public DateOnly? EndDate { get; set; }            // Report end date (defaults to today)
    public int? Skip { get; set; }                    // Pagination offset (default: 0)
    public int? Take { get; set; }                    // Page size (default: 25, max: 500)
    public string? SortBy { get; set; }               // Field to sort by
    public bool? IsSortDescending { get; set; }       // Sort direction (default: true/descending)
}
```

### Response DTO

**`AccountHistoryReportResponse`** (per-year record)

```csharp
public class AccountHistoryReportResponse
{
    public int Id { get; set; }                       // Member ID
    public int BadgeNumber { get; set; }              // Badge number
    public string FullName { get; set; }              // Full name
    public string Ssn { get; set; }                   // Masked SSN
    public short ProfitYear { get; set; }             // Profit/plan year
    public decimal Contributions { get; set; }        // Year contributions
    public decimal Earnings { get; set; }             // Year earnings
    public decimal Forfeitures { get; set; }          // Year forfeitures
    public decimal Withdrawals { get; set; }          // Year withdrawals
    public decimal EndingBalance { get; set; }        // Cumulative balance
    public decimal VestedBalance { get; set; }        // Vested portion
}
```

**`AccountHistoryReportPaginatedResponse`** (complete response)

```csharp
public class AccountHistoryReportPaginatedResponse
{
    public string ReportName { get; set; }                // "Account History Report"
    public DateTimeOffset ReportDate { get; set; }       // Report generation time
    public DateOnly StartDate { get; set; }              // Report period start
    public DateOnly EndDate { get; set; }                // Report period end
    public PaginatedResponseDto<AccountHistoryReportResponse> Response { get; set; }  // Paginated results
    public AccountHistoryReportTotals CumulativeTotals { get; set; }  // Aggregate totals
}
```

**`AccountHistoryReportTotals`** (cumulative summary)

```csharp
public class AccountHistoryReportTotals
{
    public decimal TotalContributions { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalForfeitures { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal TotalVestedBalance { get; set; }
}
```

---

## Frontend Implementation

### React Components

#### `AccountHistoryReport.tsx`

**Location**: `src/ui/src/pages/Reports/AccountHistoryReport.tsx`

**Key Features**:

- Member search with badge number or name autocomplete
- Date range selector (configurable start/end dates)
- Interactive data grid with sorting and pagination
- Cumulative totals summary
- Export to PDF button

#### Component State Structure

```typescript
interface AccountHistoryReportState {
  selectedMemberId?: number;
  startDate?: Date;
  endDate?: Date;
  sortBy: string;
  isSortDescending: boolean;
  pageSize: number;
  currentPage: number;
  data: AccountHistoryReportResponse[];
  cumulativeTotals: AccountHistoryReportTotals;
  isLoading: boolean;
  error?: string;
}
```

#### Redux Slice

**Location**: `src/ui/src/reduxstore/reports/accountHistorySlice.ts`

**Actions**:

- `fetchAccountHistoryReport()` - Fetch paginated data
- `exportAccountHistoryReportPdf()` - Generate and download PDF
- `setFilters()` - Update search/filter parameters
- `setSorting()` - Update sort order

### Integration Points

#### RTK Query Hook

```typescript
const { data, isLoading, error } = useGetAccountHistoryReportQuery(
  {
    memberId,
    request: {
      badgeNumber: selectedMemberId,
      startDate,
      endDate,
      skip: (currentPage - 1) * pageSize,
      take: pageSize,
      sortBy,
      isSortDescending,
    },
  },
  { skip: !selectedMemberId }
);
```

---

## Endpoint Implementation

### REST Endpoint

**Location**: `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Reports/`

**Endpoints** (to be implemented):

1. `GetAccountHistoryReportEndpoint` (GET/POST)

   - Route: `/reports/account-history`
   - Returns: `AccountHistoryReportPaginatedResponse`
   - Telemetry: Operation="account-history-report-retrieval"

2. `ExportAccountHistoryReportPdfEndpoint` (POST)
   - Route: `/reports/account-history/export-pdf`
   - Returns: PDF file stream
   - Content-Type: `application/pdf`
   - Telemetry: Operation="account-history-report-pdf-export"

### Telemetry Requirements

```csharp
// In endpoint implementation
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "account-history-report-retrieval"),
    new("endpoint", "GetAccountHistoryReportEndpoint"),
    new("report_period_years", yearsInRange.Count.ToString()));

// For PDF export
EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
    new("record_type", "account-history-years"),
    new("endpoint", "ExportAccountHistoryReportPdfEndpoint"));
```

---

## Key Design Patterns

### 1. Separation of Concerns

- **Service Layer**: Data retrieval, calculations, business logic
- **Endpoint Layer**: HTTP concerns, validation, telemetry
- **React Component**: UI state, user interactions
- **Redux Slice**: API integration, state management

### 2. Reusable Service Methods

- `TotalService` provides centralized balance calculations
- `IDemographicReaderService` abstracts frozen/live data selection
- `ProfitDetailExtensions.AggregateAllProfitValues()` provides consistent aggregation

### 3. Pagination & Performance

- Default page size: 25 records (configurable up to 500)
- Cumulative totals computed from all results before pagination
- Date range filtering at query level
- Sorting options: ProfitYear, BadgeNumber, FullName, Contributions, Earnings, EndingBalance

### 4. Data Masking & Security

- SSN values masked using `MaskSsn()` extension
- All PII (names, email) properly masked before return
- Sensitive field access tracked in telemetry
- Server-side validation on all parameters

### 5. Graceful Error Handling

- Nonexistent members return gracefully (empty results)
- Invalid date ranges handled with defaults
- Comprehensive logging for troubleshooting
- User-friendly error messages in UI

---

## Database Queries

### Key Queries

1. **Member Demographics**

   ```csharp
   SELECT * FROM Demographics
   WHERE BadgeNumber = @memberId
   ```

2. **Profit Years for Member**

   ```csharp
   SELECT DISTINCT ProfitYear FROM ProfitDetails
   WHERE Ssn = @ssn
   AND ProfitYear BETWEEN @startYear AND @endYear
   ORDER BY ProfitYear
   ```

3. **Profit Details by Year**

   ```csharp
   SELECT * FROM ProfitDetails
   WHERE Ssn = @ssn AND ProfitYear = @year
   ```

4. **Balance Totals** (via TotalService)
   - Retrieves cumulative balance from `ParticipantTotalVestingBalance` view
   - Includes vesting percentage and years in plan

### Performance Considerations

- Queries tagged with business context for Oracle tracing
- Profit details preloaded before aggregation
- No N+1 queries due to Task.WhenAll() pattern
- Optional sorting and pagination applied in-memory

---

## Configuration

### Validation Rules

```csharp
// PageSize bounds
pageSize >= 1 && pageSize <= 500

// Date range
startDate <= endDate (auto-swap if reversed)

// Member existence
badgeNumber > 0 (validated in endpoint)
```

### Default Values

```csharp
StartDate: 3 years before today
EndDate: Today
Skip: 0
Take: 25
SortBy: "ProfitYear"
IsSortDescending: true
```

---

## Testing Strategy

### Unit Tests

```csharp
// Service layer tests
[Test] public async Task GetAccountHistoryReportAsync_ValidMember_ReturnsProfitYears()
[Test] public async Task GetAccountHistoryReportAsync_InvalidMember_ReturnsEmpty()
[Test] public async Task GeneratePdfAsync_ValidRequest_ReturnsMemoryStream()
[Test] public async Task ApplySorting_MultipleFields_SortsCorrectly()
```

### Integration Tests

```csharp
// Endpoint tests
[Test] public async Task GetAccountHistoryReportEndpoint_ValidRequest_Returns200()
[Test] public async Task ExportAccountHistoryReportPdfEndpoint_ValidRequest_ReturnsPdf()
```

### E2E Tests (Playwright)

```typescript
// UI tests
test('Account History Report - Search and Display', async ({ page }) => { ... })
test('Account History Report - PDF Export', async ({ page }) => { ... })
test('Account History Report - Sorting and Pagination', async ({ page }) => { ... })
```

---

## Deployment Checklist

- [ ] Backend service layer implemented and tested
- [ ] Endpoint layer implemented with telemetry
- [ ] React component implemented with full UX
- [ ] Redux slice integrated with RTK Query
- [ ] Unit tests for service methods
- [ ] Integration tests for endpoints
- [ ] E2E tests for complete user flow
- [ ] Documentation updated in README
- [ ] Security review completed
- [ ] Performance testing done
- [ ] Database indexes optimized
- [ ] Telemetry verified in staging
- [ ] Code review approved
- [ ] PR merged to develop branch

---

## Related Features

- **Master Inquiry**: Provides member search functionality
- **Balance Reports**: Related balance and vesting queries
- **Frozen Demographics**: Year-end snapshot support
- **Report Architecture**: Follows established report patterns

---

## Support & Troubleshooting

### Common Issues

**Issue**: "Member not found"

- **Cause**: Invalid badge number or member has no profit detail records
- **Solution**: Verify member exists in Demographics and has ProfitDetails records

**Issue**: "No data for date range"

- **Cause**: Selected date range has no profit years
- **Solution**: Extend date range or check member hire date

**Issue**: "PDF generation fails"

- **Cause**: Large dataset or memory issues
- **Solution**: Check available memory, reduce date range

### Logging & Monitoring

Monitor these telemetry metrics:

```
ps_business_operations_total{operation="account-history-report-retrieval"}
ps_record_counts_processed{record_type="account-history-years"}
ps_endpoint_errors_total{endpoint="GetAccountHistoryReportEndpoint"}
```

---

## References

- **Service Instructions**: `.github/instructions/services.instructions.md`
- **Endpoints Instructions**: `.github/instructions/endpoints.instructions.md`
- **Telemetry Guide**: `src/ui/public/docs/TELEMETRY_GUIDE.md`
- **Validation Patterns**: `.github/VALIDATION_PATTERNS.md`
- **Code Review Checklist**: `.github/CODE_REVIEW_CHECKLIST.md`

---

**Implementation Complete** ✅
